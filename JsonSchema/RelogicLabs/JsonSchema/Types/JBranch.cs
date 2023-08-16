namespace RelogicLabs.JsonSchema.Types;

public abstract class JBranch : JNode
{
    internal JBranch(IDictionary<JNode, JNode> relations) : base(relations) { }
}