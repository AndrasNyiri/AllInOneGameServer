namespace LightEngineSerializeable.SerializableClasses.DatabaseModel
{
    [System.Serializable]
    public class PlayerUnit : SerializableModel
    {
        public short UnitId { get; set; }
        public int Amount { get; set; }
    }
}
