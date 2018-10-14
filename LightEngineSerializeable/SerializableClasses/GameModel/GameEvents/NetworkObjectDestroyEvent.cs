using LightEngineSerializeable.SerializableClasses.Enums;

namespace LightEngineSerializeable.SerializableClasses.GameModel.GameEvents
{
    public class NetworkObjectDestroyEvent : GameEvent
    {
        public ushort Id { get; set; }

        public NetworkObjectDestroyEvent() : base(GameEventType.NetworkObjectDestroy)
        {

        }
    }
}
