namespace RelogicLabs.JsonSchema.Types;

public interface IJsonType
{
    public JsonType Type { get; }
    public JNode Node { get; }
}