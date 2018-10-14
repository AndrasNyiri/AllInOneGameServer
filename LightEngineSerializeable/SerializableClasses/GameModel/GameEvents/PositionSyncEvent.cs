using LightEngineSerializeable.SerializableClasses.Enums;

namespace LightEngineSerializeable.SerializableClasses.GameModel.GameEvents
{
    public class PositionSyncEvent : GameEvent
    {
        public ushort Id { get; set; }
        public short PositionX { get; set; }
        public short PositionY { get; set; }

        public PositionSyncEvent() : base(GameEventType.PositionSync)
        {
        }
    }
}
