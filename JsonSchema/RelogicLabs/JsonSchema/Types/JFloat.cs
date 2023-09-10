using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Message.ErrorDetail;

namespace RelogicLabs.JsonSchema.Types;

public class JFloat : JNumber, IJsonFloat, IPragmaValue<double>
{
    public required double Value { get; init; }
    public override JsonType Type => JsonType.FLOAT;
    
    internal JFloat(IDictionary<JNode, JNode> relations) : base(relations) { }
    
    public override bool Match(JNode node)
    {
        var other = CastType<JFloat>(node);
        if(other == null) return false;
        if(!IsEquivalent(Value, other.Value)) return FailWith(
            new JsonSchemaException(
                new ErrorDetail(FLOT01, ValueMismatch),
                ExpectedDetail.AsValueMismatch(this),
                ActualDetail.AsValueMismatch(other)));
        return true;
    }

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        if(obj.GetType() != this.GetType()) return false;
        JFloat other = (JFloat) obj;
        return IsEquivalent(Value, other.Value);
    }

    public override int GetHashCode() => Value.GetHashCode();
    public static implicit operator double(JFloat @float) => @float.Value;
    protected override double ToDouble() => Convert.ToDouble(Value);
    public override string ToJson() => $"{Value:0.0##############}";
    public override string ToString() => ToJson();
}