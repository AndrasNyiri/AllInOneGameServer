using LightEngineSerializeable.SerializableClasses.Enums;

namespace LightEngineSerializeable.SerializableClasses.GameModel.RequestEvents
{
    public class SetAimDirectionRequest : RequestEvent
    {
        public bool Active { get; set; }
        public float DirectionX { get; set; }
        public float DirectionZ { get; set; }

        public SetAimDirectionRequest() : base(RequestEventType.SetAimDirection)
        {
        }
    }
}
