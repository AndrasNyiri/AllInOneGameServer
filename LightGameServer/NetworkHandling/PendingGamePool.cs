using System;
using System.Collections.Generic;
using LightGameServer.NetworkHandling.Model;

namespace LightGameServer.NetworkHandling
{
    class PendingGamePool
    {
        private readonly List<PeerInfo> _pendingPool = new List<PeerInfo>();

        private const int LADDER_SCORE_MAX_DIFF = 100;
        private const int PENDING_WAIT_TIME = 2;

        public void AddPlayer(PeerInfo playerInfo)
        {
            playerInfo.PendingPoolJoinTime = DateTime.Now;
            _pendingPool.Add(playerInfo);
        }

        public void RemoveLeaver(PeerInfo peerInfo)
        {
            _pendingPool.Remove(peerInfo);
        }

        public List<PeerInfo> ResolveWaiters()
        {
            List<PeerInfo> waiters = new List<PeerInfo>();


            foreach (var peerInfo in _pendingPool)
            {
                var resolveTime = peerInfo.PendingPoolJoinTime.AddSeconds(PENDING_WAIT_TIME);
                if (DateTime.Now > resolveTime)
                {
                    waiters.Add(peerInfo);
                }
            }

            waiters.ForEach(w =>
            {
                _pendingPool.Remove(w);
            });

            return waiters;
        }

        public List<PlayerPair> ResolvePendings()
        {
            List<PlayerPair> pairs = new List<PlayerPair>();

            foreach (var playerPeerInfo in _pendingPool)
            {
                if (InPairs(pairs, playerPeerInfo)) continue;
                List<PeerInfo> candidates = new List<PeerInfo>();
                foreach (var otherPeerInfo in _pendingPool)
                {
                    if (playerPeerInfo == otherPeerInfo || InPairs(pairs, otherPeerInfo)) continue;
                    if (Math.Abs(playerPeerInfo.PlayerData.LadderScore - otherPeerInfo.PlayerData.LadderScore) < LADDER_SCORE_MAX_DIFF)
                    {
                        candidates.Add(otherPeerInfo);
                    }
                }

                PeerInfo match = null;
                int minDif = int.MaxValue;
                foreach (var candidate in candidates)
                {
                    var diff = Math.Abs(playerPeerInfo.PlayerData.LadderScore - candidate.PlayerData.LadderScore);
                    if (diff < minDif)
                    {
                        match = candidate;
                        minDif = diff;
                    }
                }

                if (match != null)
                {
                    pairs.Add(new PlayerPair { PlayerOne = playerPeerInfo, PlayerTwo = match });
                }
            }

            pairs.ForEach(p =>
            {
                _pendingPool.Remove(p.PlayerOne);
                _pendingPool.Remove(p.PlayerTwo);
            });

            return pairs;
        }

        private bool InPairs(List<PlayerPair> pairs, PeerInfo peerInfo)
        {
            foreach (var pair in pairs)
            {
                if (pair.PlayerOne == peerInfo || pair.PlayerTwo == peerInfo)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
