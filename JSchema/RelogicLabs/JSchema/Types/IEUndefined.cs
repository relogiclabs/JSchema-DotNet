namespace RelogicLabs.JSchema.Types;

public interface IEUndefined : IEValue
{
    const string Literal = "undefined";
    const string Marker = "!";
    private class EUndefined : IEUndefined
    {
        public override string ToString() => Literal;
    }
    static readonly IEUndefined UNDEFINED = new EUndefined();
    EType IEValue.Type => EType.UNDEFINED;
    bool IEValue.ToBoolean() => false;
}