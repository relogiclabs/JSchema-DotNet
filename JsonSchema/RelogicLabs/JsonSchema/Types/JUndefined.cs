namespace RelogicLabs.JsonSchema.Types;

public sealed class JUndefined : JLeaf
{
    public const string UndefinedMarker = "!";

    private JUndefined(Builder builder) : base(builder) { }

    public override bool Match(JNode node) => true;

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        return obj.GetType() == this.GetType();
    }

    public override int GetHashCode() => UndefinedMarker.GetHashCode();
    public override string ToString() => UndefinedMarker;

    internal new class Builder : JNode.Builder
    {
        public override JUndefined Build() => Build(new JUndefined(this));
    }
}