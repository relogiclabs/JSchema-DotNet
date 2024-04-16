using System.Globalization;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using RelogicLabs.JSchema.Exceptions;
using static RelogicLabs.JSchema.Message.ErrorCode;

namespace RelogicLabs.JSchema.Time;

internal abstract class SegmentProcessor
{
    private static readonly Regex EraRegex = new("^(BC|AD)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex YearNumber4Regex = new(@"^(\d{4})", RegexOptions.Compiled);
    private static readonly Regex YearNumber2Regex = new(@"^(\d{2})", RegexOptions.Compiled);

    private static readonly Regex MonthNameRegex = new(
            "^(January|February|March|April|May|June|July"
            + "|August|September|October|November|December)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex MonthShortNameRegex = new(
            "^(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex MonthNumber2Regex = new(@"^(\d{2})", RegexOptions.Compiled);
    private static readonly Regex MonthNumberRegex = new(@"^(\d{1,2})", RegexOptions.Compiled);

    private static readonly Regex WeekdayNameRegex = new(
            "^(Sunday|Monday|Tuesday|Wednesday|Thursday|Friday|Saturday)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex WeekdayShortNameRegex = new("^(Sun|Mon|Tue|Wed|Thu|Fri|Sat)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex DayNumber2Regex = new(@"^(\d{2})", RegexOptions.Compiled);
    private static readonly Regex DayNumberRegex = new(@"^(\d{1,2})", RegexOptions.Compiled);

    private static readonly Regex ClockAmPmRegex = new("^(AM|PM)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex HourNumber2Regex = new(@"^(\d{2})", RegexOptions.Compiled);
    private static readonly Regex HourNumberRegex = new(@"^(\d{1,2})", RegexOptions.Compiled);

    private static readonly Regex MinuteNumber2Regex = new(@"^(\d{2})", RegexOptions.Compiled);
    private static readonly Regex MinuteNumberRegex = new(@"^(\d{1,2})", RegexOptions.Compiled);

    private static readonly Regex SecondNumber2Regex = new(@"^(\d{2})", RegexOptions.Compiled);
    private static readonly Regex SecondNumberRegex = new(@"^(\d{1,2})", RegexOptions.Compiled);

    private static readonly Regex FractionNumberRegex = new(@"^(\d{1,6})", RegexOptions.Compiled);
    private static readonly Regex FractionNumber1Regex = new(@"^(\d)", RegexOptions.Compiled);
    private static readonly Regex FractionNumber2Regex = new(@"^(\d{2})", RegexOptions.Compiled);
    private static readonly Regex FractionNumber3Regex = new(@"^(\d{3})", RegexOptions.Compiled);
    private static readonly Regex FractionNumber4Regex = new(@"^(\d{4})", RegexOptions.Compiled);
    private static readonly Regex FractionNumber5Regex = new(@"^(\d{5})", RegexOptions.Compiled);
    private static readonly Regex FractionNumber6Regex = new(@"^(\d{6})", RegexOptions.Compiled);

    private static readonly Regex UtcOffsetHourRegex = new(@"^([+-]\d{2}|Z)", RegexOptions.Compiled);
    private static readonly Regex UtcOffsetTime1Regex = new(@"^([+-]\d{2}):(\d{2})|Z", RegexOptions.Compiled);
    private static readonly Regex UtcOffsetTime2Regex = new(@"^([+-]\d{2})(\d{2})|Z", RegexOptions.Compiled);

    public static readonly SegmentProcessor Text = new TextProcessor();
    public static readonly SegmentProcessor Symbol = new SymbolProcessor();
    public static readonly SegmentProcessor Whitespace = new WhitespaceProcessor();

    public static readonly SegmentProcessor Era = new EraProcessor(EraRegex, DERA01);
    public static readonly SegmentProcessor YearNumber4
            = new YearNumberProcessor(YearNumber4Regex, DYAR01);
    public static readonly SegmentProcessor YearNumber2
            = new YearNumberProcessor(YearNumber2Regex, DYAR02);

    public static readonly SegmentProcessor MonthName = new MonthNameProcessor(MonthNameRegex, DMON01);
    public static readonly SegmentProcessor MonthShortName = new MonthNameProcessor(MonthShortNameRegex, DMON02);
    public static readonly SegmentProcessor MonthNumber2 = new MonthNumberProcessor(MonthNumber2Regex, DMON03);
    public static readonly SegmentProcessor MonthNumber = new MonthNumberProcessor(MonthNumberRegex, DMON04);

    public static readonly SegmentProcessor WeekdayName = new WeekdayProcessor(WeekdayNameRegex, DWKD01);
    public static readonly SegmentProcessor WeekdayShortName = new WeekdayProcessor(WeekdayShortNameRegex, DWKD02);
    public static readonly SegmentProcessor DayNumber2 = new DayNumberProcessor(DayNumber2Regex, DDAY01);
    public static readonly SegmentProcessor DayNumber = new DayNumberProcessor(DayNumberRegex, DDAY02);

    public static readonly SegmentProcessor ClockAmPm = new ClockAmPmProcessor(ClockAmPmRegex, DTAP01);
    public static readonly SegmentProcessor HourNumber2 = new HourNumberProcessor(HourNumber2Regex, DHUR01);
    public static readonly SegmentProcessor HourNumber = new HourNumberProcessor(HourNumberRegex, DHUR02);

    public static readonly SegmentProcessor MinuteNumber2 = new MinuteNumberProcessor(MinuteNumber2Regex, DMIN01);
    public static readonly SegmentProcessor MinuteNumber = new MinuteNumberProcessor(MinuteNumberRegex, DMIN02);

    public static readonly SegmentProcessor SecondNumber2 = new SecondNumberProcessor(SecondNumber2Regex, DSEC01);
    public static readonly SegmentProcessor SecondNumber = new SecondNumberProcessor(SecondNumberRegex, DSEC02);

    public static readonly SegmentProcessor FractionNumber = new FractionNumberProcessor(FractionNumberRegex, DFRC01);
    public static readonly SegmentProcessor FractionNumber1 = new FractionNumberProcessor(FractionNumber1Regex, DFRC02);
    public static readonly SegmentProcessor FractionNumber2 = new FractionNumberProcessor(FractionNumber2Regex, DFRC03);
    public static readonly SegmentProcessor FractionNumber3 = new FractionNumberProcessor(FractionNumber3Regex, DFRC04);
    public static readonly SegmentProcessor FractionNumber4 = new FractionNumberProcessor(FractionNumber4Regex, DFRC05);
    public static readonly SegmentProcessor FractionNumber5 = new FractionNumberProcessor(FractionNumber5Regex, DFRC06);
    public static readonly SegmentProcessor FractionNumber6 = new FractionNumberProcessor(FractionNumber6Regex, DFRC07);

    public static readonly SegmentProcessor UtcOffsetHour = new UtcOffsetHourProcessor(UtcOffsetHourRegex, DUTC01);
    public static readonly SegmentProcessor UtcOffsetTime1 = new UtcOffsetTimeProcessor(UtcOffsetTime1Regex, DUTC02);
    public static readonly SegmentProcessor UtcOffsetTime2 = new UtcOffsetTimeProcessor(UtcOffsetTime2Regex, DUTC03);

    public abstract string Process(string input, IToken token, DateTimeContext context);


    private sealed class TextProcessor : SegmentProcessor
    {
        public override string Process(string input, IToken token, DateTimeContext context)
        {
            var _token = token.Text[1..^1].Replace("''", "'");
            if(!input.StartsWith(_token)) throw FailOnInvalidDateTime(context, DTXT01,
                "text mismatch or input format");
            return input[_token.Length..];
        }
    }

    private sealed class SymbolProcessor : SegmentProcessor
    {
        public override string Process(string input, IToken token, DateTimeContext context)
        {
            if(!input.StartsWith(token.Text)) throw FailOnInvalidDateTime(context, DSYM01,
                "symbol mismatch or input format");
            return input[token.Text.Length..];
        }
    }

    private sealed class WhitespaceProcessor : SegmentProcessor
    {
        public override string Process(string input, IToken token, DateTimeContext context)
        {
            if(!input.StartsWith(token.Text)) throw FailOnInvalidDateTime(context, DWTS01,
                "whitespace mismatch or input format");
            return input[token.Text.Length..];
        }
    }

    private abstract class SegmentRegexProcessor : SegmentProcessor
    {
        protected readonly string _code;
        private readonly Regex _regex;

        protected SegmentRegexProcessor(Regex regex, string code)
        {
            _regex = regex;
            _code = code;
        }

        public override string Process(string input, IToken token, DateTimeContext context)
        {
            var match = _regex.Match(input);
            Process(match, context);
            return input[match.Length..];
        }

        protected abstract void Process(Match match, DateTimeContext context);
    }

    private sealed class EraProcessor : SegmentRegexProcessor
    {
        public EraProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw FailOnInvalidDateTime(context, _code, "era input");
            context.SetEra(match.Groups[1].Value);
        }
    }

    private sealed class YearNumberProcessor : SegmentRegexProcessor
    {
        public YearNumberProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw FailOnInvalidDateTime(context, _code, "year input");
            var year = match.Groups[1].Value;
            context.SetYear(int.Parse(year, NumberStyles.Integer), year.Length);
        }
    }

    private sealed class MonthNameProcessor : SegmentRegexProcessor
    {
        public MonthNameProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw FailOnInvalidDateTime(context, _code, "month name input");
            context.SetMonth(match.Groups[1].Value);
        }
    }

    private sealed class MonthNumberProcessor : SegmentRegexProcessor
    {
        public MonthNumberProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw FailOnInvalidDateTime(context, _code, "month number input");
            context.SetMonth(int.Parse(match.Groups[1].Value, NumberStyles.Integer));
        }
    }

    private sealed class WeekdayProcessor : SegmentRegexProcessor
    {
        public WeekdayProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw FailOnInvalidDateTime(context, _code, "weekday input");
            context.SetWeekday(match.Groups[1].Value);
        }
    }

    private sealed class DayNumberProcessor : SegmentRegexProcessor
    {
        public DayNumberProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw FailOnInvalidDateTime(context, _code, "day input");
            context.SetDay(int.Parse(match.Groups[1].Value, NumberStyles.Integer));
        }
    }

    private sealed class ClockAmPmProcessor : SegmentRegexProcessor
    {
        public ClockAmPmProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw FailOnInvalidDateTime(context, _code, "AM/PM input");
            context.SetAmPm(match.Groups[1].Value);
        }
    }

    private sealed class HourNumberProcessor : SegmentRegexProcessor
    {
        public HourNumberProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw FailOnInvalidDateTime(context, _code, "hour input");
            context.SetHour(int.Parse(match.Groups[1].Value, NumberStyles.Integer));
        }
    }

    private sealed class MinuteNumberProcessor : SegmentRegexProcessor
    {
        public MinuteNumberProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw FailOnInvalidDateTime(context, _code, "minute input");
            context.SetMinute(int.Parse(match.Groups[1].Value, NumberStyles.Integer));
        }
    }

    private sealed class SecondNumberProcessor : SegmentRegexProcessor
    {
        public SecondNumberProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw FailOnInvalidDateTime(context, _code, "second input");
            context.SetSecond(int.Parse(match.Groups[1].Value, NumberStyles.Integer));
        }
    }

    private sealed class FractionNumberProcessor : SegmentRegexProcessor
    {
        public FractionNumberProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw FailOnInvalidDateTime(context, _code, "second fraction input");
            context.SetFraction(int.Parse(match.Groups[1].Value, NumberStyles.Integer));
        }
    }

    private sealed class UtcOffsetHourProcessor : SegmentRegexProcessor
    {
        public UtcOffsetHourProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw FailOnInvalidDateTime(context, _code, "UTC offset hour input");
            context.SetUtcOffset(match.Value == "Z"? 0 : int.Parse(match.Groups[1].Value,
                NumberStyles.Integer), 0);
        }
    }

    private sealed class UtcOffsetTimeProcessor : SegmentRegexProcessor
    {
        public UtcOffsetTimeProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw FailOnInvalidDateTime(context, _code, "UTC offset input");
            if(match.Value == "Z") context.SetUtcOffset(0, 0);
            else context.SetUtcOffset(int.Parse(match.Groups[1].Value, NumberStyles.Integer),
                int.Parse(match.Groups[2].Value, NumberStyles.Integer));
        }
    }

    private static InvalidDateTimeException FailOnInvalidDateTime(DateTimeContext context,
                string code, string message)
        => new(code, $"Invalid {context.Type} {message}");
}