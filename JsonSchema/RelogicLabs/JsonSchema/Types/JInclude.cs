namespace RelogicLabs.JsonSchema.Types;

public class JInclude : JDirective
{
    public const string IncludeMarker = "%include";
    public required string ClassName { get; init; }

    internal JInclude(IDictionary<JNode, JNode> relations) : base(relations) { }
    internal override JInclude Initialize() => (JInclude) base.Initialize();

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        if(obj.GetType() != this.GetType()) return false;
        JInclude other = (JInclude) obj;
        return ClassName == other.ClassName;
    }

    public override int GetHashCode() => ClassName.GetHashCode();
    public override string ToString() => $"{IncludeMarker} {ClassName}";
}