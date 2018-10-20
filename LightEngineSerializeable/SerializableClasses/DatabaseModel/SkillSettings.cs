namespace LightEngineSerializeable.SerializableClasses.DatabaseModel
{
    [System.Serializable]
    public class SkillSettings : SerializableModel
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public float Radius { get; set; }
        public float Density { get; set; }
        public float Value1 { get; set; }
        public float Value2 { get; set; }
        public float Value3 { get; set; }
    }
}
