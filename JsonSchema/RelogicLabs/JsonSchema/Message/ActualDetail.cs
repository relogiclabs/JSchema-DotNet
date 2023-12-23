using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Types;

namespace RelogicLabs.JsonSchema.Message;

public sealed class ActualDetail : ContextDetail
{
    public ActualDetail(Context context, string message)
        : base(context, message) { }

    public ActualDetail(JNode node, string message)
        : base(node, message) { }

    internal static ActualDetail AsValueMismatch(JNode node)
        => new(node, $"found {node.GetOutline()}");

    internal static ActualDetail AsGeneralValueMismatch(JNode node)
        => new(node, $"found {node.GetOutline()}");

    internal static ActualDetail AsPropertyNotFound(JNode node, JProperty property)
        => new(node, $"not found property key '{property.Key}'");

    internal static ActualDetail AsUndefinedProperty(JProperty property)
        => new(property, $"property found {{{property.GetOutline()}}}");

    internal static ActualDetail AsDataTypeMismatch(JNode node)
        => new(node, $"found {GetTypeName(node)} inferred by {node.GetOutline()}");

    internal static ActualDetail AsArrayElementNotFound(JArray array, int index)
        => new(array, "not found");

    internal static ActualDetail AsInvalidFunction(JNode node)
        => new(node, $"applied on non-composite type {GetTypeName(node)}");

    internal static ActualDetail AsInvalidNonCompositeType(JNode node)
        => new(node, $"found non-composite {GetTypeName(node)} value {node.GetOutline()}");

    internal static ActualDetail AsDataTypeArgumentFailed(JNode node)
        => new(node, $"found invalid value {node.GetOutline()}");

    internal static ActualDetail AsPropertyOrderMismatch(JNode node)
        => node is JProperty property
            ? new(property, $"key '{property.Key}' is found at current position")
            : new(node, "key not found at current position");
}