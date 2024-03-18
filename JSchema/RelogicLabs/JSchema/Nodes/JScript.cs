using RelogicLabs.JSchema.Engine;
using static RelogicLabs.JSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JSchema.Nodes;

public sealed class JScript : JDirective
{
    internal const string ScriptMarker = "%script";
    internal Evaluator Evaluator { get; }
    internal string Source { get; }

    private JScript(Builder builder) : base(builder) {
        Evaluator = RequireNonNull(builder.Evaluator);
        Source = RequireNonNull(builder.Source);
    }

    public override string ToString() => Source;

    internal new sealed class Builder : JNode.Builder
    {
        public Evaluator? Evaluator { get; init; }
        public string? Source { get; init; }
        public override JScript Build() => Build(new JScript(this));
    }
}