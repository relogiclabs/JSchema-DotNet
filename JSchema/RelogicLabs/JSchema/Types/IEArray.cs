namespace RelogicLabs.JSchema.Types;

public interface IEArray : IEValue
{
    EType IEValue.Type => EType.ARRAY;
    IReadOnlyList<IEValue> Values { get; }
    int Count => Values.Count;
    IEValue Get(int index);
}