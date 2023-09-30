using System.Diagnostics.CodeAnalysis;
using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Types;
using RelogicLabs.JsonSchema.Utilities;

namespace RelogicLabs.JsonSchema.Message;

public class ActualDetail : ContextDetail
{
    [SetsRequiredMembers]
    public ActualDetail(Context context, string message) 
        : base(context, message) { }
    
    [SetsRequiredMembers]
    public ActualDetail(JNode node, string message) 
        : base(node, message) { }

    internal static ActualDetail AsValueMismatch(JNode node)
        => new(node, node.GetOutline().Affix("found "));
    
    internal static ActualDetail AsPropertyNotFound(JNode node, JProperty property) 
        => new(node, $"not found property key \"{property.Key}\"");
    
    internal static ActualDetail AsUndefinedProperty(JProperty property)
        => new(property, $"property found {{{property.GetOutline()}}}");
    
    internal static ActualDetail AsDataTypeMismatch(JNode node) 
        => new(node, $"found {GetTypeName(node)} inferred by {node.GetOutline()}");

    internal static ActualDetail AsArrayElementNotFound(JArray array, int index)
        => new(array, "not found");
    
    internal static ActualDetail AsInvalidFunction(JNode node) 
        => new(node, $"applied on non-composite type {GetTypeName(node)}");
    
    internal static ActualDetail AsInvalidDataType(JNode node) 
        => new(node, $"applied on non-composite type {GetTypeName(node)}");

    internal static ActualDetail AsPropertyOrderMismatch(JNode node)
        => node is JProperty property
            ? new(property, $"key \"{property.Key}\" is found at position")
            : new(node, "key not found at position");
}