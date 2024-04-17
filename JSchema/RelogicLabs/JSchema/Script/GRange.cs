using RelogicLabs.JSchema.Types;

namespace RelogicLabs.JSchema.Script;

internal sealed class GRange : IEValue
{
    private const string RangeOperator = "..";
    private readonly int _start;
    private readonly int _end;
    public EType Type => EType.RANGE;

    private GRange(int start, int end)
    {
        _start = start;
        _end = end;
    }

    internal static GRange From(IEInteger? start, IEInteger? end)
    {
        var _start = start?.Value ?? int.MinValue;
        var _end = end?.Value ?? int.MinValue;
        return new GRange((int) _start, (int) _end);
    }

    public int GetStart(int count)
    {
        if(_start == int.MinValue) return 0;
        return _start >= 0 ? _start : count + _start;
    }

    public int GetEnd(int count)
    {
        if(_end == int.MinValue) return count;
        return _end >= 0 ? _end : count + _end;
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is GRange other
            && _start == other._start && _end == other._end;

    public override int GetHashCode() => HashCode.Combine(_start, _end);

    public override string ToString()
        => ToString(_start) + RangeOperator + ToString(_end);

    private static string ToString(int value)
        => value != int.MinValue ? value.ToString() : string.Empty;
}