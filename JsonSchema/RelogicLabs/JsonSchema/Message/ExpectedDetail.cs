using System.Diagnostics.CodeAnalysis;
using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Types;
using RelogicLabs.JsonSchema.Utilities;

namespace RelogicLabs.JsonSchema.Message;

public class ExpectedDetail : CommonDetail<ExpectedDetail>
{
    [SetsRequiredMembers]
    public ExpectedDetail(Context context, string message) 
        : base(context, message) { }

    [SetsRequiredMembers]
    public ExpectedDetail(JNode node, string message) 
        : base(node, message) { }

    internal static ExpectedDetail AsValueMismatch(JNode node)
        => new(node.Context, node.ToOutline().ToString("value "));
    
    internal static ExpectedDetail AsPropertyNotFound(JProperty property) 
        => new(property, $"property schema ({property.ToOutline()})");

    internal static ExpectedDetail AsUndefinedProperty(JObject @object, JProperty property)
        => new(@object, $"no property with key {property.Key.DoubleQuote()}");

    internal static ExpectedDetail AsDataTypeMismatch(JDataType dataType) 
        => new(dataType, $"data type {dataType}");

    internal static ExpectedDetail AsArrayElementNotFound(JNode node, int index) 
        => new(node, $"element at {index} ({node.ToOutline()})");

    internal static ExpectedDetail AsInvalidFunction(JFunction function) 
        => new(function, "applied on composite type");

    internal static ExpectedDetail AsInvalidDataType(JDataType dataType) 
        => new(dataType, "applied on composite type");

    internal static ExpectedDetail AsPropertyOrderMismatch(JProperty property) 
        => new(property, $"property with key \"{property.Key}\" at position");
}