namespace RelogicLabs.JSchema.Nodes;

public abstract class JComposite : JBranch, IJsonType
{
    private protected JComposite(Builder builder) : base(builder) { }
    public abstract IList<JNode> Components { get; }
    public JNode Node => this;
}