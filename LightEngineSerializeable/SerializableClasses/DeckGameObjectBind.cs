using LightEngineSerializeable.SerializableClasses.DatabaseModel;

namespace LightEngineSerializeable.SerializableClasses
{
    public class DeckGameObjectBind : SerializableModel
    {
        public byte DeckIndex { get; set; }
        public ushort GameObjectId { get; set; }
    }
}
