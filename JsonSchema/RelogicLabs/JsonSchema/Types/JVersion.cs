using static RelogicLabs.JsonSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JsonSchema.Types;

public sealed class JVersion : JDirective
{
    internal const string VersionMarker = "%version";
    public string Version { get; }

    private JVersion(Builder builder) : base(builder)
        => Version = RequireNonNull(builder.Version);

    public override string ToString() => $"{VersionMarker}: {Version}";

    internal new class Builder : JNode.Builder
    {
        public string? Version { get; init; }
        public override JVersion Build() => Build(new JVersion(this));
    }
}