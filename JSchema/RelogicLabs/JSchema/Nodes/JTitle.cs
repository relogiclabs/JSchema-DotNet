using static RelogicLabs.JSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JSchema.Nodes;

public sealed class JTitle : JDirective
{
    internal const string TitleMarker = "%title";
    public string Title { get; }

    private JTitle(Builder builder) : base(builder)
        => Title = RequireNonNull(builder.Title);

    public override string ToString() => $"{TitleMarker}: {Title}";

    internal new sealed class Builder : JNode.Builder
    {
        public string? Title { get; init; }
        public override JTitle Build() => Build(new JTitle(this));
    }
}