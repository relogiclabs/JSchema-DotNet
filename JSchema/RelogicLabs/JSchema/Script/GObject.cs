using RelogicLabs.JSchema.Types;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Types.IEValue;

namespace RelogicLabs.JSchema.Script;

internal sealed class GObject : IEObject
{
    public IDictionary<string, IEValue> Properties { get; }
    public int Count => Properties.Count;
    public IEnumerable<string> Keys => Properties.Keys;
    public GObject(int capacity) => Properties = new Dictionary<string, IEValue>(capacity);

    public GObject(IEObject value) : this(value.Count)
    {
        foreach(var k in value.Keys) Properties.Add(k, new GReference(value.Get(k)!));
    }

    public GObject(IList<string> keys, IList<IEValue> values) : this(keys.Count)
    {
        for(var i = 0; i < keys.Count; i++)
            Properties.Add(keys[i], new GReference(values[i]));
    }

    public IEValue Get(string key)
    {
        Properties.TryGetValue(key, out var value);
        if(value == default) Properties[key] = value = new GReference(VOID);
        return value;
    }

    public void Set(string key, IEValue value) => Properties[key] = value;
    public override string ToString() => Properties.Select(static p
        => $"{p.Key.Quote()}: {p.Value}").JoinWith(", ", "{", "}");
}