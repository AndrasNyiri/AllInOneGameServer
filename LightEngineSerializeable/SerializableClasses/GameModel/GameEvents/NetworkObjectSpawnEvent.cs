using LightEngineSerializeable.SerializableClasses.Enums;

namespace LightEngineSerializeable.SerializableClasses.GameModel.GameEvents
{
    public class NetworkObjectSpawnEvent : GameEvent
    {
        public ushort Id { get; set; }
        public NetworkObjectType ObjectType { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }

        public NetworkObjectSpawnEvent() : base(GameEventType.NetworkObjectSpawn)
        {
        }
    }
}
