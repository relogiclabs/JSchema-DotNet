namespace RelogicLabs.JsonSchema.Utilities;

internal static class CommonUtilities
{
    public static T NonNull<T>(T? value)
        => value ?? throw new NullReferenceException("Required reference not set to an instance");

    public static T NonNull<T>(T? value) where T : struct
        => value ?? throw new NullReferenceException("Required reference not set to an instance");

    public static IList<T> AsList<T>(params T?[] elements)
    {
        List<T> list = new();
        foreach(var e in elements) if(!ReferenceEquals(e, null)) list.Add(e);
        return list.AsReadOnly();
    }
}