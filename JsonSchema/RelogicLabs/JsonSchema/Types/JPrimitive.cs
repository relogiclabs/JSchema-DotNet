namespace RelogicLabs.JsonSchema.Types;

public abstract class JPrimitive : JLeaf, IJsonType
{
    internal JPrimitive(IDictionary<JNode, JNode> relations) : base(relations) { }
    public virtual JsonType Type => JsonType.ANY;
    public JNode Node => this;
}