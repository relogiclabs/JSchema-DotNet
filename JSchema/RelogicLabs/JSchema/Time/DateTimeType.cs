using RelogicLabs.JsonSchema.Types;
using static RelogicLabs.JsonSchema.Types.JsonType;

namespace RelogicLabs.JsonSchema.Time;

public sealed class DateTimeType
{
    public static readonly DateTimeType DATE_TYPE = new("date", DATE);
    public static readonly DateTimeType TIME_TYPE = new("time", TIME);

    public string Name { get; }
    public JsonType Type { get; }

    private DateTimeType(string name, JsonType type)
    {
        Name = name;
        Type = type;
    }

    public override string ToString() => Name;
}