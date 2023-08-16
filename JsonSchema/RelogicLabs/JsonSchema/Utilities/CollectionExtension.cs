using RelogicLabs.JsonSchema.Types;

namespace RelogicLabs.JsonSchema.Utilities;

internal static class CollectionExtension
{
    public static string ToString(this IEnumerable<string> source, string separator,
        string prefix = "", string suffix = "")
    {
        IEnumerable<string> collection = source as string[] 
                                         ?? source as IList<string> 
                                         ?? source.ToArray();
        var result = string.Join(separator, collection);
        return collection.Any()? $"{prefix}{result}{suffix}" : string.Empty;
    }

    public static void AddRange<TKey, TValue>(this IDictionary<TKey, List<TValue>> source, 
        IDictionary<TKey, List<TValue>> collection)
    {
        foreach(var element in collection)
        {
            source.TryGetValue(element.Key, out List<TValue>? value);
            if(value == default) value = collection[element.Key];
            else value.AddRange(collection[element.Key]);
            source.Add(element.Key, value);
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
    
    public static bool ForEachTrue(this IEnumerable<bool> enumeration)
    {
        var result = true;
        foreach(var item in enumeration) result &= item;
        return result;
    }

    public static Properties<JProperty, string, JNode> ToProperties(
        this IEnumerable<JProperty> source) => new(source);

    public static ReadOnlySet<T> AsReadOnly<T>(this ISet<T> set) 
        => new ReadOnlySet<T>(set);
    
    public static IEnumerable<string> ToJson(this IEnumerable<JNode> source) 
        => source.Select(s => s.ToJson());
    
    public static bool AllTrue(this IEnumerable<bool> source) 
        => source.All(i => i);
    
    public static bool AnyTrue(this IEnumerable<bool> source) 
        => source.Any(i => i);
}