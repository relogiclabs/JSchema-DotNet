using System.Globalization;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using RelogicLabs.JsonSchema.Exceptions;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Time;

internal abstract partial class SegmentProcessor
{
    public static readonly SegmentProcessor Text = new TextProcessor();
    public static readonly SegmentProcessor Symbol = new SymbolProcessor();
    public static readonly SegmentProcessor Whitespace = new WhitespaceProcessor();

    public static readonly SegmentProcessor Era = new EraProcessor(EraRegex(), DERA01);
    public static readonly SegmentProcessor YearNum4 = new YearNumProcessor(YearNum4Regex(), DYAR01);
    public static readonly SegmentProcessor YearNum2 = new YearNumProcessor(YearNum2Regex(), DYAR02);

    public static readonly SegmentProcessor MonthName = new MonthNameProcessor(MonthNameRegex(), DMON01);
    public static readonly SegmentProcessor MonthShortName = new MonthNameProcessor(MonthShortNameRegex(), DMON02);
    public static readonly SegmentProcessor MonthNum2 = new MonthNumProcessor(MonthNum2Regex(), DMON03);
    public static readonly SegmentProcessor MonthNum = new MonthNumProcessor(MonthNumRegex(), DMON04);

    public static readonly SegmentProcessor WeekdayName = new WeekdayProcessor(WeekdayNameRegex(), DWKD01);
    public static readonly SegmentProcessor WeekdayShortName = new WeekdayProcessor(WeekdayShortNameRegex(), DWKD02);
    public static readonly SegmentProcessor DayNum2 = new DayNumProcessor(DayNum2Regex(), DDAY01);
    public static readonly SegmentProcessor DayNum = new DayNumProcessor(DayNumRegex(), DDAY02);

    public static readonly SegmentProcessor AmPm = new AmPmProcessor(AmPmRegex(), DTAP01);
    public static readonly SegmentProcessor HourNum2 = new HourNumProcessor(HourNum2Regex(), DHUR01);
    public static readonly SegmentProcessor HourNum = new HourNumProcessor(HourNumRegex(), DHUR02);

    public static readonly SegmentProcessor MinuteNum2 = new MinuteNumProcessor(MinuteNum2Regex(), DMIN01);
    public static readonly SegmentProcessor MinuteNum = new MinuteNumProcessor(MinuteNumRegex(), DMIN02);

    public static readonly SegmentProcessor SecondNum2 = new SecondNumProcessor(SecondNum2Regex(), DSEC01);
    public static readonly SegmentProcessor SecondNum = new SecondNumProcessor(SecondNumRegex(), DSEC02);

    public static readonly SegmentProcessor FractionNum = new FractionNumProcessor(FractionNumRegex(), DFRC01);
    public static readonly SegmentProcessor FractionNum01 = new FractionNumProcessor(FractionNum01Regex(), DFRC02);
    public static readonly SegmentProcessor FractionNum02 = new FractionNumProcessor(FractionNum02Regex(), DFRC03);
    public static readonly SegmentProcessor FractionNum03 = new FractionNumProcessor(FractionNum03Regex(), DFRC04);
    public static readonly SegmentProcessor FractionNum04 = new FractionNumProcessor(FractionNum04Regex(), DFRC05);
    public static readonly SegmentProcessor FractionNum05 = new FractionNumProcessor(FractionNum05Regex(), DFRC06);
    public static readonly SegmentProcessor FractionNum06 = new FractionNumProcessor(FractionNum06Regex(), DFRC07);

    public static readonly SegmentProcessor UtcOffsetHour = new UtcOffsetHourProcessor(UtcOffsetHourRegex(), DUTC01);
    public static readonly SegmentProcessor UtcOffsetTime1 = new UtcOffsetTimeProcessor(UtcOffsetTime1Regex(), DUTC02);
    public static readonly SegmentProcessor UtcOffsetTime2 = new UtcOffsetTimeProcessor(UtcOffsetTime2Regex(), DUTC03);

    public abstract string Process(string input, IToken token, DateTimeContext context);


    private class TextProcessor : SegmentProcessor
    {
        public override string Process(string input, IToken token, DateTimeContext context)
        {
            var _token = token.Text[1..^1].Replace("''", "'");
            if(!input.StartsWith(_token)) throw new InvalidDateTimeException(DTXT01,
                $"Invalid {context.Type} text mismatch or input format");
            return input[_token.Length..];
        }
    }

    private class SymbolProcessor : SegmentProcessor
    {
        public override string Process(string input, IToken token, DateTimeContext context)
        {
            if(!input.StartsWith(token.Text)) throw new InvalidDateTimeException(DSYM01,
                $"Invalid {context.Type} symbol mismatch or input format");
            return input[token.Text.Length..];
        }
    }

    private class WhitespaceProcessor : SegmentProcessor
    {
        public override string Process(string input, IToken token, DateTimeContext context)
        {
            if(!input.StartsWith(token.Text)) throw new InvalidDateTimeException(DWTS01,
                $"Invalid {context.Type} whitespace mismatch or input format");
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

    private class EraProcessor : SegmentRegexProcessor
    {
        public EraProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw new InvalidDateTimeException(_code,
                $"Invalid {context.Type} era input");
            context.SetEra(match.Groups[1].Value);
        }
    }

    private class YearNumProcessor : SegmentRegexProcessor
    {
        public YearNumProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw new InvalidDateTimeException(_code,
                $"Invalid {context.Type} year input");
            var year = match.Groups[1].Value;
            context.SetYear(int.Parse(year, NumberStyles.Integer), year.Length);
        }
    }

    private class MonthNameProcessor : SegmentRegexProcessor
    {
        public MonthNameProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw new InvalidDateTimeException(_code,
                $"Invalid {context.Type} month name input");
            context.SetMonth(match.Groups[1].Value);
        }
    }

    private class MonthNumProcessor : SegmentRegexProcessor
    {
        public MonthNumProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw new InvalidDateTimeException(_code,
                $"Invalid {context.Type} month number input");
            context.SetMonth(int.Parse(match.Groups[1].Value, NumberStyles.Integer));
        }
    }

    private class WeekdayProcessor : SegmentRegexProcessor
    {
        public WeekdayProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw new InvalidDateTimeException(_code,
                $"Invalid {context.Type} weekday input");
            context.SetWeekday(match.Groups[1].Value);
        }
    }

    private class DayNumProcessor : SegmentRegexProcessor
    {
        public DayNumProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw new InvalidDateTimeException(_code,
                $"Invalid {context.Type} day input");
            context.SetDay(int.Parse(match.Groups[1].Value, NumberStyles.Integer));
        }
    }

    private class AmPmProcessor : SegmentRegexProcessor
    {
        public AmPmProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw new InvalidDateTimeException(_code,
                $"Invalid {context.Type} AM/PM input");
            context.SetAmPm(match.Groups[1].Value);
        }
    }

    private class HourNumProcessor : SegmentRegexProcessor
    {
        public HourNumProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw new InvalidDateTimeException(_code,
                $"Invalid {context.Type} hour input");
            context.SetHour(int.Parse(match.Groups[1].Value, NumberStyles.Integer));
        }
    }

    private class MinuteNumProcessor : SegmentRegexProcessor
    {
        public MinuteNumProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw new InvalidDateTimeException(_code,
                $"Invalid {context.Type} minute input");
            context.SetMinute(int.Parse(match.Groups[1].Value, NumberStyles.Integer));
        }
    }

    private class SecondNumProcessor : SegmentRegexProcessor
    {
        public SecondNumProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw new InvalidDateTimeException(_code,
                $"Invalid {context.Type} second input");
            context.SetSecond(int.Parse(match.Groups[1].Value, NumberStyles.Integer));
        }
    }

    private class FractionNumProcessor : SegmentRegexProcessor
    {
        public FractionNumProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw new InvalidDateTimeException(_code,
                $"Invalid {context.Type} second faction input");
            context.SetFraction(int.Parse(match.Groups[1].Value, NumberStyles.Integer));
        }
    }

    private class UtcOffsetHourProcessor : SegmentRegexProcessor
    {
        public UtcOffsetHourProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw new InvalidDateTimeException(_code,
                $"Invalid {context.Type} UTC offset hour input");
            context.SetUtcOffset(match.Value == "Z"? 0 : int.Parse(match.Groups[1].Value,
                NumberStyles.Integer), 0);
        }
    }

    private class UtcOffsetTimeProcessor : SegmentRegexProcessor
    {
        public UtcOffsetTimeProcessor(Regex regex, string code) : base(regex, code) { }
        protected override void Process(Match match, DateTimeContext context)
        {
            if(!match.Success) throw new InvalidDateTimeException(_code,
                $"Invalid {context.Type} UTC offset input");
            if(match.Value == "Z") context.SetUtcOffset(0, 0);
            else context.SetUtcOffset(int.Parse(match.Groups[1].Value, NumberStyles.Integer),
                int.Parse(match.Groups[2].Value, NumberStyles.Integer));
        }
    }

    [GeneratedRegex("^(BC|AD)", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex EraRegex();

    [GeneratedRegex(@"^(\d{4})", RegexOptions.Compiled)]
    private static partial Regex YearNum4Regex();

    [GeneratedRegex(@"^(\d{2})", RegexOptions.Compiled)]
    private static partial Regex YearNum2Regex();

    [GeneratedRegex("^(January|February|March|April|May|June|July" +
                    "|August|September|October|November|December)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex MonthNameRegex();

    [GeneratedRegex("^(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex MonthShortNameRegex();

    [GeneratedRegex(@"^(\d{2})", RegexOptions.Compiled)]
    private static partial Regex MonthNum2Regex();

    [GeneratedRegex(@"^(\d{1,2})", RegexOptions.Compiled)]
    private static partial Regex MonthNumRegex();

    [GeneratedRegex("^(Sunday|Monday|Tuesday|Wednesday|Thursday|Friday|Saturday)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex WeekdayNameRegex();

    [GeneratedRegex("^(Sun|Mon|Tue|Wed|Thu|Fri|Sat)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex WeekdayShortNameRegex();

    [GeneratedRegex(@"^(\d{2})", RegexOptions.Compiled)]
    private static partial Regex DayNum2Regex();

    [GeneratedRegex(@"^(\d{1,2})", RegexOptions.Compiled)]
    private static partial Regex DayNumRegex();

    [GeneratedRegex("^(AM|PM)", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex AmPmRegex();

    [GeneratedRegex(@"^(\d{2})", RegexOptions.Compiled)]
    private static partial Regex HourNum2Regex();

    [GeneratedRegex(@"^(\d{1,2})", RegexOptions.Compiled)]
    private static partial Regex HourNumRegex();

    [GeneratedRegex(@"^(\d{2})", RegexOptions.Compiled)]
    private static partial Regex MinuteNum2Regex();

    [GeneratedRegex(@"^(\d{1,2})", RegexOptions.Compiled)]
    private static partial Regex MinuteNumRegex();

    [GeneratedRegex(@"^(\d{2})", RegexOptions.Compiled)]
    private static partial Regex SecondNum2Regex();

    [GeneratedRegex(@"^(\d{1,2})", RegexOptions.Compiled)]
    private static partial Regex SecondNumRegex();

    [GeneratedRegex(@"^(\d{1,6})", RegexOptions.Compiled)]
    private static partial Regex FractionNumRegex();

    [GeneratedRegex(@"^(\d)", RegexOptions.Compiled)]
    private static partial Regex FractionNum01Regex();

    [GeneratedRegex(@"^(\d{2})", RegexOptions.Compiled)]
    private static partial Regex FractionNum02Regex();

    [GeneratedRegex(@"^(\d{3})", RegexOptions.Compiled)]
    private static partial Regex FractionNum03Regex();

    [GeneratedRegex(@"^(\d{4})", RegexOptions.Compiled)]
    private static partial Regex FractionNum04Regex();

    [GeneratedRegex(@"^(\d{5})", RegexOptions.Compiled)]
    private static partial Regex FractionNum05Regex();

    [GeneratedRegex(@"^(\d{6})", RegexOptions.Compiled)]
    private static partial Regex FractionNum06Regex();

    [GeneratedRegex(@"^([+-]\d{2}|Z)", RegexOptions.Compiled)]
    private static partial Regex UtcOffsetHourRegex();

    [GeneratedRegex(@"^([+-]\d{2}):(\d{2})|Z", RegexOptions.Compiled)]
    private static partial Regex UtcOffsetTime1Regex();

    [GeneratedRegex(@"^([+-]\d{2})(\d{2})|Z", RegexOptions.Compiled)]
    private static partial Regex UtcOffsetTime2Regex();
}