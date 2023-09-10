namespace RelogicLabs.JsonSchema.Time;

internal class DateTimeType
{
    public static readonly DateTimeType DATE_TYPE = new(1, "date");
    public static readonly DateTimeType TIME_TYPE = new(2, "time");

    private int Id { get; }
    private string Name { get; }

    private DateTimeType(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public override string ToString() => Name;
}