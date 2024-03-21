using RelogicLabs.JSchema.Exceptions;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.MessageFormatter;
using static RelogicLabs.JSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JSchema.Nodes;

public sealed class JAlias : JLeaf
{
    public string Name { get; }

    private JAlias(Builder builder) : base(builder)
        => Name = RequireNonNull(builder.Name);

    public override bool Match(JNode node)
    {
        if(!Runtime.Definitions.ContainsKey(this))
            throw new DefinitionNotFoundException(FormatForSchema(DEFI02,
                $"Definition of '{Name}' not found", this));
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
    public override string ToString() => Name;

    internal new sealed class Builder : JNode.Builder
    {
        public string? Name { get; init; }
        public override JAlias Build() => Build(new JAlias(this));
    }
}