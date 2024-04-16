using RelogicLabs.JSchema.Types;

namespace RelogicLabs.JSchema.Script;

internal sealed class GBoolean : IEBoolean
{
    public static readonly GBoolean TRUE = new(true);
    public static readonly GBoolean FALSE = new(false);
    public bool Value { get; }

    private GBoolean(bool value) => Value = value;
    public override string ToString() => Value.ToString().ToLower();
}