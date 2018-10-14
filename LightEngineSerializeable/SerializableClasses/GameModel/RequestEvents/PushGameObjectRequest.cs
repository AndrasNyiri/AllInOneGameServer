using LightEngineSerializeable.SerializableClasses.Enums;

namespace LightEngineSerializeable.SerializableClasses.GameModel.RequestEvents
{
    public class PushGameObjectRequest : RequestEvent
    {
        public float DirectionX { get; set; }
        public float DirectionY { get; set; }

        public PushGameObjectRequest() : base(RequestEventType.PushGameObject)
        {
        }
    }
}
