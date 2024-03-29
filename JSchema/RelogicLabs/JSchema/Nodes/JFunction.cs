using System.Reflection;
using System.Text;
using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Message;
using RelogicLabs.JSchema.Tree;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.ErrorDetail;
using static RelogicLabs.JSchema.Nodes.INestedMode;
using static RelogicLabs.JSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JSchema.Nodes;

public sealed class JFunction : JBranch, INestedMode
{
    public string Name { get; }
    public bool Nested { get; }
    public IList<JNode> Arguments { get; }
    internal FunctionCache Cache { get; }

    private JFunction(Builder builder) : base(builder)
    {
        Name = RequireNonNull(builder.Name);
        Nested = RequireNonNull(builder.Nested);
        Arguments = RequireNonNull(builder.Arguments);
        Children = Arguments;
        Cache = new FunctionCache();
    }

    public override bool Match(JNode node)
    {
        if(!Nested) return InvokeFunction(node);
        if(node is not JComposite composite) return Fail(
            new JsonSchemaException(
                new ErrorDetail(FUNC06, InvalidNestedFunction),
                ExpectedDetail.AsInvalidFunction(this),
                ActualDetail.AsInvalidFunction(node)));
        return composite.Components.ForEachTrue(InvokeFunction);
    }

    private bool InvokeFunction(JNode node)
    {
        try
        {
            return Runtime.Functions.InvokeFunction(this, node);
        }
        catch(TargetInvocationException ex)
        {
            if(ex.InnerException == null) throw;
            throw ex.InnerException;
        }
        catch { throw; }
    }

    internal bool IsApplicable(JNode node) => !Nested || node is JComposite;
    public override string ToString() => ToString(false);
    public string ToString(bool baseForm)
    {
        StringBuilder builder = new(Name);
        if(Nested && !baseForm) builder.Append(NestedMarker);
        builder.Append(Arguments.Join(", ", "(", ")"));
        return builder.ToString();
    }

    internal new sealed class Builder : JNode.Builder
    {
        public string? Name { get; init; }
        public bool? Nested { get; init; }
        public IList<JNode>? Arguments { get; init; }
        public override JFunction Build() => Build(new JFunction(this));
    }
}