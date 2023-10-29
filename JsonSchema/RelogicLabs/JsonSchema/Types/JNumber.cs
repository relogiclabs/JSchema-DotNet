namespace RelogicLabs.JsonSchema.Types;

public abstract class JNumber : JPrimitive
{
    private protected JNumber(Builder builder) : base(builder) { }

    protected abstract double ToDouble();
    public static implicit operator double(JNumber number) => number.ToDouble();

    public int Compare(double other)
    {
        double number = ToDouble();
        if(AreEqual(number, other)) return 0;
        return Math.Sign(number - other);
    }

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        if(obj.GetType() != this.GetType()) return false;
        var other = (JNumber) obj;
        return AreEqual(this.ToDouble(), other.ToDouble());
    }

    internal bool AreEqual(double value1, double value2)
        => Runtime.AreEqual(value1, value2);

    public override JsonType Type => JsonType.NUMBER;
    public override int GetHashCode() => base.GetHashCode();
}