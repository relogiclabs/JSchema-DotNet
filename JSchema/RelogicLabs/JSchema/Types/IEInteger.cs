namespace RelogicLabs.JSchema.Types;

public interface IEInteger : IENumber
{
    EType IEValue.Type => EType.INTEGER;
    long Value { get; }
    double IENumber.ToDouble() => Value;
}