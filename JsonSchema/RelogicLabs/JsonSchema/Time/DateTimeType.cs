namespace RelogicLabs.JsonSchema.Time;

internal sealed class DateTimeType
{
    public static readonly DateTimeType DATE_TYPE = new("date");
    public static readonly DateTimeType TIME_TYPE = new("time");

    private string Name { get; }

    private DateTimeType(string name) => Name = name;
    public override string ToString() => Name;
}