using System.Diagnostics.CodeAnalysis;
using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Types;

namespace RelogicLabs.JsonSchema.Message;

public abstract class ContextDetail
{
    public required Context Context { get; init; }
    public required string Message { get; init; }
    public required Location Location { get; init; }

    [SetsRequiredMembers]
    protected ContextDetail(Context context, string message)
    {
        Context = context;
        Message = message;
        Location = context.GetLocation();
    }

    [SetsRequiredMembers]
    protected ContextDetail(JNode node, string message)
        : this(node.Context, message) { }

    internal static string GetTypeName(JNode node)
    {
        return node is IJsonType jsonNode
            ? jsonNode.Type.Name
            : node.GetType().Name;
    }

    internal static string GetTypeName(Type type)
    {
        var jsonType = JsonType.From(type);
        return jsonType != null ? jsonType.Name : type.Name;
    }
}