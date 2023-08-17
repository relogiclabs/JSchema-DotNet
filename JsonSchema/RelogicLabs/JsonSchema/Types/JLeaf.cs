namespace RelogicLabs.JsonSchema.Types;

public abstract class JLeaf : JNode
{
    public override IEnumerable<JNode> Children => Enumerable.Empty<JNode>();
    internal JLeaf(IDictionary<JNode, JNode> relations) : base(relations) { }
}