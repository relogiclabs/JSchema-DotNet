namespace RelogicLabs.JsonSchema.Types;

public abstract class JNumber : JPrimitive, IJsonType<JNumber>
{
    public virtual JsonType Type => JsonType.NUMBER;
    internal JNumber(IDictionary<JNode, JNode> relations) : base(relations) { }
    protected abstract double ToDouble();

    public static implicit operator double(JNumber number) => number.ToDouble();

    public int Compare(double other)
    {
        double number = ToDouble();
        if(IsEquivalent(number, other)) return 0;
        return Math.Sign(number - other);
    }
    
    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        if(obj.GetType() != this.GetType()) return false;
        var other = (JNumber) obj;
        return IsEquivalent(this.ToDouble(), other.ToDouble());
    }

    public override int GetHashCode() => base.GetHashCode();
    
}