namespace RelogicLabs.JSchema.Types;

public interface IENull : IEValue
{
    const string Literal = "null";
    private class ENull : IENull
    {
        public override string ToString() => Literal;
    }
    static readonly IENull NULL = new ENull();
    EType IEValue.Type => EType.NULL;
    bool IEValue.ToBoolean() => false;
}