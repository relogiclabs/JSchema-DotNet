using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Message.ErrorDetail;

namespace RelogicLabs.JsonSchema.Types;

public class JInteger : JNumber, IPragmaValue<long>
{
    public required long Value { get; init; }

    internal JInteger(IDictionary<JNode, JNode> relations) : base(relations) { }

    public override bool Match(JNode node)
    {
        var other = CastType<JInteger>(node);
        if(other == null) return false;
        if(Value == other.Value) return true;
        return FailWith(new JsonSchemaException(
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

    public override JsonType Type => JsonType.INTEGER;
    public override int GetHashCode() => Value.GetHashCode();
    public static implicit operator long(JInteger integer) => integer.Value;
    public static implicit operator double(JInteger integer) => integer.Value;
    protected override double ToDouble() => Convert.ToDouble(Value);
    public override string ToString() => Value.ToString();
}