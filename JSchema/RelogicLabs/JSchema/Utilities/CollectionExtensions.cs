using RelogicLabs.JsonSchema.Collections;
using RelogicLabs.JsonSchema.Types;

namespace RelogicLabs.JsonSchema.Utilities;

internal static class CollectionExtensions
{
    public static string Join(this IEnumerable<object> source, string separator,
        string prefix = "", string suffix = "")
    {
        var result = string.Join(separator, source);
        return string.IsNullOrEmpty(result) ? result : $"{prefix}{result}{suffix}";
    }

    public static string JoinWith(this IEnumerable<JNode> source, string separator,
        string prefix = "", string suffix = "") => $"{prefix}{string.Join(separator,
            source)}{suffix}";

    public static void Merge<TKey, TValue>(this IDictionary<TKey, List<TValue>> source,
        IDictionary<TKey, List<TValue>> other)
    {
        foreach(var pair in other)
        {
            source.TryGetValue(pair.Key, out List<TValue>? values);
            if(values == default) source.Add(pair.Key, other[pair.Key]);
            else values.AddRange(other[pair.Key]);
        }
    }

    public static IList<T> GetRange<T>(this IList<T> source, int index, int count)
    {
        if(source is List<T> asList) return asList.GetRange(index, count);
        List<T> range = new(count);
        int limit = index + count;
        for(int i = index; i < limit; i++) range.Add(source[i]);
        return range;
    }

    public static IList<T> ToReadOnlyList<T>(this IEnumerable<T> source)
        => source.ToList().AsReadOnly();

    public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
    {
        foreach(T item in enumeration) action(item);
    }

    public static bool ForEachTrue<T>(this IEnumerable<T> enumeration, Func<T, bool> predicate)
    {
        // When no short-circuit evaluation with no early return
        var result = true;
        foreach(var item in enumeration) result &= predicate(item);
        return result;
    }

    public static IIndexMap<string, JProperty> ToIndexMap(this IEnumerable<JProperty> source)
        => new IndexHashMap<string, JProperty>(source);

    public static IEnumerable<string> ContainsKeys(
        this IEnumerable<JProperty> source, params JString[] keys)
    {
        ISet<string> _keys = keys.Select(s => s.Value).ToHashSet();
        source.ForEach(p => _keys.Remove(p.Key));
        return _keys;
    }

    public static IEnumerable<JNode> ContainsValues(
        this IEnumerable<JProperty> source, params JNode[] values)
    {
        // Here values should be distinct on parameter
        ISet<JNode> _values = values.ToHashSet();
        source.ForEach(p => _values.Remove(p.Value));
        return _values;
    }

    public static TV? TryGetValue<TK, TV>(this IDictionary<TK, TV> dict, TK key)
        => dict.TryGetValue(key, out var value) ? value : default;

    public static List<T> AddToList<T>(this List<T> source, params IEnumerable<T>?[] collection)
    {
        foreach(var e in collection) if(e is not null) source.AddRange(e);
        return source;
    }

    public static List<T> AddToList<T>(this List<T> source, params T?[] collection)
    {
        foreach(var e in collection) if(e is not null) source.Add(e);
        return source;
    }

    public static bool IsEmpty<T>(this IEnumerable<T> source) => !source.Any();
}