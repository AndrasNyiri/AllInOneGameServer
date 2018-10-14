using LightEngineSerializeable.SerializableClasses.Enums;

namespace LightEngineSerializeable.SerializableClasses.GameModel.GameEvents
{
    public class CanPlayEvent : GameEvent
    {
        public bool CanPlay { get; set; }
        public ushort SelectedUnitId { get; set; }
        public CanPlayEvent() : base(GameEventType.CanPlay)
        {
        }
    }
}
