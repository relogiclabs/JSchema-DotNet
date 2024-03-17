using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Types;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Types.IEValue;

namespace RelogicLabs.JSchema.Script;

internal sealed class GArray : IEArray
{
    private const int MaxLimit = short.MaxValue;
    public IList<IEValue> Elements { get; }
    public IReadOnlyList<IEValue> Values => (IReadOnlyList<IEValue>) Elements;

    public GArray(int capacity) => Elements = new List<IEValue>(capacity);

    public GArray(IEnumerable<IEValue> collection)
        => Elements = collection.Select(static i => (IEValue) new GReference(i)).ToList();

    public static GArray From(IEArray array, GRange range)
    {
        var list = array.Values;
        var count = list.Count;
        return new GArray(list.GetRange(range.GetStart(count), range.GetEnd(count)));
    }

    public static GArray FilledFrom(IEValue value, int count)
    {
        var array = new GArray(count);
        for(var i = 0; i < count; i++) array.Elements.Add(new GReference(value));
        return array;
    }

    public IEValue Get(int index)
    {
        var count = Elements.Count;
        if(index < count) return Elements[index];
        if(index > MaxLimit) throw new ScriptCommonException(INDX01,
            $"Array index {index} exceeds maximum size limit");
        if(index > count) throw new IndexOutOfRangeException($"Array index out of range: {index}");
        var reference = new GReference(VOID);
        Elements.Add(reference);
        return reference;
    }

    public override string ToString() => Elements.JoinWith(", ", "[", "]");
}