using LightEngineSerializeable.SerializableClasses.DatabaseModel;
using LightEngineSerializeable.SerializableClasses.Enums;

namespace LightEngineSerializeable.SerializableClasses.GameModel.GameEvents
{
    public class GameStartEvent : GameEvent
    {
        public PlayerType PlayerType { get; set; }
        public byte LevelId { get; set; }
        public NetworkObjectSpawnEvent[] SpawnEvents { get; set; }
        public PlayerData EnemyPlayerData { get; set; }
        public DeckGameObjectBind[] MyDeckBind { get; set; }
        public DeckGameObjectBind[] EnemyDeckBind { get; set; }

        public GameStartEvent() : base(GameEventType.GameStart)
        {
        }
    }
}

