using LightEngineSerializeable.SerializableClasses.DatabaseModel;
using LightEngineSerializeable.SerializableClasses.Enums;

namespace LightEngineSerializeable.SerializableClasses.GameModel
{
    [System.Serializable]
    public class GameEvent : SerializableModel
    {
        public GameEventType Type { get; protected set; }
        public float TimeStamp { get; set; }

        public GameEvent(GameEventType type)
        {
            this.Type = type;
        }
    }
}
