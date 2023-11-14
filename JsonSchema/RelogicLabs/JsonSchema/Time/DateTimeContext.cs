using System.Text;
using RelogicLabs.JsonSchema.Exceptions;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static System.DayOfWeek;
using static RelogicLabs.JsonSchema.Time.JsonDateTime;

namespace RelogicLabs.JsonSchema.Time;

internal sealed class DateTimeContext
{
    private const int PIVOT_YEAR = 50;
    private static readonly int[] _DaysInMonth = { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
    private static readonly Dictionary<string, int> _Months = new();
    private static readonly Dictionary<string, int> _Weekdays = new();

    static DateTimeContext()
    {
        AddMonth("january", "jan", 1);
        AddMonth("february", "feb", 2);
        AddMonth("march", "mar", 3);
        AddMonth("april", "apr", 4);
        AddMonth("may", "may", 5);
        AddMonth("june", "jun", 6);
        AddMonth("july", "jul", 7);
        AddMonth("august", "aug", 8);
        AddMonth("september", "sep", 9);
        AddMonth("october", "oct", 10);
        AddMonth("november", "nov", 11);
        AddMonth("december", "dec", 12);

        AddWeekday("sunday", "sun", (int) Sunday);
        AddWeekday("monday", "mon", (int) Monday);
        AddWeekday("tuesday", "tue", (int) Tuesday);
        AddWeekday("wednesday", "wed", (int) Wednesday);
        AddWeekday("thursday", "thu", (int) Thursday);
        AddWeekday("friday", "fri", (int) Friday);
        AddWeekday("saturday", "sat", (int) Saturday);
    }

    private static void AddMonth(string key1, string key2, int value)
    {
        _Months[key1] = value;
        _Months[key2] = value;
    }

    private static void AddWeekday(string key1, string key2, int value)
    {
        _Weekdays[key1] = value;
        _Weekdays[key2] = value;
    }

    private int _era = UNSET;
    private int _year = UNSET;
    private int _month = UNSET;
    private int _weekday = UNSET;
    private int _day = UNSET;
    private int _amPm = UNSET;
    private int _hour = UNSET;
    private int _minute = UNSET;
    private int _second = UNSET;
    private int _fraction = UNSET;
    private int _utcHour = UNSET;
    private int _utcMinute = UNSET;

    public DateTimeType Type { get; }

    public DateTimeContext(DateTimeType type)
        => Type = type;

    public void SetEra(string era)
    {
        var eraNum = era.ToUpper() switch
        {
            "BC" => 1,
            "AD" => 2,
            _ => throw new InvalidDateTimeException(DERA02, $"Invalid {Type} era input")
        };
        SetField(ref _era, eraNum);
    }

    public void SetYear(int year, int digitNum)
    {
        if(year is < 1 or > 9999) throw new InvalidDateTimeException(DYAR03,
            $"Invalid {Type} year out of range");
        year = digitNum <= 2 ? ToFourDigitYear(year) : year;
        SetField(ref _year, year);
    }

    public void SetMonth(string month)
    {
        var monthNum = _Months[month.ToLower()];
        SetField(ref _month, monthNum);
    }

    public void SetMonth(int month)
    {
        if(month is < 1 or > 12) throw new InvalidDateTimeException(DMON05,
            $"Invalid {Type} month out of range");
        SetField(ref _month, month);
    }

    public void SetWeekday(string weekday)
    {
        var dayOfWeek = _Weekdays[weekday.ToLower()];
        SetField(ref _weekday, dayOfWeek);
    }

    public void SetDay(int day)
    {
        if(day is < 1 or > 31) throw new InvalidDateTimeException(DDAY04,
            $"Invalid {Type} day out of range");
        SetField(ref _day, day);
    }

    public void SetAmPm(string amPm)
    {
        var amPmNum = amPm.ToLower() switch
        {
            "am" => 1,
            "pm" => 2,
            _ => throw new InvalidDateTimeException(DTAP02,
                $"Invalid {Type} hour AM/PM input")
        };
        if(_hour != UNSET && _hour is < 1 or > 12)
            throw new InvalidDateTimeException(DHUR03,
                $"Invalid {Type} hour AM/PM out of range");
        SetField(ref _amPm, amPmNum);
    }

    public void SetHour(int hour)
    {
        if(_amPm != UNSET && _hour is < 1 or > 12)
            throw new InvalidDateTimeException(DHUR04,
                $"Invalid {Type} hour AM/PM out of range");
        if(hour is < 0 or > 23)
            throw new InvalidDateTimeException(DHUR06,
                $"Invalid {Type} hour out of range");
        SetField(ref _hour, hour);
    }

    public void SetMinute(int minute)
    {
        if(minute is < 0 or > 59) throw new InvalidDateTimeException(DMIN03,
            $"Invalid {Type} minute out of range");
        SetField(ref _minute, minute);
    }

    public void SetSecond(int second)
    {
        if(second is < 0 or > 59) throw new InvalidDateTimeException(DSEC03,
            $"Invalid {Type} second out of range");
        SetField(ref _second, second);
    }

    public void SetFraction(int fraction) => SetField(ref _fraction, fraction);

    public void SetUtcOffset(int hour, int minute)
    {
        if(hour is < -12 or > 12) throw new InvalidDateTimeException(DUTC04,
            $"Invalid {Type} UTC offset hour out of range");
        if(minute is < 0 or > 59) throw new InvalidDateTimeException(DUTC05,
            $"Invalid {Type} UTC offset minute out of range");
        SetField(ref _utcHour, hour);
        SetField(ref _utcMinute, minute);
    }

    private void SetField(ref int field, int value)
    {
        if(field != UNSET && field != value)
            throw new InvalidDateTimeException(DCNF01,
                $"Conflicting {Type} segments input");
        field = value;
    }

    private static bool IsAllSet(params int[] values)
        => values.All(value => value != UNSET);

    public JsonDateTime Validate()
    {
        try
        {
            if(IsAllSet(_year, _month, _day))
            {
                _DaysInMonth[2] = IsLeapYear(_year)? 29 : 28;
                if(_day < 1 || _day > _DaysInMonth[_month])
                    throw new InvalidDateTimeException(DDAY03,
                        $"Invalid {Type} day out of range");
                JsonDateTime dateTime = new(Type, _year, _month, _day);
                if(_weekday != UNSET && (int?) dateTime.GetDayOfWeek() != _weekday)
                    throw new InvalidDateTimeException(DWKD03, $"Invalid {Type} weekday input");
            }
            if(IsAllSet(_hour, _amPm)) ConvertTo24Hour();
            if(_hour != UNSET && _hour is < 0 or > 23)
                throw new InvalidDateTimeException(DHUR05, $"Invalid {Type} hour out of range");

            return new JsonDateTime(Type, _year, _month, _day, _hour, _minute, _second,
                _fraction, _utcHour, _utcMinute);
        }
        catch(InvalidDateTimeException) { throw; }
        catch(Exception ex)
        {
            throw new InvalidDateTimeException(DINV01,
                $"Invalid {Type} year, month or day out of range", ex);
        }
    }

    private void ConvertTo24Hour()
    {
        if(_amPm == 2 && _hour != 12) _hour += 12;
        else if(_amPm == 1 && _hour == 12) _hour = 0;
    }

    public override string ToString()
    {
        StringBuilder builder = new("{");
        if(_era != UNSET) builder.Append($"Era: {_era}, ");
        if(_year != UNSET) builder.Append($"Year: {_year}, ");
        if(_month != UNSET) builder.Append($"Month: {_month}, ");
        if(_weekday != UNSET) builder.Append($"Weekday: {_weekday}, ");
        if(_day != UNSET) builder.Append($"Day: {_day}, ");
        if(_amPm != UNSET) builder.Append($"AM/PM: {_amPm}, ");
        if(_hour != UNSET) builder.Append($"Hour: {_hour}, ");
        if(_minute != UNSET) builder.Append($"Minute: {_minute}, ");
        if(_second != UNSET) builder.Append($"Second: {_second}, ");
        if(_fraction != UNSET) builder.Append($"Fraction: {_fraction}, ");
        if(_utcHour != UNSET) builder.Append($"UTC Offset Hour: {_utcHour}, ");
        if(_utcMinute != UNSET) builder.Append($"UTC Offset Minute: {_utcMinute}, ");
        var result = builder.ToString();
        if(result.EndsWith(", ")) result = result[..^2];
        return result + "}";
    }

    private static bool IsLeapYear(int year)
        => (year % 4 == 0 && year % 100 != 0) || year % 400 == 0;

    private static int ToFourDigitYear(int year)
    {
        var century = DateTime.Now.Year / 100 * 100;
        return year < PIVOT_YEAR ? century + year : century - 100 + year;
    }
}