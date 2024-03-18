namespace RelogicLabs.JSchema.Nodes;

public abstract class JLeaf : JNode
{
    private protected JLeaf(Builder builder) : base(builder) { }
    private protected JLeaf(JLeaf node) : base(node) { }
}