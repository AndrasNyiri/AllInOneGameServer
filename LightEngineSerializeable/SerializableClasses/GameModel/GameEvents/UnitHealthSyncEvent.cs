using LightEngineSerializeable.SerializableClasses.Enums;

namespace LightEngineSerializeable.SerializableClasses.GameModel.GameEvents
{
    public class UnitHealthSyncEvent : GameEvent
    {
        public ushort Id { get; set; }
        public bool IsDamaged { get; set; }
        public short CurrentHealth { get; set; }
        public short HealthChanged { get; set; }

        public UnitHealthSyncEvent() : base(GameEventType.UnitHealthSync)
        {

        }
    }
}
