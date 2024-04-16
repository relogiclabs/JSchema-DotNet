using RelogicLabs.JSchema.Library;

namespace RelogicLabs.JSchema.Types;

public interface IEString : IEValue
{
    EType IEValue.Type => EType.STRING;
    string Value { get; }
    int Length => Value.Length;
    MethodEvaluator IScriptable.GetMethod(string name, int argCount)
        => StringLibrary.Instance.GetMethod(name, argCount);
}