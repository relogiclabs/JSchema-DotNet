namespace RelogicLabs.JsonSchema.Types;

public class JVersion : JDirective
{
    public const string VersionMarker = "%version";
    public required string Version { get; init; }
    internal JVersion(IDictionary<JNode, JNode> relations) : base(relations) { }
    public override string ToString() => $"{VersionMarker} {Version}";
}