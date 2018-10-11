[System.Serializable]
public class RequestEvent
{
    public RequestEventType Type { get; protected set; }

    public RequestEvent(RequestEventType type)
    {
        this.Type = type;
    }
}




