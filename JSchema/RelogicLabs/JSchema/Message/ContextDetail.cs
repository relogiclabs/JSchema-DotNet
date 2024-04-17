using RelogicLabs.JSchema.Nodes;
using RelogicLabs.JSchema.Tree;

namespace RelogicLabs.JSchema.Message;

public abstract class ContextDetail
{
    public Context Context { get; }
    public string Message { get; }
    public Location Location { get; }

    protected ContextDetail(Context context, string message)
    {
        Context = context;
        Message = message;
        Location = context.GetLocation();
    }

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
        var t = JsonType.GetType(type);
        return t?.Name ?? type.Name;
    }
}