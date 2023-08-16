namespace RelogicLabs.JsonSchema.Types;

public class JTitle : JDirective
{
    public const string TitleMarker = "%title";
    public required string Title { get; init; }
    internal JTitle(IDictionary<JNode, JNode> relations) : base(relations) { }

    public override string ToJson() => $"{TitleMarker} {Title}";
}