using RelogicLabs.JsonSchema.Collections;
using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Message.ErrorDetail;
using static RelogicLabs.JsonSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JsonSchema.Types;

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
        if(!Key.Equals(other.Key)) return FailWith(
            new JsonSchemaException(
                new ErrorDetail(PROP01, PropertyKeyMismatch),
                ExpectedDetail.AsValueMismatch(this),
                ActualDetail.AsValueMismatch(other)));
        if(!Value.Match(other.Value)) return FailWith(
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
    public string GetKey() => Key;
    public string GetPropertyKey() => Key;
    public JNode GetPropertyValue() => Value;

    internal static IEnumerable<JProperty> CheckForDuplicate(IList<JProperty> properties, string errorCode)
    {
        var group = properties.GroupBy(p => p.Key).FirstOrDefault(g => g.Count() > 1);
        if(group == default) return properties;
        JProperty property = group.First();
        throw new DuplicatePropertyKeyException(MessageFormatter.FormatForJson(
            errorCode, $"Multiple key with name {property.Key.Quote()} found",
            property.Context));
    }

    internal new class Builder : JNode.Builder
    {
        public string? Key { get; init; }
        public JNode? Value { get; init; }
        public override JProperty Build() => Build(new JProperty(this));
    }
}