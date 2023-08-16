namespace RelogicLabs.JsonSchema.Types;

public abstract class JDirective : JNode
{
    public override IEnumerable<JNode> Children => Enumerable.Empty<JNode>();
    internal JDirective(IDictionary<JNode, JNode> relations) : base(relations) { }
    public override bool Match(JNode node) => throw new InvalidOperationException();
}