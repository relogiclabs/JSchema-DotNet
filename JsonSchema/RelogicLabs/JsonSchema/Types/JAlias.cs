using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Types;

public class JAlias : JLeaf
{
    public required string Name { get; init; }

    internal JAlias(IDictionary<JNode, JNode> relations) : base(relations) { }

    public override bool Match(JNode node)
    {
        if(!Runtime.Definitions.ContainsKey(this))
            throw new DefinitionNotFoundException(MessageFormatter
                .FormatForSchema(DEFI02, $"Definition of {Name} not found", Context));
        return Runtime.Definitions[this].Match(node);
    }

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        if(obj.GetType() != this.GetType()) return false;
        JAlias other = (JAlias) obj;
        return Name == other.Name;
    }

    public override int GetHashCode() => Name.GetHashCode();
    public override string ToJson() => Name;
    public override string ToString() => ToJson();
}