namespace RelogicLabs.JSchema.Utilities;

internal static class CommonUtilities
{
    internal static T RequireNonNull<T>(T? value)
        => value ?? throw new NullReferenceException("Required reference not set to an instance");

    internal static T RequireNonNull<T>(T? value) where T : struct
        => value ?? throw new NullReferenceException("Required reference not set to an instance");

    internal static IList<T> AsList<T>(params T?[] elements)
    {
        List<T> list = new();
        foreach(var e in elements) if(e is not null) list.Add(e);
        return list.AsReadOnly();
    }

    internal static bool HasFlag(int flagSet, int flag) => (flagSet & flag) == flag;
}