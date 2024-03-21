namespace RelogicLabs.JSchema.Types;

public interface IEDouble : IENumber
{
    EType IEValue.Type => EType.DOUBLE;
    double Value { get; }
    double IENumber.ToDouble() => Value;
}