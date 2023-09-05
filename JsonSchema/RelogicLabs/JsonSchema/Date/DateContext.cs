using System.Text;
using RelogicLabs.JsonSchema.Exceptions;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static System.DayOfWeek;

namespace RelogicLabs.JsonSchema.Date;

internal class DateContext
{
    private const int PIVOT_YEAR = 50;
    private static readonly int[] _DaysInMonth = { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
    private static readonly Dictionary<string, int> _Months = new();
    private static readonly Dictionary<string, DayOfWeek> _Weekdays = new();

    static DateContext()
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
        
        AddWeekday("sunday", "sun", Sunday);
        AddWeekday("monday", "mon", Monday);
        AddWeekday("tuesday", "tue", Tuesday);
        AddWeekday("wednesday", "wed", Wednesday);
        AddWeekday("thursday", "thu", Thursday);
        AddWeekday("friday", "fri", Friday);
        AddWeekday("saturday", "sat", Saturday);
    }

    private static void AddMonth(string key1, string key2, int value)
    {
        _Months[key1] = value;
        _Months[key2] = value;
    }

    private static void AddWeekday(string key1, string key2, DayOfWeek value)
    {
        _Weekdays[key1] = value;
        _Weekdays[key2] = value;
    }
    
    private const int _UNSET = -100;

    private int _era = _UNSET;
    private int _year = _UNSET;
    private int _month = _UNSET;
    private DayOfWeek _weekday = (DayOfWeek) _UNSET;
    private int _day = _UNSET;
    private int _amPm = _UNSET;
    private int _hour = _UNSET;
    private int _minute = _UNSET;
    private int _second = _UNSET;
    private int _fraction = _UNSET;
    private int _utcOffsetHour = _UNSET;
    private int _utcOffsetMinute = _UNSET;

    public void SetEra(string era)
    {
        var eraNum = era.ToUpper() switch
        {
            "BC" => 1,
            "AD" => 2,
            _ => throw new InvalidDateException(DERA02, "Unknown era found")
        };
        SetField(ref _era, eraNum);
    }
    
    public void SetYear(int year)
    {
        if(year is < 1 or > 9999) throw new InvalidDateException(DYAR03, 
            "Invalid date year out of range");
        year = year < 100 ? ToFourDigitYear(year) : year;
        SetField(ref _year, year);
    }
    
    public void SetMonth(string month)
    {
        var monthNum = _Months[month.ToLower()];
        SetField(ref _month, monthNum);
    }
    
    public void SetMonth(int month)
    {
        if(month is < 1 or > 12) throw new InvalidDateException(DMON05, 
            "Invalid date month out of range");
        SetField(ref _month, month);
    }
    
    public void SetWeekday(string weekday)
    {
        var dayOfWeek = _Weekdays[weekday.ToLower()];
        SetField(ref _weekday, dayOfWeek);
    }
    
    public void SetDay(int day)
    {
        if(day is < 1 or > 31) throw new InvalidDateException(DDAY04,
            "Invalid date day out of range");
        SetField(ref _day, day);
    }
    
    public void SetAmPm(string amPm)
    {
        var amPmNum = amPm.ToLower() switch
        {
            "am" => 1,
            "pm" => 2,
            _ => throw new InvalidDateException(DTAP02, 
                "Unknown time period found")
        };
        if(_hour != _UNSET && _hour is < 1 or > 12)
            throw new InvalidDateException(DHUR03, 
                "Invalid date hour AM/PM out of range");
        SetField(ref _amPm, amPmNum);
    }
    
    public void SetHour(int hour)
    {
        if(_amPm != _UNSET && _hour is < 1 or > 12)
            throw new InvalidDateException(DHUR04, 
                "Invalid date hour AM/PM out of range");
        if(hour is < 0 or > 23)
            throw new InvalidDateException(DHUR06,
                "Invalid date hour out of range");
        SetField(ref _hour, hour);
    }
    
    public void SetMinute(int minute)
    {
        if(minute is < 0 or > 59) throw new InvalidDateException(DMIN03,
            "Invalid date minute out of range");
        SetField(ref _minute, minute);
    }
    
    public void SetSecond(int second)
    {
        if(second is < 0 or > 59) throw new InvalidDateException(DSEC03,
            "Invalid date second out of range");
        SetField(ref _second, second);
    }
    
    public void SetFraction(int fraction) => SetField(ref _fraction, fraction);

    public void SetUtcOffset(int hour, int minute)
    {
        if(hour is < -12 or > 12) throw new InvalidDateException(DUTC04,
            "Invalid date UTC offset hour out of range");
        if(minute is < 0 or > 59) throw new InvalidDateException(DUTC05,
                "Invalid date UTC offset minute out of range");
        SetField(ref _utcOffsetHour, hour);
        SetField(ref _utcOffsetMinute, minute);
    }
    
    private static void SetField(ref int field, int value)
    {
        if(field != _UNSET && field != value) 
            throw new InvalidDateException(DCNF01, 
                "Conflicting date segments");
        field = value;
    }
    
    private static void SetField(ref DayOfWeek field, DayOfWeek value)
    {
        if(field != (DayOfWeek) _UNSET && field != value) 
            throw new InvalidDateException(DCNF02, 
                "Conflicting date segments");
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
                    throw new InvalidDateException(DDAY03,
                        "Invalid date day out of range");
                dateTime = new DateTime(_year, _month, _day);
                if(_weekday != (DayOfWeek) _UNSET && dateTime.DayOfWeek != _weekday)
                    throw new InvalidDateException(DWKD03, "Invalid date weekday");
            }

            if(IsAllSet(_year, _month)) dateTime = new DateTime(_year, _month, 1);
            if(IsAllSet(_year)) dateTime = new DateTime(_year, 1, 1);
        }
        catch(InvalidDateException) { throw; }
        catch(Exception ex)
        {
            throw new InvalidDateException(DINV01,
                "Invalid date year, month or day out of range", ex);
        }

        if(IsAllSet(_hour, _amPm)) ConvertTo24Hour();
        if(_hour != _UNSET && _hour is < 0 or > 23)
            throw new InvalidDateException(DHUR05,
            "Invalid date hour out of range");
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
        if(_weekday != (DayOfWeek) _UNSET) builder.Append($"Weekday: {_weekday}, ");
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