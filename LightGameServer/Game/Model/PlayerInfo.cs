using System.Linq;
using LightEngineSerializeable.SerializableClasses.Enums;
using LightGameServer.Game.Prefabs.Units;
using LightGameServer.NetworkHandling.Model;

namespace LightGameServer.Game.Model
{
    class PlayerInfo
    {
        public PeerInfo PeerInfo { get; set; }
        public PlayerType PlayerType { get; set; }
        public Unit[] Deck { get; set; }
        public int DeckIndex { get; set; }
        public bool CanPlay { get; set; }

        public bool IncrementDeckIndex()
        {
            int currentIndex = DeckIndex;

            for (int i = 0; i < Deck.Length; i++)
            {
                currentIndex =
                    (ushort)((currentIndex + 1) % Deck.Length);
                if (Deck[currentIndex].IsAlive)
                {
                    DeckIndex = currentIndex;
                    return true;
                }
            }

            return false;
        }

        public bool IsInDeck(Unit unit)
        {
            return Deck.ToList().Find(u => u == unit) != null;
        }

        public Unit GetSelectedUnit()
        {
            return Deck[DeckIndex];
        }

        public ushort GetSelectedGoId()
        {
            return (ushort)Deck[DeckIndex].id;
        }
    }
}
