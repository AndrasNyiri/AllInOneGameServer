using LightEngineSerializeable.SerializableClasses.Enums;

namespace LightEngineSerializeable.SerializableClasses.GameModel.GameEvents
{
    public class GameStartEvent : GameEvent
    {
        public float NetworkTime { get; set; }
        public PlayerType PlayerType { get; set; }
        public byte LevelId { get; set; }
        public bool CanPlay { get; set; }
        public NetworkObjectSpawnEvent[] SpawnEvents { get; set; }

        public GameStartEvent() : base(GameEventType.GameStart)
        {
        }
    }
}

