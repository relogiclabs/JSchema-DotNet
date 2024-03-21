using static RelogicLabs.JSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JSchema.Nodes;

public sealed class JDefinition : JDirective
{
    internal const string DefineMarker = "%define";
    public JAlias Alias { get; }
    public JValidator Validator { get; }

    private JDefinition(Builder builder) : base(builder)
    {
        Alias = RequireNonNull(builder.Alias);
        Validator = RequireNonNull(builder.Validator);
        Children = AsList<JNode>(Alias, Validator);
    }

    public override string ToString() => $"{DefineMarker} {Alias}: {Validator}";

    internal new sealed class Builder : JNode.Builder
    {
        public JAlias? Alias { get; init; }
        public JValidator? Validator { get; init; }
        public override JDefinition Build() => Build(new JDefinition(this));
    }
}