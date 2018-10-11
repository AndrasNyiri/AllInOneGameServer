public class PositionSyncEvent : GameEvent
{
    public ushort Id { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }

    public PositionSyncEvent() : base(GameEventType.PositionSync)
    {
    }
}
