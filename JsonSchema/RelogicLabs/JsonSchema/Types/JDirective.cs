namespace RelogicLabs.JsonSchema.Types;

public abstract class JDirective : JNode
{
    private protected JDirective(Builder builder) : base(builder) { }
    public override bool Match(JNode node) => throw new InvalidOperationException();
}