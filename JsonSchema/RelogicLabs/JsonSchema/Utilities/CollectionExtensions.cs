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

    public static string ToString(this IEnumerable<JNode> source, string separator,
        string prefix = "", string suffix = "")
    {
        var result = string.Join(separator, source);
        return $"{prefix}{result}{suffix}";
    }

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

    // Use only if necessary to eagerly check every item
    public static bool ForEachTrue(this IEnumerable<bool> enumeration)
    {
        var result = true;
        foreach(var item in enumeration) result &= item;
        return result;
    }

    public static IIndexMap<string, JProperty> ToIndexMap(this IEnumerable<JProperty> source)
        => new IndexHashMap<string, JProperty>(source);

    public static bool AllTrue(this IEnumerable<bool> source)
        => source.All(i => i);

    public static bool AnyTrue(this IEnumerable<bool> source)
        => source.Any(i => i);

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

    public static TV? GetValue<TK, TV>(this IDictionary<TK, TV> dict, TK key)
        => dict.TryGetValue(key, out var value) ? value : default;
}