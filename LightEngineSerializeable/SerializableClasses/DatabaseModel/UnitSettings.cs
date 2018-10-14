namespace LightEngineSerializeable.SerializableClasses.DatabaseModel
{
    [System.Serializable]
    public class UnitSettings : SerializableModel
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public float Density { get; set; }
        public float Radius { get; set; }
        public short PushForce { get; set; }
        public short Hp { get; set; }
        public short Damage { get; set; }
        public short ProjectileDamage { get; set; }
    }
}
