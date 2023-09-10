namespace RelogicLabs.JsonSchema.Types;

public class JNull : JPrimitive, IJsonType<JNull>
{
    public const string NullMarker = "null";
    
    public JsonType Type => JsonType.NULL;
    internal JNull(IDictionary<JNode, JNode> relations) : base(relations) { }
    public override bool Match(JNode node) => CheckType<JNull>(node);

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        return obj.GetType() == this.GetType();
    }

    public override int GetHashCode() => NullMarker.GetHashCode();
    public override string ToJson() => NullMarker;
    public override string ToString() => ToJson();
}