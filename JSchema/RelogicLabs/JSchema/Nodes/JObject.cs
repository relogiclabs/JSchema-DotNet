using RelogicLabs.JSchema.Collections;
using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Message;
using RelogicLabs.JSchema.Types;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.ErrorDetail;
using static RelogicLabs.JSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JSchema.Nodes;

public sealed class JObject : JComposite, IEObject
{
    public IIndexMap<string, JProperty> Properties { get; }
    public override IList<JNode> Components { get; }
    public int Count => Properties.Count;
    public IEnumerable<string> Keys => Properties.Keys;

    private JObject(Builder builder) : base(builder)
    {
        Properties = RequireNonNull(builder.Properties);
        Components = Properties.Select(static p => p.Value).ToList().AsReadOnly();
        Children = Properties.Values;
    }

    public override bool Match(JNode node)
    {
        var other = CastType<JObject>(node);
        if(other == null) return false;
        var result = true;
        HashSet<string> unresolved = new(other.Properties.Keys);
        for(var i = 0; i < Properties.Count; i++)
        {
            var thisProp = Properties[i];
            var otherProp = GetOtherProp(other, i);
            if(otherProp != null)
            {
                result &= thisProp.Value.Match(other.Properties[thisProp.Key].Value);
                unresolved.Remove(thisProp.Key);
                continue;
            }
            if(!((JValidator) thisProp.Value).Optional)
                return Fail(new JsonSchemaException(
                        new ErrorDetail(PROP05, PropertyNotFound),
                        ExpectedDetail.AsPropertyNotFound(thisProp),
                        ActualDetail.AsPropertyNotFound(node, thisProp)));
        }
        if(unresolved.IsEmpty() || Runtime.Pragmas.IgnoreUndefinedProperties) return result;
        foreach(var key in unresolved)
        {
            var property = other.Properties[key];
            result &= Fail(new JsonSchemaException(
                new ErrorDetail(PROP06, UndefinedPropertyFound),
                ExpectedDetail.AsUndefinedProperty(this, property),
                ActualDetail.AsUndefinedProperty(property)));
        }
        return result;
    }

    private JProperty? GetOtherProp(JObject other, int index)
    {
        var thisProp = Properties[index];
        JProperty? otherProp = null;
        if(!Runtime.Pragmas.IgnoreObjectPropertyOrder)
        {
            var atProp = GetPropAt(other.Properties, index);
            if(AreKeysEqual(atProp, thisProp)) otherProp = atProp;
            JProperty? existing = null;
            if(otherProp == null) other.Properties.TryGetValue(thisProp.Key, out existing);
            if(otherProp == null && existing != null)
                Fail(new JsonSchemaException(
                    new ErrorDetail(PROP07, PropertyOrderMismatch),
                    ExpectedDetail.AsPropertyOrderMismatch(thisProp),
                    ActualDetail.AsPropertyOrderMismatch(atProp ?? (JNode) other)));
        }
        else other.Properties.TryGetValue(thisProp.Key, out otherProp);
        return otherProp;
    }

    private static bool AreKeysEqual(JProperty? p1, JProperty? p2) {
        if(p1 == null || p2 == null) return false;
        return p1.Key == p2.Key;
    }

    private static JProperty? GetPropAt(IIndexMap<string, JProperty> properties, int index)
        => index >= properties.Count ? null : properties[index];

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        if(obj.GetType() != this.GetType()) return false;
        var other = (JObject) obj;
        if(Properties.Count != other.Properties.Count) return false;
        return Runtime.Pragmas.IgnoreObjectPropertyOrder? UnorderedEquals(other.Properties)
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

    public IEValue? Get(string key)
    {
        Properties.TryGetValue(key, out var property);
        return property?.Value;
    }

    public void Set(string key, IEValue value)
        => throw new UpdateNotSupportedException(OUPD01, "Readonly object cannot be updated");
    public override int GetHashCode() => Properties.GetHashCode();
    public override string ToString() => Properties.JoinWith(", ", "{", "}");

    internal new sealed class Builder : JNode.Builder
    {
        public IIndexMap<string, JProperty>? Properties { get; init; }
        public override JObject Build() => Build(new JObject(this));
    }
}