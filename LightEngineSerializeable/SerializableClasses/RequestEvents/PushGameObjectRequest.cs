public class PushGameObjectRequest : RequestEvent
{
    public ushort GameObjectId { get; set; }
    public float DirectionX { get; set; }
    public float DirectionY { get; set; }

    public PushGameObjectRequest() : base(RequestEventType.PushGameObject)
    {
    }
}
