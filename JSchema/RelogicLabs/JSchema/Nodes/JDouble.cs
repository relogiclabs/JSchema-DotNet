using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Message;
using RelogicLabs.JSchema.Types;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.ErrorDetail;
using static RelogicLabs.JSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JSchema.Nodes;

public sealed class JDouble : JNumber, IEDouble, IPragmaValue<double>
{
    public double Value { get; }

    private JDouble(Builder builder) : base(builder)
        => Value = RequireNonNull(builder.Value);

    public override bool Match(JNode node)
    {
        var other = CastType<JDouble>(node);
        if(other == null) return false;
        if (AreEqual(Value, other.Value)) return true;
        return Fail(new JsonSchemaException(
            new ErrorDetail(DUBL01, ValueMismatch),
            ExpectedDetail.AsValueMismatch(this),
            ActualDetail.AsValueMismatch(other)));
    }

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        if(obj.GetType() != this.GetType()) return false;
        JDouble other = (JDouble) obj;
        return AreEqual(Value, other.Value);
    }

    public static implicit operator double(JDouble node) => node.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override double ToDouble() => Value;
    public override string ToString() => $"{Value:0.###############E+0}";

    internal new sealed class Builder : JPrimitive.Builder<double>
    {
        public override JDouble Build() => Build(new JDouble(this));
    }
}