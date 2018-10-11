public class CanPlayEvent : GameEvent
{
    public bool CanPlay { get; set; }
    public CanPlayEvent() : base(GameEventType.CanPlay)
    {
    }
}
