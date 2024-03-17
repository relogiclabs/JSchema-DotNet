namespace RelogicLabs.JSchema.Types;

public interface IEObject : IEValue
{
    EType IEValue.Type => EType.OBJECT;
    IEnumerable<string> Keys { get; }
    int Count { get; }
    IEValue? Get(string key);
}