using RelogicLabs.JSchema.Library;

namespace RelogicLabs.JSchema.Types;

public interface IEValue : IScriptable
{
    private class EVoid : IEValue
    {
        public EType Type => EType.VOID;
        public bool ToBoolean() => false;
        public override string ToString() => Type.ToString();
    }
    static readonly IEValue VOID = new EVoid();
    EType Type => EType.ANY;
    bool ToBoolean() => true;
    MethodEvaluator IScriptable.GetMethod(string name, int argCount)
        => CommonLibrary.Instance.GetMethod(name, argCount);
}