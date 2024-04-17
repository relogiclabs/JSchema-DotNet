using RelogicLabs.JSchema.Library;

namespace RelogicLabs.JSchema.Types;

public interface IEObject : IEValue
{
    EType IEValue.Type => EType.OBJECT;
    IEnumerable<string> Keys { get; }
    int Count { get; }
    IEValue? Get(string key);
    void Set(string key, IEValue value);
    MethodEvaluator IScriptable.GetMethod(string name, int argCount)
        => ObjectLibrary.Instance.GetMethod(name, argCount);
}