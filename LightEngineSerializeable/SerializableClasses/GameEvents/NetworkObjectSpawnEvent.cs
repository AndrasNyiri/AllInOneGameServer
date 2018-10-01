public class NetworkObjectSpawnEvent : GameEvent
{
    public int Id { get; set; }
    public NetworkObjectType ObjectType { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }

    public NetworkObjectSpawnEvent()
    {
        this.Type = GameEventType.NetworkObjectSpawn;
    }
}
