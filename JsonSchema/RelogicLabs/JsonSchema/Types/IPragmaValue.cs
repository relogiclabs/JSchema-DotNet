namespace RelogicLabs.JsonSchema.Types;

public interface IPragmaValue<out T>
{
    public T Value { get; }
}