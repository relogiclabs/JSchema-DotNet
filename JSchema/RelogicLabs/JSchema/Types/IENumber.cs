namespace RelogicLabs.JSchema.Types;

public interface IENumber : IEValue
{
    EType IEValue.Type => EType.NUMBER;
    double ToDouble();
}