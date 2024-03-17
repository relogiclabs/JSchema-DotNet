using RelogicLabs.JSchema.Types;
using static RelogicLabs.JSchema.Engine.ScriptTreeHelper;

namespace RelogicLabs.JSchema.Script;

internal sealed class GReference : IEValue
{
    private IEValue _value;

    public IEValue Value
    {
        get => _value;
        set => _value = Dereference(value);
    }
    public EType Type => Value.Type;

    public GReference(IEValue value) => _value = Dereference(value);
    public override bool Equals(object? obj)
        => obj is IEValue v && Dereference(v).Equals(Value);
    public override int GetHashCode() => Value.GetHashCode();
    public bool ToBoolean() => Value.ToBoolean();
    public override string ToString() => Value.ToString()
            ?? throw new InvalidOperationException("Invalid runtime state");
}