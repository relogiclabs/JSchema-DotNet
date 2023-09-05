using System.Diagnostics.CodeAnalysis;
using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Types;
using RelogicLabs.JsonSchema.Utilities;

namespace RelogicLabs.JsonSchema.Message;

public class ActualDetail : CommonDetail<ActualDetail>
{
    [SetsRequiredMembers]
    public ActualDetail(Context context, string message) 
        : base(context, message) { }
    
    [SetsRequiredMembers]
    public ActualDetail(JNode node, string message) 
        : base(node, message) { }

    internal static ActualDetail AsValueMismatch(JNode node)
        => new(node.Context, node.ToOutline().ToString("found "));
    
    internal static ActualDetail AsPropertyNotFound(JNode node, JProperty property) 
        => new(node.Context, $"not found property key \"{property.Key}\"");
    
    internal static ActualDetail AsUndefinedProperty(JProperty property)
        => new(property.Context, $"property found {{{property.ToOutline()}}}");
    
    internal static ActualDetail AsArrayElementNotFound(JArray array, int index)
        => new(array.Context, $"not found");
    
    internal static ActualDetail AsInvalidFunction(JNode node) 
        => new(node, $"applied on non-composite type {GetType(node)}");
    
    internal static ActualDetail AsInvalidDataType(JNode node) 
        => new(node, $"applied on non-composite type {GetType(node)}");

    internal static ActualDetail AsPropertyOrderMismatch(JNode node)
        => node is JProperty property
            ? new(property, $"key \"{property.Key}\" is found at position")
            : new(node, "key not found at position");

    private static JsonType GetType(JNode node) => ((IJsonType<JNode>) node).Type;
}