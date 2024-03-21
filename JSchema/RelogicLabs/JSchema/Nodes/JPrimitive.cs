namespace RelogicLabs.JSchema.Nodes;

public abstract class JPrimitive : JLeaf, IJsonType
{
    private protected JPrimitive(Builder builder) : base(builder) { }
    private protected JPrimitive(JPrimitive node) : base(node) { }
    public JNode Node => this;

    internal abstract class Builder<T> : JNode.Builder
    {
        public T? Value { get; init; }
    }
}