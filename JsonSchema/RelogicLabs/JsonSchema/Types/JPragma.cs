namespace RelogicLabs.JsonSchema.Types;

public class JPragma : JDirective
{
    public const string PragmaMarker = "%pragma";
    
    internal JPragma(IDictionary<JNode, JNode> relations) : base(relations) { }
    
    public required string Name { get; init; }
    public required JPrimitive Value { get; init; }
    public override IEnumerable<JNode> Children 
        => new List<JNode> { Value }.AsReadOnly();

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        if(obj.GetType() != this.GetType()) return false;
        JPragma other = (JPragma) obj;
        return Name == other.Name && Value.Equals(other.Value);
    }

    public override int GetHashCode() => HashCode.Combine(Name, Value);
    internal override JPragma Initialize() => (JPragma) base.Initialize();
    public override string ToJson() => $"{PragmaMarker} {Name}: {Value}";
    public override string ToString() => ToJson();
}