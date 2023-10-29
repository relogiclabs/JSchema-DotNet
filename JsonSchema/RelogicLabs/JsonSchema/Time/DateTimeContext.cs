using System.Text;
using RelogicLabs.JsonSchema.Exceptions;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static System.DayOfWeek;

namespace RelogicLabs.JsonSchema.Time;

internal class DateTimeContext
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

    private const int _UNSET = -100;

    private int _era = _UNSET;
    private int _year = _UNSET;
    private int _month = _UNSET;
    private int _weekday = _UNSET;
    private int _day = _UNSET;
    private int _amPm = _UNSET;
    private int _hour = _UNSET;
    private int _minute = _UNSET;
    private int _second = _UNSET;
    private int _fraction = _UNSET;
    private int _utcOffsetHour = _UNSET;
    private int _utcOffsetMinute = _UNSET;

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
        if(_hour != _UNSET && _hour is < 1 or > 12)
            throw new InvalidDateTimeException(DHUR03,
                $"Invalid {Type} hour AM/PM out of range");
        SetField(ref _amPm, amPmNum);
    }

    public void SetHour(int hour)
    {
        if(_amPm != _UNSET && _hour is < 1 or > 12)
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
        SetField(ref _utcOffsetHour, hour);
        SetField(ref _utcOffsetMinute, minute);
    }

    private void SetField(ref int field, int value)
    {
        if(field != _UNSET && field != value)
            throw new InvalidDateTimeException(DCNF01,
                $"Conflicting {Type} segments input");
        field = value;
    }

    private static bool IsAllSet(params int[] values)
        => values.All(value => value != _UNSET);

    public void Validate()
    {
        try
        {
            DateTime dateTime;
            if(IsAllSet(_year, _month, _day))
            {
                _DaysInMonth[2] = IsLeapYear(_year)? 29 : 28;
                if(_day < 1 || _day > _DaysInMonth[_month])
                    throw new InvalidDateTimeException(DDAY03,
                        $"Invalid {Type} day out of range");
                dateTime = new DateTime(_year, _month, _day);
                if(_weekday != _UNSET && (int) dateTime.DayOfWeek != _weekday)
                    throw new InvalidDateTimeException(DWKD03, $"Invalid {Type} weekday input");
            }

            if(IsAllSet(_year, _month)) dateTime = new DateTime(_year, _month, 1);
            if(IsAllSet(_year)) dateTime = new DateTime(_year, 1, 1);
        }
        catch(InvalidDateTimeException) { throw; }
        catch(Exception ex)
        {
            throw new InvalidDateTimeException(DINV01,
                $"Invalid {Type} year, month or day out of range", ex);
        }

        if(IsAllSet(_hour, _amPm)) ConvertTo24Hour();
        if(_hour != _UNSET && _hour is < 0 or > 23)
            throw new InvalidDateTimeException(DHUR05, $"Invalid {Type} hour out of range");
    }

    private void ConvertTo24Hour()
    {
        if(_amPm == 2 && _hour != 12) _hour += 12;
        else if(_amPm == 1 && _hour == 12) _hour = 0;
    }

    public override string ToString()
    {
        StringBuilder builder = new("{");
        if(_era != _UNSET) builder.Append($"Era: {_era}, ");
        if(_year != _UNSET) builder.Append($"Year: {_year}, ");
        if(_month != _UNSET) builder.Append($"Month: {_month}, ");
        if(_weekday != _UNSET) builder.Append($"Weekday: {_weekday}, ");
        if(_day != _UNSET) builder.Append($"Day: {_day}, ");
        if(_amPm != _UNSET) builder.Append($"AM/PM: {_amPm}, ");
        if(_hour != _UNSET) builder.Append($"Hour: {_hour}, ");
        if(_minute != _UNSET) builder.Append($"Minute: {_minute}, ");
        if(_second != _UNSET) builder.Append($"Second: {_second}, ");
        if(_fraction != _UNSET) builder.Append($"Fraction: {_fraction}, ");
        if(_utcOffsetHour != _UNSET) builder.Append($"UTC Offset Hour: {_utcOffsetHour}, ");
        if(_utcOffsetMinute != _UNSET) builder.Append($"UTC Offset Minute: {_utcOffsetMinute}, ");
        var result = builder.ToString();
        if(result.EndsWith(", ")) result = result[..^2];
        return result + "}";
    }

    private static bool IsLeapYear(int year)
        => (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);

    private static int ToFourDigitYear(int year)
    {
        var century = DateTime.Now.Year / 100 * 100;
        return year < PIVOT_YEAR ? century + year : century - 100 + year;
    }
}