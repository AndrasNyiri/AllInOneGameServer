public class PositionSyncEvent : GameEvent
{
    public int Id { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }

    public PositionSyncEvent()
    {
        this.Type = GameEventType.PositionSync;
    }
}
