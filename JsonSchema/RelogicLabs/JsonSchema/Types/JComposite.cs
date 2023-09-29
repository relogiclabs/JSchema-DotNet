namespace RelogicLabs.JsonSchema.Types;

public abstract class JComposite : JBranch, IJsonType
{
    protected JComposite(IDictionary<JNode, JNode> relations) : base(relations) { }
    public abstract IList<JNode> GetComponents();
    public virtual JsonType Type => JsonType.ANY;
    public JNode Node => this;
}