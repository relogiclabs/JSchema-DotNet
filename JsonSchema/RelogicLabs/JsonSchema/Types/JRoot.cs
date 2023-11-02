using System.Text;
using RelogicLabs.JsonSchema.Utilities;
using static System.Environment;
using static RelogicLabs.JsonSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JsonSchema.Types;

public sealed class JRoot : JNode
{
    public JTitle? Title { get; }
    public JVersion? Version { get; }
    public IList<JInclude>? Includes { get; }
    public IList<JPragma>? Pragmas { get; }
    public IList<JDefinition>? Definitions { get; }
    public JNode Value { get; }

    public override JNode Parent => null!;

    private JRoot(Builder builder) : base(builder)
    {
        Title = builder.Title;
        Version = builder.Version;
        Includes = builder.Includes;
        Pragmas = builder.Pragmas;
        Definitions = builder.Definitions;
        Value = NonNull(builder.Value);
        Children = new List<JNode>().AddToList(Title, Version)
            .AddToList(Includes, Pragmas, Definitions)
            .AddToList(Value).AsReadOnly();
    }

    public override bool Match(JNode node)
    {
        var other = CastType<JRoot>(node);
        if(other == null) return false;
        return Value.Match(other.Value);
    }

    public override string ToString()
    {
        StringBuilder builder = new();
        AppendTo(builder, Title?.ToString());
        AppendTo(builder, Version?.ToString());
        AppendTo(builder, Includes?.ToString(NewLine));
        AppendTo(builder, Pragmas?.ToString(NewLine));
        AppendTo(builder, Definitions?.ToString(NewLine));
        AppendTo(builder, Value.ToString());
        return builder.ToString().Trim();
    }

    private static void AppendTo(StringBuilder builder, string? text)
    {
        if(text is null || text.Length == 0) return;
        builder.Append(text).Append(NewLine);
    }

    internal new class Builder : JNode.Builder
    {
        public JTitle? Title { get; init; }
        public JVersion? Version { get; init; }
        public IList<JInclude>? Includes { get; init; }
        public IList<JPragma>? Pragmas { get; init; }
        public IList<JDefinition>? Definitions { get; init; }
        public JNode? Value { get; init; }
        public override JRoot Build() => Build(new JRoot(this));
    }
}