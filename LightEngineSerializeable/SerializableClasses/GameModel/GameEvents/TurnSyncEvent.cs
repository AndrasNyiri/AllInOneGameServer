public class TurnSyncEvent : GameEvent
{
    public PlayerType PlayerType { get; set; }

    public TurnSyncEvent() : base(GameEventType.TurnSync)
    {
    }
}
