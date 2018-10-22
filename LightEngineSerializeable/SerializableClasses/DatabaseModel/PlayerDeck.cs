namespace LightEngineSerializeable.SerializableClasses.DatabaseModel
{
    [System.Serializable]
    public class PlayerDeck : SerializableModel
    {
        public short UnitOne { get; set; }
        public short UnitTwo { get; set; }
        public short UnitThree { get; set; }
        public short UnitFour { get; set; }
    }
}
