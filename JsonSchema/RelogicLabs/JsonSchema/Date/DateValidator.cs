using Antlr4.Runtime;
using RelogicLabs.JsonSchema.Antlr;
using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Utilities;

namespace RelogicLabs.JsonSchema.Date;

internal class DateValidator
{
    public const string ISO_8601 = "YYYY-MM-DD'T'hh:mm:ss.fffZZ";

    private static readonly Dictionary<string, SegmentProcessor> _Processors = new();
    private readonly DateLexer _dateLexer;
    private readonly IList<IToken> _lexerTokens;

    static DateValidator()
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

    public DateValidator(string pattern)
    {
        if(pattern == "ISO-8601") pattern = ISO_8601;
        _dateLexer = new DateLexer(CharStreams.fromString(pattern));
        _dateLexer.RemoveErrorListeners();
        _dateLexer.AddErrorListener(LexerErrorListener.Date);
        _lexerTokens = _dateLexer.GetAllTokens();
    }
    
    public void Validate(string input)
    {
        var _dateContext = new DateContext();
        _dateLexer.Reset();
        foreach(var token in _lexerTokens)
        {
            var processor = _Processors[_dateLexer.Vocabulary.GetSymbolicName(token.Type)];
            input = processor.Process(input, token, _dateContext);
        }
        if(input.Length != 0) throw new InvalidDateException(ErrorCode.DINV02, 
            "Invalid date input format");
        
        _dateContext.Validate();
        DebugUtils.Print(_dateContext);
    }

    public bool IsDate(string input)
    {
        try
        {
            Validate(input);
            return true;
        }
        catch(InvalidDateException ex)
        {
            Console.Error.WriteLine(ex);
            return false;
        }
    }
}