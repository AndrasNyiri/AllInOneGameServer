using LightEngineSerializeable.SerializableClasses.Enums;

namespace LightEngineSerializeable.SerializableClasses.GameModel.GameEvents
{
    public class NetworkObjectSpawnEvent : GameEvent
    {
        public ushort Id { get; set; }
        public byte ObjectType { get; set; }
        public short PositionX { get; set; }
        public short PositionY { get; set; }

        public NetworkObjectSpawnEvent() : base(GameEventType.NetworkObjectSpawn)
        {
        }
    }
}
