using RelogicLabs.JSchema.Collections;
using RelogicLabs.JSchema.Nodes;

namespace RelogicLabs.JSchema.Utilities;

internal static class CollectionExtensions
{
    public static string Join(this IEnumerable<object> source, string separator,
        string prefix = "", string suffix = "")
    {
        var result = string.Join(separator, source);
        return string.IsNullOrEmpty(result) ? result : $"{prefix}{result}{suffix}";
    }

    public static string JoinWith(this IEnumerable<object> source, string separator,
        string prefix = "", string suffix = "") => $"{prefix}{string.Join(separator,
            source)}{suffix}";

    public static void Merge<TKey, TValue>(this IDictionary<TKey, List<TValue>> source,
        IDictionary<TKey, List<TValue>> other)
    {
        foreach(var pair in other)
        {
            source.TryGetValue(pair.Key, out var values);
            if(values == default) source.Add(pair.Key, other[pair.Key]);
            else values.AddRange(other[pair.Key]);
        }
    }

    public static T? TryGetLast<T>(this IList<T> list)
        => list.Count == 0 ? default : list[^1];

    public static IList<T> GetRange<T>(this IEnumerable<T> source, int start, int? end = null)
    {
        var asList = source.ToList();
        try
        {
            var _end = end ?? asList.Count;
            return asList.GetRange(start, _end - start);
        }
        catch(Exception e)
        {
            throw new IndexOutOfRangeException(e.Message, e);
        }
    }

    public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
    {
        foreach(var item in enumeration) action(item);
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
        ISet<string> _keys = keys.Select(static s => s.Value).ToHashSet();
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

    public static bool IsEmpty<T>(this IEnumerable<T> source)
    {
        return source switch
        {
            ICollection<T> c1 => c1.Count == 0,
            IReadOnlyCollection<T> c2 => c2.Count == 0,
            _ => !source.Any()
        };
    }
}