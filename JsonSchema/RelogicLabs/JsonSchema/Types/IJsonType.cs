namespace RelogicLabs.JsonSchema.Types;

public interface IJsonType
{
    public JsonType Type => JsonType.ANY;
    public JNode Node { get; }
}