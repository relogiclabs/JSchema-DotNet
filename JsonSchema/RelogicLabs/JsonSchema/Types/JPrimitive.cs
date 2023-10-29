namespace RelogicLabs.JsonSchema.Types;

public abstract class JPrimitive : JLeaf, IJsonType
{
    private protected JPrimitive(Builder builder) : base(builder) { }
    public virtual JsonType Type => JsonType.PRIMITIVE;
    public JNode Node => this;

    internal abstract class Builder<T> : JNode.Builder
    {
        public T? Value { get; init; }
    }
}