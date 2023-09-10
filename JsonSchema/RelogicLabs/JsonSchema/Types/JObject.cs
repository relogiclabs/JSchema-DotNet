using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Message.ErrorDetail;
using IJsonProperties = RelogicLabs.JsonSchema.Utilities.IProperties<
    RelogicLabs.JsonSchema.Types.JProperty, string, RelogicLabs.JsonSchema.Types.JNode>;

namespace RelogicLabs.JsonSchema.Types;

public class JObject : JBranch, IJsonType<JObject>, IJsonComposite
{
    public JsonType Type => JsonType.OBJECT;
    public override IEnumerable<JNode> Children => Properties.Values;
    public required IJsonProperties Properties { get; init; }

    internal JObject(IDictionary<JNode, JNode> relations) : base(relations) { }
    
    public override bool Match(JNode node)
    {
        var other = CastType<JObject>(node);
        if(other == null) return false;
        var result = true;
        HashSet<string> unresolved = new(other.Properties.Keys);
        for(var i = 0; i < Properties.Count; i++)
        {
            var thisProp = Properties[i];
            var otherProp = GetAssociatedProperty(other, i);
            if(otherProp != null)
            {
                result &= thisProp.Value.Match(other.Properties[thisProp.Key].Value);
                unresolved.Remove(thisProp.Key);
                continue;
            }
            if(!((JValidator) thisProp.Value).Optional)
                return FailWith(new JsonSchemaException(
                        new ErrorDetail(PROP05, PropertyNotFound),
                        ExpectedDetail.AsPropertyNotFound(thisProp),
                        ActualDetail.AsPropertyNotFound(node, thisProp)));
        }
        if(unresolved.Count > 0 && !Runtime.IgnoreUndefinedProperties)
        {
            var property = other.Properties[unresolved.First()];
            return FailWith(new JsonSchemaException(
                new ErrorDetail(PROP06, UndefinedPropertyFound),
                ExpectedDetail.AsUndefinedProperty(this, property),
                ActualDetail.AsUndefinedProperty(property)));
        }
        return result;
    }

    private JProperty? GetAssociatedProperty(JObject other, int index)
    {
        var thisProp = Properties[index];
        JProperty? otherProp = null;
        if(!Runtime.IgnoreObjectPropertyOrder)
        {
            var atAnyProp = GetPropertyAt(other.Properties, index);
            if(atAnyProp != null && thisProp.Key == atAnyProp.Key) otherProp = atAnyProp;
            JProperty? existing = null;
            if(otherProp == null) other.Properties.TryGetValue(thisProp.Key, out existing);
            if(otherProp == null && existing != null)
                FailWith(new JsonSchemaException(
                    new ErrorDetail(PROP07, PropertyOrderMismatch),
                    ExpectedDetail.AsPropertyOrderMismatch(thisProp),
                    ActualDetail.AsPropertyOrderMismatch(atAnyProp ?? (JNode) other)));
        }
        else other.Properties.TryGetValue(thisProp.Key, out otherProp);
        return otherProp;
    }

    private JProperty? GetPropertyAt(IJsonProperties properties, int index) 
        => index >= properties.Count ? null : properties[index];

    public IList<JNode> ExtractComponents() => Properties.Select(p => p.Value)
        .ToList().AsReadOnly();

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        if(obj.GetType() != this.GetType()) return false;
        var other = (JObject) obj;
        if(Properties.Count != other.Properties.Count) return false;
        return Runtime.IgnoreObjectPropertyOrder? UnorderedEquals(other.Properties) 
            : OrderedEquals(other.Properties);
    }

    private bool UnorderedEquals(IEnumerable<JProperty> properties)
    {
        foreach(var p in properties)
            if(!p.Equals(Properties[p.Key])) return false;
        return true;
    }

    private bool OrderedEquals(IList<JProperty> properties)
    {
        for(int i = 0; i < Properties.Count; i++)
            if(!Properties[i].Equals(properties[i])) return false;
        return true;
    }

    public override int GetHashCode() => Properties.GetHashCode();
    public override string ToJson() => $"{{{Properties.ToJson().ToString(", ")}}}";
    public override string ToString() => ToJson();
}