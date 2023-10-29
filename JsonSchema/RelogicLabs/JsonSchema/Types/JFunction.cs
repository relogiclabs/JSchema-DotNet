using System.Reflection;
using System.Text;
using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Message.ErrorDetail;
using static RelogicLabs.JsonSchema.Types.INestedMode;
using static RelogicLabs.JsonSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JsonSchema.Types;

public sealed class JFunction : JBranch, INestedMode
{
    public string Name { get; }
    public bool Nested { get; }
    public IList<JNode> Arguments { get; }

    private JFunction(Builder builder) : base(builder)
    {
        Name = NonNull(builder.Name);
        Nested = NonNull(builder.Nested);
        Arguments = NonNull(builder.Arguments);
        Children = Arguments;
    }

    public override bool Match(JNode node)
    {
        if(!Nested) return InvokeFunction(node);
        if(node is not JComposite composite) return FailWith(
            new JsonSchemaException(
                new ErrorDetail(FUNC06, InvalidNestedFunction),
                ExpectedDetail.AsInvalidFunction(this),
                ActualDetail.AsInvalidFunction(node)));
        IList<JNode> components = composite.GetComponents();
        return components.Select(InvokeFunction).ForEachTrue();
    }

    private bool InvokeFunction(JNode node)
    {
        try
        {
            return Runtime.InvokeFunction(this, node);
        }
        catch(Exception ex)
        {
            var cause = ex is TargetInvocationException? ex.InnerException ?? ex : ex;
            if(cause is JsonSchemaException) throw cause;
            throw;
        }
    }

    public bool IsApplicable(JNode node) => !Nested || node is JComposite;
    public override string ToString() => ToString(false);
    public string ToString(bool baseForm)
    {
        StringBuilder builder = new(Name);
        if(Nested && !baseForm) builder.Append(NestedMarker);
        builder.Append(Arguments.ToString(", ", "(", ")"));
        return builder.ToString();
    }

    internal new class Builder : JNode.Builder
    {
        public string? Name { get; init; }
        public bool? Nested { get; init; }
        public IList<JNode>? Arguments { get; init; }
        public override JFunction Build() => Build(new JFunction(this));
    }
}