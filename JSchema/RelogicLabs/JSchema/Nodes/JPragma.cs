using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Tree;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.MessageFormatter;
using static RelogicLabs.JSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JSchema.Nodes;

public sealed class JPragma : JDirective
{
    internal const string PragmaMarker = "%pragma";

    public string Name { get; }
    public JPrimitive Value { get; }

    private JPragma(Builder builder) : base(builder)
    {
        Name = RequireNonNull(builder.Name);
        Value = RequireNonNull(builder.Value);
        Children = AsList(Value);
    }

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        if(obj.GetType() != this.GetType()) return false;
        JPragma other = (JPragma) obj;
        return Name == other.Name && Value.Equals(other.Value);
    }

    public override int GetHashCode() => HashCode.Combine(Name, Value);
    public override string ToString() => $"{PragmaMarker} {Name}: {Value}";

    internal new sealed class Builder : JNode.Builder
    {
        public string? Name { get; init; }
        public JPrimitive? Value { get; init; }

        private void CheckPragma()
        {
            var name = RequireNonNull(Name);
            var value = RequireNonNull(Value);
            var descriptor = PragmaDescriptor.From(name);
            if(descriptor == null) throw new PragmaNotFoundException(FormatForSchema(PRAG01,
                $"Invalid pragma '{name}' with value {value.GetOutline()} found", Context));
            if(!descriptor.MatchType(value.GetType()))
                throw new InvalidPragmaValueException(FormatForSchema(PRAG02,
                $"Invalid value {value.GetOutline()} for pragma '{name}' found", value));
        }

        public override JPragma Build()
        {
            CheckPragma();
            return Build(new JPragma(this));
        }
    }
}