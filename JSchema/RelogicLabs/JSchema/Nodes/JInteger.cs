using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Message;
using RelogicLabs.JSchema.Types;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.ErrorDetail;
using static RelogicLabs.JSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JSchema.Nodes;

public sealed class JInteger : JNumber, IEInteger, IPragmaValue<long>
{
    public long Value { get; }

    private JInteger(Builder builder) : base(builder)
        => Value = RequireNonNull(builder.Value);

    public override bool Match(JNode node)
    {
        var other = CastType<JInteger>(node);
        if(other == null) return false;
        if(Value == other.Value) return true;
        return Fail(new JsonSchemaException(
            new ErrorDetail(INTE01, ValueMismatch),
            ExpectedDetail.AsValueMismatch(this),
            ActualDetail.AsValueMismatch(other)));
    }

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        if(obj.GetType() != this.GetType()) return false;
        JInteger other = (JInteger) obj;
        return Value == other.Value;
    }

    public override int GetHashCode() => Value.GetHashCode();
    public static implicit operator long(JInteger node) => node.Value;
    public static implicit operator double(JInteger node) => node.Value;
    public override double ToDouble() => Value;
    public override string ToString() => Value.ToString();

    internal new sealed class Builder : JPrimitive.Builder<long>
    {
        public override JInteger Build() => Build(new JInteger(this));
    }
}