namespace RelogicLabs.JsonSchema.Types;

public class JUndefined : JLeaf
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

    public override int GetHashCode() => UndefinedMarker.GetHashCode();
    public override string ToString() => UndefinedMarker;
}