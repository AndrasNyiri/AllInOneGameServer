using System;
using LiteNetLib;

namespace LightGameServer.NetworkHandling.Model
{
    class PeerInfo
    {
        public PlayerData PlayerData { get; set; }
        public string DeviceId { get; set; }
        public NetPeer Peer { get; set; }
        public DateTime PendingPoolJoinTime { get; set; }
    }
}
