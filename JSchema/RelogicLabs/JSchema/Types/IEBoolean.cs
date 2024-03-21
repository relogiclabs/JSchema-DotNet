namespace RelogicLabs.JSchema.Types;

public interface IEBoolean : IEValue
{
    EType IEValue.Type => EType.BOOLEAN;
    bool Value { get; }
    bool IEValue.ToBoolean() => Value;
}