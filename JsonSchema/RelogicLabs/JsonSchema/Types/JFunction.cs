using System.Reflection;
using System.Text;
using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Message.ErrorDetail;
using static RelogicLabs.JsonSchema.Types.INestedMode;

namespace RelogicLabs.JsonSchema.Types;

public class JFunction : JBranch, INestedMode
{
    public required string Name { get; init; }
    public required IList<JNode> Arguments { get; init; }
    public override IEnumerable<JNode> Children => Arguments;
    public required bool Nested { get; init; }

    internal JFunction(IDictionary<JNode, JNode> relations) : base(relations) { }
    
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
}