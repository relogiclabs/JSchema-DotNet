namespace RelogicLabs.JsonSchema.Types;

public class JDefinition : JDirective
{
    public const string DefineMarker = "%define";
    public required JAlias Alias { get; init; }
    public required JValidator Validator { get; init; }
    public override IEnumerable<JNode> Children 
        => new List<JNode> { Alias, Validator }.AsReadOnly();

    internal JDefinition(IDictionary<JNode, JNode> relations) : base(relations) { }
    internal override JDefinition Initialize() => (JDefinition) base.Initialize();
    public override string ToString() => $"{DefineMarker} {Alias} {Validator}";
}