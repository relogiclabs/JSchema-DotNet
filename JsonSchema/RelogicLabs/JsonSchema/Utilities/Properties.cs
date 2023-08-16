using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace RelogicLabs.JsonSchema.Utilities;

public class Properties<T, TK, TV> : IProperties<T, TK, TV>, IReadOnlyList<T>
    where T : IProperty<TK, TV> where TK : notnull
{
    private const string CollectionIsReadOnly = "Collection is read-only.";
    private readonly ReadOnlyDictionary<TK, T> _dictionary;
    private readonly ReadOnlySet<TV> _values;
    private readonly ReadOnlyCollection<T> _items;
    
    public int Count => _items.Count;
    public bool IsReadOnly => true;
    
    public int IndexOf(T item) => _items.IndexOf(item);

    public void Insert(int index, T item) 
        => throw new NotSupportedException(CollectionIsReadOnly);

    public void RemoveAt(int index) 
        => throw new NotSupportedException(CollectionIsReadOnly);

    public T this[int index]
    {
        get => _items[index];
        set => throw new NotSupportedException();
    }

    public T this[TK key] => _dictionary[key];
    public IEnumerable<TK> Keys => _dictionary.Keys;
    public IEnumerable<TV> Values => _values;
    public IEnumerable<T> Pairs => _items;

    public Properties(IEnumerable<T> properties)
    {
        var list = properties.ToList();
        _dictionary = list.ToDictionary(kv => kv.GetPropertyKey(), kv => kv).AsReadOnly();
        _values = list.Select(kv => kv.GetPropertyValue()).ToHashSet().AsReadOnly();
        _items = list.AsReadOnly();
    }

    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Contains(T value) => _dictionary.ContainsKey(value.GetPropertyKey());
    public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);
    public bool ContainsKey(TK key) => _dictionary.ContainsKey(key);
    public bool ContainsValue(TV value) => _values.Contains(value);

    public IProperties<T, TK, TV> AddRange(IEnumerable<T> values)
        => throw new NotSupportedException(CollectionIsReadOnly);
    public IProperties<T, TK, TV> RemoveRange(IEnumerable<T> keys)
        => throw new NotSupportedException(CollectionIsReadOnly);

    public bool TryGetValue(TK key, [MaybeNullWhen(false)] out T value) 
        => _dictionary.TryGetValue(key, out value);

    public bool Remove(T value) => throw new NotSupportedException(CollectionIsReadOnly);
    public void Add(T item) => throw new NotSupportedException(CollectionIsReadOnly);
    public void Clear() => throw new NotSupportedException(CollectionIsReadOnly);
}