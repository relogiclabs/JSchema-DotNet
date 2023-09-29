using Antlr4.Runtime;
using RelogicLabs.JsonSchema.Antlr;
using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Utilities;

namespace RelogicLabs.JsonSchema.Time;

internal class DateTimeValidator
{
    public const string ISO_8601_DATE = "YYYY-MM-DD";
    public const string ISO_8601_TIME = "YYYY-MM-DD'T'hh:mm:ss.fffZZ";

    private static readonly Dictionary<string, SegmentProcessor> _Processors = new();
    private readonly DateTimeLexer _dateTimeLexer;
    private readonly IList<IToken> _lexerTokens;

    static DateTimeValidator()
    {
        _Processors.Add("TEXT", SegmentProcessor.Text);
        _Processors.Add("SYMBOL", SegmentProcessor.Symbol);
        _Processors.Add("WHITESPACE", SegmentProcessor.Whitespace);
        _Processors.Add("ERA", SegmentProcessor.Era);
        _Processors.Add("YEAR_NUM4", SegmentProcessor.YearNum4);
        _Processors.Add("YEAR_NUM2", SegmentProcessor.YearNum2);
        _Processors.Add("MONTH_NAME", SegmentProcessor.MonthName);
        _Processors.Add("MONTH_SHORT_NAME", SegmentProcessor.MonthShortName);
        _Processors.Add("MONTH_NUM2", SegmentProcessor.MonthNum2);
        _Processors.Add("MONTH_NUM", SegmentProcessor.MonthNum);
        _Processors.Add("WEEKDAY_NAME", SegmentProcessor.WeekdayName);
        _Processors.Add("WEEKDAY_SHORT_NAME", SegmentProcessor.WeekdayShortName);
        _Processors.Add("DAY_NUM2", SegmentProcessor.DayNum2);
        _Processors.Add("DAY_NUM", SegmentProcessor.DayNum);
        _Processors.Add("AM_PM", SegmentProcessor.AmPm);
        _Processors.Add("HOUR_NUM2", SegmentProcessor.HourNum2);
        _Processors.Add("HOUR_NUM", SegmentProcessor.HourNum);
        _Processors.Add("MINUTE_NUM2", SegmentProcessor.MinuteNum2);
        _Processors.Add("MINUTE_NUM", SegmentProcessor.MinuteNum);
        _Processors.Add("SECOND_NUM2", SegmentProcessor.SecondNum2);
        _Processors.Add("SECOND_NUM", SegmentProcessor.SecondNum);
        _Processors.Add("FRACTION_NUM", SegmentProcessor.FractionNum);
        _Processors.Add("FRACTION_NUM01", SegmentProcessor.FractionNum01);
        _Processors.Add("FRACTION_NUM02", SegmentProcessor.FractionNum02);
        _Processors.Add("FRACTION_NUM03", SegmentProcessor.FractionNum03);
        _Processors.Add("FRACTION_NUM04", SegmentProcessor.FractionNum04);
        _Processors.Add("FRACTION_NUM05", SegmentProcessor.FractionNum05);
        _Processors.Add("FRACTION_NUM06", SegmentProcessor.FractionNum06);
        _Processors.Add("UTC_OFFSET_HOUR", SegmentProcessor.UtcOffsetHour);
        _Processors.Add("UTC_OFFSET_TIME1", SegmentProcessor.UtcOffsetTime1);
        _Processors.Add("UTC_OFFSET_TIME2", SegmentProcessor.UtcOffsetTime2);
    }
    
    public DateTimeValidator(string pattern)
    {
        _dateTimeLexer = new DateTimeLexer(CharStreams.fromString(pattern));
        _dateTimeLexer.RemoveErrorListeners();
        _dateTimeLexer.AddErrorListener(LexerErrorListener.DateTime);
        _lexerTokens = _dateTimeLexer.GetAllTokens();
    }

    private void Validate(string input, DateTimeContext context)
    {
        foreach(var token in _lexerTokens)
        {
            var processor = _Processors[_dateTimeLexer.Vocabulary.GetSymbolicName(token.Type)];
            input = processor.Process(input, token, context);
        }
        if(input.Length != 0) throw new InvalidDateTimeException(ErrorCode.DINV02, 
            $"Invalid {context.Type} input format");
        
        context.Validate();
        DebugUtils.Print(context);
    }
    
    public void ValidateDate(string input) 
        => Validate(input, new DateTimeContext(DateTimeType.DATE_TYPE));

    public void ValidateTime(string input) 
        => Validate(input, new DateTimeContext(DateTimeType.TIME_TYPE));

    public bool IsValidDate(string input)
    {
        try
        {
            ValidateDate(input);
            return true;
        }
        catch(InvalidDateTimeException ex)
        {
            DebugUtils.Print(ex);
            return false;
        }
    }

    public bool IsValidTime(string input)
    {
        try
        {
            ValidateTime(input);
            return true;
        }
        catch(InvalidDateTimeException ex)
        {
            DebugUtils.Print(ex);
            return false;
        }
    }
}