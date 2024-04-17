using RelogicLabs.JSchema.Library;

namespace RelogicLabs.JSchema.Types;

public interface IEArray : IEValue
{
    EType IEValue.Type => EType.ARRAY;
    IReadOnlyList<IEValue> Values { get; }
    int Count => Values.Count;
    IEValue Get(int index);
    void Set(int index, IEValue value);
    MethodEvaluator IScriptable.GetMethod(string name, int argCount)
        => ArrayLibrary.Instance.GetMethod(name, argCount);
}