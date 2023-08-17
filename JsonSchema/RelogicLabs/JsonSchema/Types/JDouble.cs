using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Message.ErrorDetail;

namespace RelogicLabs.JsonSchema.Types;

public class JDouble : JNumber, IJsonFloat, IPragmaValue<double>
{
    public required double Value { get; init; }
    public override JsonType Type => JsonType.Decimal;
    
    internal JDouble(IDictionary<JNode, JNode> relations) : base(relations) { }
    public override bool Match(JNode node)
    {
        var other = CastType<JDouble>(node);
        if(other == null) return false;
        if (IsEquivalent(Value, other.Value)) return true;
        return FailWith(new JsonSchemaException(
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
        return IsEquivalent(Value, other.Value);
    }

    public static implicit operator double(JDouble @double) => @double.Value;
    public override int GetHashCode() => Value.GetHashCode();
    protected override double ToDouble() => Convert.ToDouble(Value);
    public override string ToJson() => $"{Value:0.###############E+0}";
    public override string ToString() => ToJson();
}