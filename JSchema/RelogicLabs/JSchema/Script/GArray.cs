using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Types;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Message.ErrorCode;

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

    public IEValue Get(int index) => Elements[index];

    public void Set(int index, IEValue value)
    {
        var count = Elements.Count;
        if(index < count)
        {
            if(Elements[index] is not GReference r) throw new UpdateNotSupportedException(AUPD03,
                $"Readonly array index {index} cannot be updated");
            r.Value = value;
            return;
        }
        if(index > count) throw new ScriptCommonException(AWRT01,
            $"Index {index} is out of bounds for writing to array length {count}");
        if(index > MaxLimit) throw new ScriptCommonException(AWRT02,
            $"Array index {index} exceeds maximum size limit");
        Elements.Add(new GReference(value));
    }

    public override string ToString() => Elements.JoinWith(", ", "[", "]");
}