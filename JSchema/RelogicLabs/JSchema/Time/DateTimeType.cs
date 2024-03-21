using RelogicLabs.JSchema.Types;
using static RelogicLabs.JSchema.Types.EType;

namespace RelogicLabs.JSchema.Time;

public sealed class DateTimeType
{
    public static readonly DateTimeType DATE_TYPE = new("date", DATE);
    public static readonly DateTimeType TIME_TYPE = new("time", TIME);

    public string Name { get; }
    public EType Type { get; }

    private DateTimeType(string name, EType type)
    {
        Name = name;
        Type = type;
    }

    public override string ToString() => Name;
}