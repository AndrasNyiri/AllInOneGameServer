namespace LightEngineSerializeable.SerializableClasses.DatabaseModel
{
    [System.Serializable]
    public class PlayerData : SerializableModel
    {
        public uint PlayerId { get; set; }
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public int Coin { get; set; }
        public int Diamond { get; set; }
        public int LadderScore { get; set; }
    }
}

