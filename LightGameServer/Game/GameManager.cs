using System.Collections.Generic;
using LightEngineCore.Configuration;
using LightEngineCore.Loop;
using LightGameServer.NetworkHandling.Model;

namespace LightGameServer.Game
{
    class GameManager
    {
        private readonly List<Match> _matches = new List<Match>();
        private readonly LoopManager _loopManager = new LoopManager(Settings.simulatedThreadCount);

        public void StartMatch(PeerInfo playerOne, PeerInfo playerTwo)
        {
            Match newMatch = new Match(_loopManager.StartLoop(), playerOne, playerTwo);
            _matches.Add(newMatch);
        }
        public void StartMatch(PeerInfo playerOne)
        {
            Match newMatch = new Match(_loopManager.StartLoop(), playerOne, null);
            _matches.Add(newMatch);
        }

        public void StopMatch(Match match)
        {
            _loopManager.StopLoop(match.gameLoop);
            _matches.Remove(match);
        }

        public Match GetMatch(ulong playerId)
        {
            foreach (var match in _matches)
            {
                if (match.playerOne != null && match.playerOne.PlayerData != null && match.playerOne.PlayerData.PlayerId == playerId ||
                    match.playerTwo != null && match.playerTwo.PlayerData != null && match.playerTwo.PlayerData.PlayerId == playerId)
                {
                    return match;
                }
            }

            return null;
        }
    }
}
