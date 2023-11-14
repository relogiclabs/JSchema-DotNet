using System.Text;
using RelogicLabs.JsonSchema.Types;
using static RelogicLabs.JsonSchema.Time.DateTimeType;

namespace RelogicLabs.JsonSchema.Time;

public class JsonDateTime
{
    private const int DEFAULT_YEAR = 2000;
    private const int DEFAULT_MONTH = 1;
    private const int DEFAULT_DAY = 1;
    private const int DEFAULT_HOUR = 0;
    private const int DEFAULT_MINUTE = 0;
    private const int DEFAULT_SECOND = 0;
    private const int DEFAULT_FRACTION = 0;
    private const int DEFAULT_UTC_HOUR = 0;
    private const int DEFAULT_UTC_MINUTE = 0;

    public const int UNSET = -1000;

    public DateTimeType Type { get; }
    public int Year { get; }
    public int Month { get; }
    public int Day { get; }
    public int Hour { get; }
    public int Minute { get; }
    public int Second { get; }
    public int Fraction { get; }
    public int UtcHour { get; }
    public int UtcMinute { get; }

    private DateTimeOffset _dateTimeOffset;
    private TimeSpan _utcOffset;

    public JsonDateTime(DateTimeType type, int year = DEFAULT_YEAR, int month = DEFAULT_MONTH,
        int day = DEFAULT_DAY, int hour = DEFAULT_HOUR, int minute = DEFAULT_MINUTE, int second = DEFAULT_SECOND,
        int fraction = DEFAULT_FRACTION, int utcHour = DEFAULT_UTC_HOUR, int utcMinute = DEFAULT_UTC_MINUTE)
    {
        Type = type;
        Year = year;
        Month = month;
        Day = day;
        Hour = hour;
        Minute = minute;
        Second = second;
        Fraction = fraction;
        UtcHour = utcHour;
        UtcMinute = utcMinute;

        _utcOffset = new TimeSpan(
            DefaultIfUnset(utcHour, DEFAULT_UTC_HOUR),
            DefaultIfUnset(utcMinute, DEFAULT_UTC_MINUTE), 0);

        _dateTimeOffset = new DateTimeOffset(
            DefaultIfUnset(year, DEFAULT_YEAR),
            DefaultIfUnset(month, DEFAULT_MONTH),
            DefaultIfUnset(day, DEFAULT_DAY),
            DefaultIfUnset(hour, DEFAULT_HOUR),
            DefaultIfUnset(minute, DEFAULT_MINUTE),
            DefaultIfUnset(second, DEFAULT_SECOND),
            _utcOffset);
    }

    private static int DefaultIfUnset(int value, int defaultValue)
        => value == UNSET ? defaultValue : value;


    private static bool IsAllSet(params int[] values)
        => values.All(value => value != UNSET);

    public DayOfWeek? GetDayOfWeek()
    {
        if(IsAllSet(Year, Month, Day)) return _dateTimeOffset.DayOfWeek;
        return null;
    }

    public int Compare(JsonDateTime other)
    {
        int result = DateTimeOffset.Compare(_dateTimeOffset, other._dateTimeOffset);
        if(result == 0)
        {
            if(Fraction < other.Fraction) return -1;
            if(Fraction > other.Fraction) return +1;
        }
        return result;
    }

    public override string ToString()
    {
        StringBuilder builder = new("{");
        if(Year != UNSET) builder.Append($"Year: {Year}, ");
        if(Month != UNSET) builder.Append($"Month: {Month}, ");
        if(Day != UNSET) builder.Append($"Day: {Day}, ");
        if(Hour != UNSET) builder.Append($"Hour: {Hour}, ");
        if(Minute != UNSET) builder.Append($"Minute: {Minute}, ");
        if(Second != UNSET) builder.Append($"Second: {Second}, ");
        if(Fraction != UNSET) builder.Append($"Fraction: {Fraction}, ");
        if(UtcHour != UNSET) builder.Append($"UTC Offset Hour: {UtcHour}, ");
        if(UtcMinute != UNSET) builder.Append($"UTC Offset Minute: {UtcMinute}, ");
        var result = builder.ToString();
        if(result.EndsWith(", ")) result = result[..^2];
        return result + "}";
    }

    internal JDateTime Create(JString dateTime)
    {
        if(Type == DATE_TYPE) return new JDate(dateTime, this);
        if(Type == TIME_TYPE) return new JTime(dateTime, this);
        throw new InvalidOperationException("Invalid date time type");
    }
}