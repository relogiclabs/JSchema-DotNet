using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Message.ErrorDetail;

namespace RelogicLabs.JsonSchema.Types;

public class JProperty : JBranch, IProperty<string, JNode>
{
    public required string Key { get; init; }
    public required JNode Value { get; init; }
    
    internal JProperty(IDictionary<JNode, JNode> relations) : base(relations) { }
    public override IEnumerable<JNode> Children => new[] { Value };
    
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
    public override string ToJson() => $"{Key.DoubleQuote()}: {Value.ToJson()}";
    public override string ToString() => ToJson();
    public string GetPropertyKey() => Key;
    public JNode GetPropertyValue() => Value;
}