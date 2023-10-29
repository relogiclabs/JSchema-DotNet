using RelogicLabs.JsonSchema.Types;

namespace RelogicLabs.JsonSchema.Utilities;

public static class CommonUtilities
{
    public static T NonNull<T>(T? value)
        => value ?? throw new NullReferenceException("Required reference not set to an instance");

    public static T NonNull<T>(T? value) where T : struct
        => value ?? throw new NullReferenceException("Required reference not set to an instance");

    public static IList<JNode> ToList(params JNode?[] nodes)
    {
        List<JNode> list = new();
        foreach(var node in nodes) if(!ReferenceEquals(node, null)) list.Add(node);
        return list.AsReadOnly();
    }

    public static IList<JNode> ToList(params IEnumerable<JNode>?[] collection)
    {
        List<JNode> list = new();
        foreach(var nodes in collection) if(!ReferenceEquals(nodes, null)) list.AddRange(nodes);
        return list.AsReadOnly();
    }
}