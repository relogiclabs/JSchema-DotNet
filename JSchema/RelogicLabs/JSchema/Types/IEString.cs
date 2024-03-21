namespace RelogicLabs.JSchema.Types;

public interface IEString : IEValue
{
    EType IEValue.Type => EType.STRING;
    string Value { get; }
    int Length => Value.Length;
}