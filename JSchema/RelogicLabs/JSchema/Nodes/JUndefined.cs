using RelogicLabs.JSchema.Types;
using static RelogicLabs.JSchema.Types.IEUndefined;

namespace RelogicLabs.JSchema.Nodes;

public sealed class JUndefined : JLeaf, IEUndefined
{
    private JUndefined(Builder builder) : base(builder) { }

    public override bool Match(JNode node) => true;

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        return obj.GetType() == this.GetType();
    }

    public override int GetHashCode() => Marker.GetHashCode();
    public override string ToString() => Marker;

    internal new sealed class Builder : JNode.Builder
    {
        public override JUndefined Build() => Build(new JUndefined(this));
    }
}