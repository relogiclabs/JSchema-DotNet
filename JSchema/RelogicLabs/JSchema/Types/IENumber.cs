using RelogicLabs.JSchema.Library;

namespace RelogicLabs.JSchema.Types;

public interface IENumber : IEValue
{
    EType IEValue.Type => EType.NUMBER;
    double ToDouble();
    MethodEvaluator IScriptable.GetMethod(string name, int argCount)
        => NumberLibrary.Instance.GetMethod(name, argCount);
}