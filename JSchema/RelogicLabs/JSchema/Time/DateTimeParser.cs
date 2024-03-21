using Antlr4.Runtime;
using RelogicLabs.JSchema.Antlr;
using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Message;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Antlr.DateTimeLexer;
using static RelogicLabs.JSchema.Time.SegmentProcessor;

namespace RelogicLabs.JSchema.Time;

internal sealed class DateTimeParser
{
    private static readonly Dictionary<string, SegmentProcessor> _Processors = new();
    private readonly DateTimeLexer _dateTimeLexer;
    private readonly IList<IToken> _lexerTokens;

    public string Pattern { get; }
    public DateTimeType Type { get; }

    static DateTimeParser()
    {
        AddProcessor(TEXT, Text);
        AddProcessor(SYMBOL, Symbol);
        AddProcessor(WHITESPACE, Whitespace);
        AddProcessor(ERA, Era);
        AddProcessor(YEAR_NUMBER4, YearNumber4);
        AddProcessor(YEAR_NUMBER2, YearNumber2);
        AddProcessor(MONTH_NAME, MonthName);
        AddProcessor(MONTH_SHORT_NAME, MonthShortName);
        AddProcessor(MONTH_NUMBER2, MonthNumber2);
        AddProcessor(MONTH_NUMBER, MonthNumber);
        AddProcessor(WEEKDAY_NAME, WeekdayName);
        AddProcessor(WEEKDAY_SHORT_NAME, WeekdayShortName);
        AddProcessor(DAY_NUMBER2, DayNumber2);
        AddProcessor(DAY_NUMBER, DayNumber);
        AddProcessor(CLOCK_AM_PM, ClockAmPm);
        AddProcessor(HOUR_NUMBER2, HourNumber2);
        AddProcessor(HOUR_NUMBER, HourNumber);
        AddProcessor(MINUTE_NUMBER2, MinuteNumber2);
        AddProcessor(MINUTE_NUMBER, MinuteNumber);
        AddProcessor(SECOND_NUMBER2, SecondNumber2);
        AddProcessor(SECOND_NUMBER, SecondNumber);
        AddProcessor(FRACTION_NUMBER, FractionNumber);
        AddProcessor(FRACTION_NUMBER1, FractionNumber1);
        AddProcessor(FRACTION_NUMBER2, FractionNumber2);
        AddProcessor(FRACTION_NUMBER3, FractionNumber3);
        AddProcessor(FRACTION_NUMBER4, FractionNumber4);
        AddProcessor(FRACTION_NUMBER5, FractionNumber5);
        AddProcessor(FRACTION_NUMBER6, FractionNumber6);
        AddProcessor(UTC_OFFSET_HOUR, UtcOffsetHour);
        AddProcessor(UTC_OFFSET_TIME1, UtcOffsetTime1);
        AddProcessor(UTC_OFFSET_TIME2, UtcOffsetTime2);
    }

    private static void AddProcessor(int index, SegmentProcessor processor)
        => _Processors.Add(ruleNames[index - 1], processor);

    public DateTimeParser(string pattern, DateTimeType type)
    {
        Pattern = pattern;
        Type = type;
        _dateTimeLexer = new DateTimeLexer(CharStreams.fromString(pattern));
        _dateTimeLexer.RemoveErrorListeners();
        _dateTimeLexer.AddErrorListener(LexerErrorListener.DateTime);
        _lexerTokens = _dateTimeLexer.GetAllTokens();
    }

    private JsonDateTime Parse(string input, DateTimeContext context)
    {
        foreach(var token in _lexerTokens)
        {
            var processor = _Processors[_dateTimeLexer.Vocabulary.GetSymbolicName(token.Type)];
            input = processor.Process(input, token, context);
        }
        if(input.Length != 0) throw new InvalidDateTimeException(ErrorCode.DINV02,
            $"Invalid {context.Type} input format");

        var dateTime = context.Validate();
        LogHelper.Debug(context);
        return dateTime;
    }

    public JsonDateTime Parse(string input)
        => Parse(input, new DateTimeContext(Type));

    public JsonDateTime? TryParse(string input, out string error)
    {
        error = string.Empty;
        try
        {
            return Parse(input);
        }
        catch(InvalidDateTimeException ex)
        {
            LogHelper.Debug(ex);
            error = ex.Message;
            return null;
        }
    }
}