using LightEngineSerializeable.SerializableClasses.Enums;

namespace LightEngineSerializeable.SerializableClasses.GameModel.GameEvents
{
    public class TurnSyncEvent : GameEvent
    {
        public PlayerType PlayerType { get; set; }

        public TurnSyncEvent() : base(GameEventType.TurnSync)
        {
        }
    }
}
