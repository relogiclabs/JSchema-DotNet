using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Types;
using static RelogicLabs.JsonSchema.Message.MessageFormatter;

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
        foreach(var e in elements) if(e is not null) list.Add(e);
        return list.AsReadOnly();
    }

    internal static JNode GetDerived(JNode target)
    {
        if(target is IDerived _derived) return _derived.Derived ?? target;
        return target;
    }

    internal static IEnumerable<JProperty> RequireUniqueness(List<JProperty> list, string errorCode)
    {
        var group = list.GroupBy(p => p.Key).FirstOrDefault(g => g.Count() > 1);
        if(group == default) return list;
        JProperty property = group.First();
        throw new DuplicatePropertyKeyException(FormatForJson(errorCode,
            $"Multiple key with name '{property.Key}' found", property));
    }

    internal static T? TryGetLast<T>(List<T> list) => list.IsEmpty() ? default : list[^1];
}