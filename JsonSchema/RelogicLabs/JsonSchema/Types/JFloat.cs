using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Message.ErrorDetail;
using static RelogicLabs.JsonSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JsonSchema.Types;

public sealed class JFloat : JNumber, IPragmaValue<double>
{
    public double Value { get; }

    private JFloat(Builder builder) : base(builder)
        => Value = RequireNonNull(builder.Value);

    public override bool Match(JNode node)
    {
        var other = CastType<JFloat>(node);
        if(other == null) return false;
        if(AreEqual(Value, other.Value)) return true;
        return FailWith(new JsonSchemaException(
            new ErrorDetail(FLOT01, ValueMismatch),
            ExpectedDetail.AsValueMismatch(this),
            ActualDetail.AsValueMismatch(other)));
    }

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        if(obj.GetType() != this.GetType()) return false;
        JFloat other = (JFloat) obj;
        return AreEqual(Value, other.Value);
    }

    public override JsonType Type => JsonType.FLOAT;
    public override int GetHashCode() => Value.GetHashCode();
    public static implicit operator double(JFloat node) => node.Value;
    protected override double ToDouble() => Convert.ToDouble(Value);
    public override string ToString() => $"{Value:0.0##############}";

    internal new class Builder : JPrimitive.Builder<double>
    {
        public override JFloat Build() => Build(new JFloat(this));
    }
}