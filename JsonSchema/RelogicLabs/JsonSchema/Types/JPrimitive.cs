namespace RelogicLabs.JsonSchema.Types;

public abstract class JPrimitive : JLeaf
{
    internal JPrimitive(IDictionary<JNode, JNode> relations) : base(relations) { }
}