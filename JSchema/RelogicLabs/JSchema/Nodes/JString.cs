using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Message;
using RelogicLabs.JSchema.Types;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.ErrorDetail;
using static RelogicLabs.JSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JSchema.Nodes;

public class JString : JPrimitive, IEString, IDerived, IPragmaValue<string>
{
    public string Value { get; }
    public JNode? Derived { get; set; }
    public virtual EType Type => EType.STRING;

    private JString(Builder builder) : base(builder)
        => Value = RequireNonNull(builder.Value);

    private protected JString(JString node) : base(node)
        => Value = RequireNonNull(node.Value);

    public override bool Match(JNode node)
    {
        var other = CastType<JString>(node);
        if(other == null) return false;
        if(Value.Equals(other.Value)) return true;
        return Fail(new JsonSchemaException(
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

    public override int GetHashCode() => Value.GetHashCode();
    public static implicit operator string(JString node) => node.Value;
    public override string ToString() => Value.Quote();

    internal new sealed class Builder : JPrimitive.Builder<string>
    {
        public override JString Build() => Build(new JString(this));
    }
}