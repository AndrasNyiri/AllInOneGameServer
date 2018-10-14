using System;
using LightEngineSerializeable.LiteNetLib;
using LightEngineSerializeable.SerializableClasses.DatabaseModel;

namespace LightGameServer.NetworkHandling.Model
{
    class PeerInfo
    {
        public PlayerData PlayerData { get; set; }
        public string DeviceId { get; set; }
        public NetPeer NetPeer { get; set; }
        public DateTime PendingPoolJoinTime { get; set; }

        public bool IsConnected => NetPeer != null && NetPeer.ConnectionState == ConnectionState.Connected;
    }
}
