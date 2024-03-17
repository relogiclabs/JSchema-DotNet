using RelogicLabs.JSchema.Types;
using RelogicLabs.JSchema.Utilities;

namespace RelogicLabs.JSchema.Script;

internal sealed class GString : IEString
{
    public string Value { get; }
    private GString(string value) => Value = value;
    public static GString Of(string value) => new(value);
    public static GString Of(char value) => new(value.ToString());

    public static GString From(IEString source, GRange range)
    {
        var length = source.Length;
        var start = range.GetStart(length);
        var count = range.GetEnd(length) - start;
        return Of(source.Value.Substring(start, count));
    }

    public override string ToString() => Value.Quote();
}