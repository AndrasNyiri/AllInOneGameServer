using LightEngineSerializeable.SerializableClasses.Enums;

namespace LightEngineSerializeable.SerializableClasses.GameModel.RequestEvents
{
    public class PlayUnitAbilityRequest : RequestEvent
    {
        public float DirectionX { get; set; }
        public float DirectionY { get; set; }

        public PlayUnitAbilityRequest() : base(RequestEventType.PlayUnitAbility)
        {

        }
    }
}
