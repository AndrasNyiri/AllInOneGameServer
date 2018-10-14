using System;
using LightEngineSerializeable.LiteNetLib;
using LightEngineSerializeable.LiteNetLib.Utils;
using LightEngineSerializeable.SerializableClasses;
using LightEngineSerializeable.SerializableClasses.DatabaseModel;
using LightEngineSerializeable.SerializableClasses.Enums;
using LightEngineSerializeable.Utils;
using LightEngineSerializeable.Utils.Serializers;
using LightGameServer.NetworkHandling.Model;
using NLog;

namespace LightGameServer.NetworkHandling.Handlers
{
    class NetworkCommandHandler
    {
        public static NetworkCommandHandler New(NetDataReader reader, NetPeer peer)
        {
            return new NetworkCommandHandler(reader, peer);
        }

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly NetDataReader _reader;
        private readonly NetPeer _peer;

        public NetworkCommandHandler(NetDataReader reader, NetPeer peer)
        {
            _reader = reader;
            _peer = peer;
        }

        public void HandleReceived()
        {
            try
            {
                NetworkCommand command = (NetworkCommand)_reader.GetByte();
                switch (command)
                {
                    case NetworkCommand.Register:
                        string nameToRegister = _reader.GetString();
                        PlayerData newPlayerData = Server.Get().QueryRepository.CreatePlayerData(nameToRegister);
                        Server.Get().PeerInfos[_peer] = new PeerInfo
                        {
                            DeviceId = newPlayerData.DeviceId,
                            PlayerData = newPlayerData,
                            NetPeer = _peer
                        };
                        DataSender.New(_peer).Send(newPlayerData.Serialize(NetworkCommand.Register), SendOptions.ReliableOrdered);
                        break;
                    case NetworkCommand.GetPlayerData:
                        PeerInfo getPlayerDataPeerInfo = Server.Get().PeerInfos[_peer];
                        PlayerData gotPlayerData = Server.Get().QueryRepository.GetPlayerData(getPlayerDataPeerInfo.DeviceId);
                        Server.Get().PeerInfos[_peer].PlayerData = gotPlayerData;
                        DataSender.New(_peer).Send(gotPlayerData.Serialize(NetworkCommand.GetPlayerData), SendOptions.ReliableOrdered);
                        break;
                    case NetworkCommand.Login:
                        string loginDeviceId = _reader.GetString();
                        PlayerData loginPlayerData = Server.Get().QueryRepository.GetPlayerData(loginDeviceId);
                        Server.Get().PeerInfos[_peer] = new PeerInfo
                        {
                            DeviceId = loginPlayerData.DeviceId,
                            PlayerData = loginPlayerData,
                            NetPeer = _peer
                        };
                        DataSender.New(_peer).Send(loginPlayerData.Serialize(NetworkCommand.Login), SendOptions.ReliableOrdered);
                        break;
                    case NetworkCommand.StartGame:
                        Server.Get().AddToPendingPool(_peer);
                        DataSender.New(_peer).Send(NetworkCommand.StartGame, SendOptions.ReliableOrdered);
                        break;
                    case NetworkCommand.RequestEventOption:
                        RequestEventSerializer requestSerializer = new RequestEventSerializer();
                        var requests = requestSerializer.Deserialize(_reader);
                        var requestPeerInfo = Server.Get().PeerInfos[_peer];
                        var playerMatch = Server.Get().GameManager.GetMatch(requestPeerInfo.PlayerData.PlayerId);
                        if (playerMatch != null) playerMatch.ProcessRequests(requestPeerInfo, requests.ToArray());
                        break;
                    case NetworkCommand.GetStaticData:
                        StaticData storedStaticData = Server.Get().DataStore.Data;
                        string clientStaticDataTimeStamp = _reader.GetString();
                        if (clientStaticDataTimeStamp != storedStaticData.TimeStamp)
                        {
                            DataSender.New(_peer).Send(storedStaticData.Serialize(NetworkCommand.GetStaticData, false), SendOptions.ReliableOrdered);
                        }
                        else
                        {
                            DataSender.New(_peer).Send(NetworkCommand.GetStaticData, SendOptions.ReliableOrdered, true);
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }
    }
}
