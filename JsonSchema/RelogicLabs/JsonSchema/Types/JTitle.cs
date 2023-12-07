using static RelogicLabs.JsonSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JsonSchema.Types;

public sealed class JTitle : JDirective
{
    public const string TitleMarker = "%title";
    public string Title { get; }

    private JTitle(Builder builder) : base(builder)
        => Title = RequireNonNull(builder.Title);

    public override string ToString() => $"{TitleMarker}: {Title}";

    internal new class Builder : JNode.Builder
    {
        public string? Title { get; init; }
        public override JTitle Build() => Build(new JTitle(this));
    }
}