using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JsonSchema.Types;

public sealed class JPragma : JDirective
{
    public const string PragmaMarker = "%pragma";

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

    internal new class Builder : JNode.Builder
    {
        public string? Name { get; init; }
        public JPrimitive? Value { get; init; }

        private void CheckPragma()
        {
            var name = RequireNonNull(Name);
            var value = RequireNonNull(Value);
            var descriptor = PragmaDescriptor.From(name);
            if(descriptor == null) throw new PragmaNotFoundException(MessageFormatter
                .FormatForSchema(PRAG01, $"Invalid pragma {name.Quote()} with value {
                    value.GetOutline()} found", Context));
            if(!descriptor.MatchType(value.GetType()))
                throw new InvalidPragmaValueException(MessageFormatter.FormatForSchema(
                    PRAG02, $"Invalid value {value.GetOutline()} for pragma {name.Quote()} found",
                    value));
        }

        public override JPragma Build()
        {
            CheckPragma();
            return Build(new JPragma(this));
        }
    }
}