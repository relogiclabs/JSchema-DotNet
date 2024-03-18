using System.Text;
using RelogicLabs.JSchema.Utilities;
using static System.Environment;
using static RelogicLabs.JSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JSchema.Nodes;

public sealed class JRoot : JNode
{
    public JTitle? Title { get; }
    public JVersion? Version { get; }
    public IList<JImport>? Imports { get; }
    public IList<JPragma>? Pragmas { get; }
    public IList<JDefinition>? Definitions { get; }
    public IList<JScript>? Scripts { get; }
    public JNode Value { get; }

    public override JNode Parent => null!;

    private JRoot(Builder builder) : base(builder)
    {
        Title = builder.Title;
        Version = builder.Version;
        Imports = builder.Imports;
        Pragmas = builder.Pragmas;
        Definitions = builder.Definitions;
        Scripts = builder.Scripts;
        Value = RequireNonNull(builder.Value);
        Children = new List<JNode>().AddToList(Title, Version)
            .AddToList(Imports, Pragmas, Definitions)
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
        AppendTo(builder, Imports?.Join(NewLine));
        AppendTo(builder, Pragmas?.Join(NewLine));
        AppendTo(builder, Definitions?.Join(NewLine));
        AppendTo(builder, Value.ToString());
        return builder.ToString().Trim();
    }

    private static void AppendTo(StringBuilder builder, string? text)
    {
        if(text is null || text.Length == 0) return;
        builder.Append(text).Append(NewLine);
    }

    internal new sealed class Builder : JNode.Builder
    {
        public JTitle? Title { get; init; }
        public JVersion? Version { get; init; }
        public IList<JImport>? Imports { get; init; }
        public IList<JPragma>? Pragmas { get; init; }
        public IList<JDefinition>? Definitions { get; init; }
        public IList<JScript>? Scripts { get; init; }
        public JNode? Value { get; init; }
        public override JRoot Build() => Build(new JRoot(this));
    }
}