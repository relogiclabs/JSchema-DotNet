namespace RelogicLabs.JsonSchema.Types;

public class JUndefined : JPrimitive
{
    public const string UndefinedMarker = "!";
    
    internal JUndefined(IDictionary<JNode, JNode> relations) : base(relations) { }
    public override bool Match(JNode node) => true;

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        return obj.GetType() == this.GetType();
    }

    public override int GetHashCode() => base.GetHashCode();
    public override string ToJson() => UndefinedMarker;
    public override string ToString() => ToJson();
}