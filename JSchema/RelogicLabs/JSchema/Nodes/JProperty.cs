using RelogicLabs.JSchema.Collections;
using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Message;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.ErrorDetail;
using static RelogicLabs.JSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JSchema.Nodes;

public sealed class JProperty : JBranch, IKeyed<string>
{
    public string Key { get; }
    public JNode Value { get; }

    private JProperty(Builder builder) : base(builder)
    {
        Key = RequireNonNull(builder.Key);
        Value = RequireNonNull(builder.Value);
        Children = AsList(Value);
    }

    public override bool Match(JNode node)
    {
        var other = CastType<JProperty>(node);
        if(other == null) return false;
        if(!Key.Equals(other.Key)) return Fail(
            new JsonSchemaException(
                new ErrorDetail(PROP01, PropertyKeyMismatch),
                ExpectedDetail.AsValueMismatch(this),
                ActualDetail.AsValueMismatch(other)));
        if(!Value.Match(other.Value)) return Fail(
            new JsonSchemaException(
                new ErrorDetail(PROP02, PropertyValueMismatch),
                ExpectedDetail.AsValueMismatch(this),
                ActualDetail.AsValueMismatch(other)));
        return true;
    }

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        if(obj.GetType() != this.GetType()) return false;
        JProperty other = (JProperty) obj;
        return Key == other.Key && Value.Equals(other.Value);
    }

    public override int GetHashCode() => HashCode.Combine(Key, Value);
    public override string ToString() => $"{Key.Quote()}: {Value}";

    internal new sealed class Builder : JNode.Builder
    {
        public string? Key { get; init; }
        public JNode? Value { get; init; }
        public override JProperty Build() => Build(new JProperty(this));
    }
}