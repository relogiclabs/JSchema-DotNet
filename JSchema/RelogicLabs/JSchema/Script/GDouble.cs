using RelogicLabs.JSchema.Types;

namespace RelogicLabs.JSchema.Script;

internal sealed class GDouble : IEDouble
{
    public double Value { get; }
    private GDouble(double value) => Value = value;
    public static GDouble From(double value) => new(value);

    public override string ToString() => Value.ToString();
}