using System;
using LightEngineSerializeable.Utils;
using LightGameServer.Database;
using LightGameServer.NetworkHandling.Model;
using LiteNetLib;
using LiteNetLib.Utils;
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
                    case NetworkCommand.CommandObjectOption:
                        byte[] commandBytes = _reader.GetBytesWithLength();
                        CommandObject commandObject = ObjectSerializationUtil.CreateCommandObjectFromByteArray(commandBytes);
                        CommandObjectHandler.New(commandObject, _peer).HandleCommand();
                        break;
                    case NetworkCommand.Register:
                        string nameToRegister = _reader.GetString();
                        PlayerData newPlayerData = QueryRepository.Get().CreatePlayerData(nameToRegister);
                        Server.Get().peerInfos[_peer] = new PeerInfo
                        {
                            DeviceId = newPlayerData.DeviceId,
                            PlayerData = newPlayerData,
                            NetPeer = _peer
                        };
                        DataSender.New(_peer).SendCommandObject(new CommandObject(CommandObjectCommand.Register, newPlayerData));
                        break;
                    case NetworkCommand.GetPlayerData:
                        PeerInfo getPlayerDataPeerInfo = Server.Get().peerInfos[_peer];
                        PlayerData gotPlayerData = QueryRepository.Get().GetPlayerData(getPlayerDataPeerInfo.DeviceId);
                        Server.Get().peerInfos[_peer].PlayerData = gotPlayerData;
                        DataSender.New(_peer).SendCommandObject(new CommandObject(CommandObjectCommand.GetPlayerData, gotPlayerData));
                        break;
                    case NetworkCommand.Login:
                        string loginDeviceId = _reader.GetString();
                        PlayerData loginPlayerData = QueryRepository.Get().GetPlayerData(loginDeviceId);
                        Server.Get().peerInfos[_peer] = new PeerInfo
                        {
                            DeviceId = loginPlayerData.DeviceId,
                            PlayerData = loginPlayerData,
                            NetPeer = _peer
                        };
                        DataSender.New(_peer).SendCommandObject(new CommandObject(CommandObjectCommand.Login, loginPlayerData));
                        break;
                    case NetworkCommand.StartGame:
                        Server.Get().AddToPendingPool(_peer);
                        DataSender.New(_peer).Send(NetworkCommand.StartGame, SendOptions.ReliableOrdered);
                        break;
                    case NetworkCommand.RequestEventOption:
                        RequestEventSerializer requestSerializer = new RequestEventSerializer();
                        var requests = requestSerializer.Deserialize(_reader);
                        var requestPeerInfo = Server.Get().peerInfos[_peer];
                        var playerMatch = Server.Get().gameManager.GetMatch(requestPeerInfo.PlayerData.PlayerId);
                        if (playerMatch != null) playerMatch.ProcessRequests(requestPeerInfo, requests.ToArray());
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
