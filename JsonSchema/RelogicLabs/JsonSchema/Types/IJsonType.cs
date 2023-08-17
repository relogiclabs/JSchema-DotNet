namespace RelogicLabs.JsonSchema.Types;

public interface IJsonType<out T> where T : JNode
{
    public T Node => (T) this;
    public JsonType Type { get; }
}