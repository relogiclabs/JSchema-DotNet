using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Message.ErrorDetail;

namespace RelogicLabs.JsonSchema.Types;

public class JString : JPrimitive, IPragmaValue<string>
{
    public required string Value { get; init; }

    internal JString(IDictionary<JNode, JNode> relations) : base(relations) { }
    public override bool Match(JNode node)
    {
        var other = CastType<JString>(node);
        if(other == null) return false;
        if(Value.Equals(other.Value)) return true;
        return FailWith(new JsonSchemaException(
            new ErrorDetail(STRN01, ValueMismatch),
            ExpectedDetail.AsValueMismatch(this),
            ActualDetail.AsValueMismatch(other)));
    }

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        if(obj.GetType() != this.GetType()) return false;
        JString other = (JString) obj;
        return Value == other.Value;
    }

    public override JsonType Type => JsonType.STRING;
    public override int GetHashCode() => Value.GetHashCode();
    public static implicit operator string(JString @string) => @string.Value;
    public override string ToString() => Value.Quote();
}