using System.Collections;
using System.Collections.ObjectModel;
using static RelogicLabs.JSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JSchema.Collections;

internal sealed class IndexHashMap<TK, TV> : IIndexMap<TK, TV>
    where TV : IKeyed<TK> where TK : notnull
{
    private IDictionary<TK, TV> _dictionary;
    private IList<TV> _list;

    public IndexHashMap(IEnumerable<TV> source)
    {
        _list = source.ToList();
        _dictionary = _list.ToDictionary(static e => e.Key, Identity);
    }

    public IEnumerator<TV> GetEnumerator() => _list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    public void Add(TV item)
    {
        _list.Add(item);
        _dictionary.Add(item.Key, item);
    }

    public bool Remove(TV item)
    {
        var result = _list.Remove(item);
        result &= _dictionary.Remove(item.Key);
        return result;
    }

    public void Clear()
    {
        _list.Clear();
        _dictionary.Clear();
    }

    public bool Contains(TV item) => _list.Contains(item);
    public void CopyTo(TV[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
    public int Count => _list.Count;
    public TV this[TK key] => _dictionary[key];
    public int IndexOf(TV item) => _list.IndexOf(item);

    public void Insert(int index, TV item)
    {
        _list.Insert(index, item);
        _dictionary.Add(item.Key, item);
    }

    public void RemoveAt(int index)
    {
        var item = _list[index];
        _list.RemoveAt(index);
        _dictionary.Remove(item.Key);
    }

    public TV this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }

    public IEnumerable<TK> Keys => _dictionary.Keys;
    public IEnumerable<TV> Values => _list;


    public bool IsReadOnly => _dictionary.IsReadOnly && _list.IsReadOnly;
    public bool TryGetValue(TK key, out TV? value)
        => _dictionary.TryGetValue(key, out value);

    public IIndexMap<TK, TV> AsReadOnly()
    {
        if(IsReadOnly) return this;
        _list = new ReadOnlyCollection<TV>(_list);
        _dictionary = new ReadOnlyDictionary<TK, TV>(_dictionary);
        return this;
    }
}