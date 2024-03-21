using static RelogicLabs.JSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JSchema.Nodes;

public sealed class JVersion : JDirective
{
    internal const string VersionMarker = "%version";
    public string Version { get; }

    private JVersion(Builder builder) : base(builder)
        => Version = RequireNonNull(builder.Version);

    public override string ToString() => $"{VersionMarker}: {Version}";

    internal new sealed class Builder : JNode.Builder
    {
        public string? Version { get; init; }
        public override JVersion Build() => Build(new JVersion(this));
    }
}