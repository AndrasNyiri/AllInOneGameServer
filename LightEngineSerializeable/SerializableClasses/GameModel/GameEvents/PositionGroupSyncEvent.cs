public class PositionGroupSyncEvent : GameEvent
{
    public PositionGroupSyncEvent() : base(GameEventType.PositionGroupSync)
    {
    }

    public float TimeStamp { get; set; }
    public PositionSyncEvent[] PositionSyncs { get; set; }
}
