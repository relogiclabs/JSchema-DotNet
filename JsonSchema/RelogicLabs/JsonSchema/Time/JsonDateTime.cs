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
    private const int DEFAULT_UTC_OFFSET_HOUR = 0;
    private const int DEFAULT_UTC_OFFSET_MINUTE = 0;

    public const int UNSET = -1000;

    public DateTimeType Type { get; }
    public int Year { get; }
    public int Month { get; }
    public int Day { get; }
    public int Hour { get; }
    public int Minute { get; }
    public int Second { get; }
    public int Fraction { get; }
    public int UtcOffsetHour { get; }
    public int UtcOffsetMinute { get; }

    private DateTimeOffset _dateTimeOffset;
    private TimeSpan _utcOffset;

    internal JsonDateTime(DateTimeType type, int year = UNSET, int month = UNSET,
        int day = UNSET, int hour = UNSET, int minute = UNSET, int second = UNSET,
        int fraction = UNSET, int utcOffsetHour = UNSET, int utcOffsetMinute = UNSET)
    {
        Type = type;
        Year = year;
        Month = month;
        Day = day;
        Hour = hour;
        Minute = minute;
        Second = second;
        Fraction = fraction;
        UtcOffsetHour = utcOffsetHour;
        UtcOffsetMinute = utcOffsetMinute;

        _utcOffset = new TimeSpan(
            DefaultIfUnset(utcOffsetHour, DEFAULT_UTC_OFFSET_HOUR),
            DefaultIfUnset(utcOffsetMinute, DEFAULT_UTC_OFFSET_MINUTE), 0);

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

    internal JDateTime CreateNode(JString dateTime)
    {
        if(Type == DATE_TYPE) return new JDate(dateTime, this);
        if(Type == TIME_TYPE) return new JTime(dateTime, this);
        throw new InvalidOperationException("Invalid date time type");
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
        if(UtcOffsetHour != UNSET) builder.Append($"UTC Offset Hour: {UtcOffsetHour}, ");
        if(UtcOffsetMinute != UNSET) builder.Append($"UTC Offset Minute: {UtcOffsetMinute}, ");
        var result = builder.ToString();
        if(result.EndsWith(", ")) result = result[..^2];
        return result + "}";
    }
}