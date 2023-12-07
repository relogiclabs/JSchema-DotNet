using RelogicLabs.JsonSchema.Types;

namespace RelogicLabs.JsonSchema.Utilities;

internal static class CommonUtilities
{
    internal static T RequireNonNull<T>(T? value)
        => value ?? throw new NullReferenceException("Required reference not set to an instance");

    internal static T RequireNonNull<T>(T? value) where T : struct
        => value ?? throw new NullReferenceException("Required reference not set to an instance");

    internal static IList<T> AsList<T>(params T?[] elements)
    {
        List<T> list = new();
        foreach(var e in elements) if(!ReferenceEquals(e, null)) list.Add(e);
        return list.AsReadOnly();
    }

    internal static JNode GetDerived(JNode target)
    {
        if(target is IDerived _derived) return _derived.Derived ?? target;
        return target;
    }
}