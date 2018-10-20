using LightEngineSerializeable.SerializableClasses.DatabaseModel;

namespace LightEngineSerializeable.SerializableClasses.GameModel.GameEvents
{
    public class PositionSyncEvent : SerializableModel
    {
        public ushort Id { get; set; }
        public short PositionX { get; set; }
        public short PositionY { get; set; }
    }
}
