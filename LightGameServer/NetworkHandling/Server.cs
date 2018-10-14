using System;
using System.Collections.Generic;
using System.Threading;
using LightEngineSerializeable.LiteNetLib;
using LightEngineSerializeable.SerializableClasses.DatabaseModel;
using LightEngineSerializeable.SerializableClasses.Enums;
using LightEngineSerializeable.Utils;
using LightGameServer.Database;
using LightGameServer.Game;
using LightGameServer.NetworkHandling.Handlers;
using LightGameServer.NetworkHandling.Model;
using NLog;

namespace LightGameServer.NetworkHandling
{
    class Server
    {
        #region Singleton

        private static Server _instance;

        public static Server Get()
        {
            if (_instance == null) _instance = new Server();
            return _instance;
        }

        #endregion


        private const string CONNECTION_KEY = "PzM@.5p&k!aZJXH6,mq44R\\ue?%BSSS*t\'N8xxH=L+\"S\'4^N,m5M{`N;>K]7{vUB[R!B\"?>sV!&d~b(G-pYW%5&,6_J5>Hky95.DTG_dhM^x]ph(&.\\.Xc(B.fFGW`e_";
        private const int PORT = 60001;
        public const int UPDATE_TIME = 33;

        public Dictionary<NetPeer, PeerInfo> PeerInfos { get; }
        public PendingGamePool PendingGamePool { get; }
        public GameManager GameManager { get; }
        public QueryRepository QueryRepository { get; }
        public DataStore DataStore { get; }

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public Server()
        {
            PeerInfos = new Dictionary<NetPeer, PeerInfo>();
            PendingGamePool = new PendingGamePool();
            GameManager = new GameManager();
            QueryRepository = new QueryRepository();
            DataStore = new DataStore(QueryRepository);
        }


        public void Start()
        {
            EventBasedNetListener listener = new EventBasedNetListener();
            NetManager server = new NetManager(listener, 10000, CONNECTION_KEY);
            server.Start(PORT);
            server.UpdateTime = UPDATE_TIME;
            Console.WriteLine("Game server is listening...");

            listener.NetworkErrorEvent += (point, code) =>
            {
                _logger.Error(string.Format("Error occured. Endpoint:{0} , ErrorCode: {1}", point, code));
            };

            listener.PeerConnectedEvent += peer =>
            {
                Console.WriteLine("We got connection: {0}", peer.EndPoint);
                PeerInfos.Add(peer, new PeerInfo());
            };

            listener.PeerDisconnectedEvent += (peer, info) =>
            {
                var match = GameManager.GetMatch(PeerInfos[peer].PlayerData.PlayerId);
                if (match != null) GameManager.StopMatch(match);
                PendingGamePool.RemoveLeaver(PeerInfos[peer]);
                PeerInfos.Remove(peer);
                Console.WriteLine("Peer disconnected: {0}", peer.EndPoint);
            };

            listener.NetworkReceiveEvent += (peer, reader) => { NetworkCommandHandler.New(reader, peer).HandleReceived(); };

            while (!Console.KeyAvailable)
            {
                ReslovePendingPool();
                server.PollEvents();
                Thread.Sleep(10);
            }
            server.Stop();
        }

        private void ReslovePendingPool()
        {
            var pairs = PendingGamePool.ResolvePendings();
            foreach (var playerPair in pairs)
            {
                DataSender.New(playerPair.PlayerOne.NetPeer).Send(playerPair.PlayerTwo.PlayerData.Serialize(NetworkCommand.GameStarted), SendOptions.ReliableOrdered);
                DataSender.New(playerPair.PlayerTwo.NetPeer).Send(playerPair.PlayerOne.PlayerData.Serialize(NetworkCommand.GameStarted), SendOptions.ReliableOrdered);
                GameManager.StartMatch(playerPair.PlayerOne, playerPair.PlayerTwo);
            }

            var waiters = PendingGamePool.ResolveWaiters();
            foreach (var waiter in waiters)
            {
                DataSender.New(waiter.NetPeer).Send(new PlayerData { Name = "BOT", LadderScore = waiter.PlayerData.LadderScore }.Serialize(NetworkCommand.GameStarted), SendOptions.ReliableOrdered);
                GameManager.StartMatch(waiter);
            }
        }

        public void AddToPendingPool(NetPeer peer)
        {
            PendingGamePool.AddPlayer(PeerInfos[peer]);
        }
    }
}
