using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Message.ErrorDetail;
using static RelogicLabs.JsonSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JsonSchema.Types;

public sealed class JBoolean : JPrimitive, IPragmaValue<bool>
{
    public bool Value { get; }

    private JBoolean(Builder builder) : base(builder)
        => Value = RequireNonNull(builder.Value);

    public override bool Match(JNode node)
    {
        var other = CastType<JBoolean>(node);
        if(other == null) return false;
        if(Value == other.Value) return true;
        return FailWith(new JsonSchemaException(
            new ErrorDetail(BOOL01, ValueMismatch),
            ExpectedDetail.AsValueMismatch(this),
            ActualDetail.AsValueMismatch(other)));
    }

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        if(obj.GetType() != this.GetType()) return false;
        JBoolean other = (JBoolean) obj;
        return Value == other.Value;
    }

    public override JsonType Type => JsonType.BOOLEAN;
    public override int GetHashCode() => Value.GetHashCode();
    public static implicit operator bool(JBoolean @bool) => @bool.Value;
    public override string ToString() => Value.ToString().ToLower();

    internal new class Builder : JPrimitive.Builder<bool>
    {
        public override JBoolean Build() => Build(new JBoolean(this));
    }
}