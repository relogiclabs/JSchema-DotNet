using System.Diagnostics.CodeAnalysis;
using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Types;

namespace RelogicLabs.JsonSchema.Message;

public abstract class CommonDetail<T> where T : CommonDetail<T>
{
    public required Context Context { get; init; }
    public required string Message { get; init; }
    public required Location Location { get; init; }

    [SetsRequiredMembers]
    protected CommonDetail(Context context, string message)
    {
        Context = context;
        Message = message;
        Location = context.GetLocation();
    }
    
    [SetsRequiredMembers]
    protected CommonDetail(JNode node, string message)
        : this(node.Context, message) { }

    private static T From(Context context, string message)
    {
        T? instance = null;
        if(typeof(T) == typeof(ActualDetail))
            instance = new ActualDetail(context, message) as T;
        if(typeof(T) == typeof(ExpectedDetail))
            instance = new ExpectedDetail(context, message) as T;
        if(instance == null) throw new InvalidOperationException();
        return instance;
    }

    internal static T AsDataTypeMismatch(JNode node)
    {
        string name = node is IJsonType<JNode> _node? _node.Type.Name : node.GetType().Name;
        string message = $"found {name} inferred by {node.ToOutline()}";
        return From(node.Context, message);
    }
}