using System.Diagnostics.CodeAnalysis;
using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Types;
using RelogicLabs.JsonSchema.Utilities;

namespace RelogicLabs.JsonSchema.Message;

public class ExpectedDetail : ContextDetail
{
    [SetsRequiredMembers]
    public ExpectedDetail(Context context, string message) 
        : base(context, message) { }

    [SetsRequiredMembers]
    public ExpectedDetail(JNode node, string message) 
        : base(node, message) { }

    internal static ExpectedDetail AsArrayElementNotFound(JNode node, int index) 
        => new(node, $"element at {index} ({node.GetOutline()})");
    
    internal static ExpectedDetail AsValueMismatch(JNode node)
        => new(node, $"value {node.GetOutline()}");
    
    internal static ExpectedDetail AsUndefinedProperty(JObject @object, JProperty property)
        => new(@object, $"no property with key {property.Key.Quote()}");
    
    internal static ExpectedDetail AsPropertyNotFound(JProperty property) 
        => new(property, $"property ({property.GetOutline()})");

    internal static ExpectedDetail AsDataTypeMismatch(JNode node) 
        => new(node, $"{GetTypeName(node)} inferred by {node.GetOutline()}");
    
    internal static ExpectedDetail AsDataTypeMismatch(JDataType dataType) 
        => new(dataType, $"data type {dataType.ToString(true)}");

    internal static ExpectedDetail AsInvalidFunction(JFunction function) 
        => new(function, "applying on composite type");

    internal static ExpectedDetail AsInvalidDataType(JDataType dataType) 
        => new(dataType, "applying on composite type");

    internal static ExpectedDetail AsPropertyOrderMismatch(JProperty property) 
        => new(property, $"property with key {property.Key.Quote()} at position");
}