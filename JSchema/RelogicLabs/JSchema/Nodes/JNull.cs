using RelogicLabs.JSchema.Types;
using static RelogicLabs.JSchema.Types.IENull;

namespace RelogicLabs.JSchema.Nodes;

public sealed class JNull : JPrimitive, IENull
{
    private JNull(Builder builder) : base(builder) { }

    public override bool Match(JNode node) => CheckType<JNull>(node);
    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        return obj.GetType() == this.GetType();
    }

    public override int GetHashCode() => Literal.GetHashCode();
    public override string ToString() => Literal;

    internal new sealed class Builder : JNode.Builder
    {
        public override JNull Build() => Build(new JNull(this));
    }
}