using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Types;

namespace RelogicLabs.JsonSchema.Message;

public sealed class ExpectedDetail : ContextDetail
{
    public ExpectedDetail(Context context, string message)
        : base(context, message) { }

    public ExpectedDetail(JNode node, string message)
        : base(node, message) { }

    internal static ExpectedDetail AsArrayElementNotFound(JNode node, int index)
        => new(node, $"'{node.GetOutline()}' element at {index}");

    internal static ExpectedDetail AsValueMismatch(JNode node)
        => new(node, $"value {node.GetOutline()}");

    internal static ExpectedDetail AsUndefinedProperty(JObject @object, JProperty property)
        => new(@object, $"no property with key '{property.Key}'");

    internal static ExpectedDetail AsPropertyNotFound(JProperty property)
        => new(property, $"property {{{property.GetOutline()}}}");

    internal static ExpectedDetail AsDataTypeMismatch(JNode node)
        => new(node, $"{GetTypeName(node)} inferred by {node.GetOutline()}");

    internal static ExpectedDetail AsDataTypeMismatch(JDataType dataType)
        => new(dataType, $"data type {dataType.ToString(true)}");

    internal static ExpectedDetail AsInvalidFunction(JFunction function)
        => new(function, "applying on composite type");

    internal static ExpectedDetail AsInvalidNestedDataType(JDataType dataType)
        => new(dataType, "composite data type");

    internal static ExpectedDetail AsDataTypeArgumentFailed(JDataType dataType)
        => new(dataType, $"a valid value for '{dataType.Alias}'");

    internal static ExpectedDetail AsPropertyOrderMismatch(JProperty property)
        => new(property, $"property with key '{property.Key}' at current position");
}