using RelogicLabs.JSchema.Types;

namespace RelogicLabs.JSchema.Script;

internal sealed class GInteger : IEInteger
{
    private const int CacheStart = -1;
    private const int CacheEnd = 256;
    private static readonly GInteger[] Cache = CreateCache();
    public long Value { get; }

    private GInteger(long value) => Value = value;

    public static GInteger From(long value)
        => value is >= CacheStart and <= CacheEnd
            ? Cache[(int) value - CacheStart] : new GInteger(value);

    private static GInteger[] CreateCache()
    {
        const int count = CacheEnd - CacheStart + 1;
        var cache = new GInteger[count];
        var value = CacheStart;
        for(var i = 0; i < count; i++) cache[i] = new GInteger(value++);
        return cache;
    }

    public override string ToString() => Value.ToString();
}