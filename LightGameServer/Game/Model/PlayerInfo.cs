using LightEngineCore.Components;
using LightGameServer.NetworkHandling.Model;

namespace LightGameServer.Game.Model
{
    class PlayerInfo
    {
        public PeerInfo PeerInfo { get; set; }
        public PlayerType PlayerType { get; set; }
        public ushort SelectedGameObjectIndex { get; set; }
        public GameObject[] Deck { get; set; }
        public bool CanPlay { get; set; }

        public void IncrementSelectedIndex()
        {
            SelectedGameObjectIndex =
                (ushort)((SelectedGameObjectIndex + 1) % Deck.Length);
        }

        public ushort GetSelectedGoId()
        {
            return (ushort)Deck[SelectedGameObjectIndex].id;
        }
    }
}
