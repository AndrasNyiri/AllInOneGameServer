using LightEngineSerializeable.SerializableClasses.DatabaseModel;
using LightEngineSerializeable.SerializableClasses.Enums;

namespace LightEngineSerializeable.SerializableClasses.GameModel
{
    [System.Serializable]
    public class RequestEvent : SerializableModel
    {
        public RequestEventType Type { get; protected set; }
        public float TimeStamp { get; set; }


        public RequestEvent(RequestEventType type)
        {
            this.Type = type;
        }
    }
}




