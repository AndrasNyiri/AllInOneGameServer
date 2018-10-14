using LightEngineSerializeable.SerializableClasses.Enums;

namespace LightEngineSerializeable.SerializableClasses.GameModel
{
    [System.Serializable]
    public class GameEvent
    {
        public GameEventType Type { get; protected set; }

        public GameEvent(GameEventType type)
        {
            this.Type = type;
        }
    }
}
