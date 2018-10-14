using LightEngineSerializeable.SerializableClasses.Enums;

namespace LightEngineSerializeable.SerializableClasses.GameModel.GameEvents
{
    public class TurnSyncEvent : GameEvent
    {
        public byte PlayerType { get; set; }

        public TurnSyncEvent() : base(GameEventType.TurnSync)
        {
        }
    }
}
