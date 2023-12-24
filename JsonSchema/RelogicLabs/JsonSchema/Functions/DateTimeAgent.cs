using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Time;
using RelogicLabs.JsonSchema.Types;

namespace RelogicLabs.JsonSchema.Functions;

internal class DateTimeAgent
{
    private DateTimeParser? _parser;

    public string Pattern { get; }
    public DateTimeType Type { get; }

    public DateTimeAgent(string pattern, DateTimeType type)
    {
        Pattern = pattern;
        Type = type;
    }

    public DateTimeAgent(DateTimeParser parser)
    {
        Pattern = parser.Pattern;
        Type = parser.Type;
        _parser = parser;
    }

    public JsonDateTime? Parse(JFunction function, JString dateTime)
    {
        var exceptions = function.Runtime.Exceptions;
        try
        {
            _parser ??= new DateTimeParser(Pattern, Type);
            return _parser.Parse(dateTime);
        }
        catch(DateTimeLexerException ex)
        {
            exceptions.FailWith(new JsonSchemaException(
                new ErrorDetail(ex.Code, ex.Message),
                new ExpectedDetail(function, $"a valid {Type} pattern"),
                new ActualDetail(dateTime, $"found {Pattern} that is invalid"),
                ex));
        }
        catch(InvalidDateTimeException ex)
        {
            exceptions.FailWith(new JsonSchemaException(
                new ErrorDetail(ex.Code, ex.Message),
                new ExpectedDetail(function, $"a valid {Type} formatted as {Pattern}"),
                new ActualDetail(dateTime, $"found {dateTime} that is invalid or malformatted"),
                ex));
        }
        return null;
    }
}