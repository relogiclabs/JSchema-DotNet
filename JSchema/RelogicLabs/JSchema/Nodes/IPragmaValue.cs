namespace RelogicLabs.JSchema.Nodes;

public interface IPragmaValue<out T>
{
    public T Value { get; }
}