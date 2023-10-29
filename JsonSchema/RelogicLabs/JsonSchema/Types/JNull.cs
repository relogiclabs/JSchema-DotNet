namespace RelogicLabs.JsonSchema.Types;

public sealed class JNull : JPrimitive
{
    public const string NullMarker = "null";

    private JNull(Builder builder) : base(builder) { }

    public override bool Match(JNode node) => CheckType<JNull>(node);
    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        return obj.GetType() == this.GetType();
    }

    public override JsonType Type => JsonType.NULL;
    public override int GetHashCode() => NullMarker.GetHashCode();
    public override string ToString() => NullMarker;

    internal new class Builder : JNode.Builder
    {
        public override JNull Build() => Build(new JNull(this));
    }
}