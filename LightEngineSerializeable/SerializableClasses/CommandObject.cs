[System.Serializable]
public class CommandObject
{
    public CommandObjectCommand Command { get; }
    private readonly object[] _attributes;

    public CommandObject(CommandObjectCommand command, params object[] attributes)
    {
        this.Command = command;
        this._attributes = attributes;
    }

    public bool GetAttribute<T>(int index, out T result)
    {
        var obj = _attributes[index];

        if (obj is T)
        {
            result = (T)obj;
            return true;
        }

        result = default(T);
        return false;
    }

}
