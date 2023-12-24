namespace RelogicLabs.JsonSchema.Types;

public abstract class JComposite : JBranch, IJsonType
{
    private protected JComposite(Builder builder) : base(builder) { }
    public abstract IList<JNode> Components { get; }
    public virtual JsonType Type => JsonType.COMPOSITE;
    public JNode Node => this;
}