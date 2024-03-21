namespace RelogicLabs.JSchema.Types;

public interface IEValue
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
}