using RelogicLabs.JSchema.Library;

namespace RelogicLabs.JSchema.Types;

public interface IScriptable
{
    internal MethodEvaluator GetMethod(string name, int argCount);
}