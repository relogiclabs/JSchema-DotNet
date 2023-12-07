using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Time;
using RelogicLabs.JsonSchema.Types;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Time.DateTimeType;

namespace RelogicLabs.JsonSchema.Functions;

public sealed partial class CoreFunctions
{
    public bool Date(JString target, JString pattern)
        => DateTime(target, pattern, DATE_TYPE);

    public bool Time(JString target, JString pattern)
        => DateTime(target, pattern, TIME_TYPE);

    private bool DateTime(JString target, JString pattern, DateTimeType type)
        => !ReferenceEquals(new DateTimeAgent(pattern, type).Parse(Function, target), null);

    public bool Before(JDateTime target, JString reference)
    {
        var dateTime = GetDateTime(target.GetDateTimeParser(), reference);
        if(ReferenceEquals(dateTime, null)) return false;
        if(target.DateTime.Compare(dateTime.DateTime) < 0) return true;
        var type = target.DateTime.Type;
        var code = type == DATE_TYPE ? BFOR01 : BFOR02;
        return FailWith(new JsonSchemaException(
            new ErrorDetail(code, $"{type} is not earlier than specified"),
            new ExpectedDetail(reference, $"a {type} before {reference}"),
            new ActualDetail(target, $"found {target} which is not inside limit")
        ));
    }

    public bool After(JDateTime target, JString reference)
    {
        var dateTime = GetDateTime(target.GetDateTimeParser(), reference);
        if(ReferenceEquals(dateTime, null)) return false;
        if(target.DateTime.Compare(dateTime.DateTime) > 0) return true;
        var type = target.DateTime.Type;
        var code = type == DATE_TYPE ? AFTR01 : AFTR02;
        return FailWith(new JsonSchemaException(
            new ErrorDetail(code, $"{type} is not later than specified"),
            new ExpectedDetail(reference, $"a {type} after {reference}"),
            new ActualDetail(target, $"found {target} which is not inside limit")
        ));
    }

    public bool Range(JDateTime target, JString start, JString end)
    {
        var _start = GetDateTime(target.GetDateTimeParser(), start);
        if(ReferenceEquals(_start, null)) return false;
        var _end = GetDateTime(target.GetDateTimeParser(), end);
        if(ReferenceEquals(_end, null)) return false;
        if(target.DateTime.Compare(_start.DateTime) < 0)
            return FailOnStartDate(target, _start, GetErrorCode(target, DRNG01, DRNG02));
        if(target.DateTime.Compare(_end.DateTime) > 0)
            return FailOnEndDate(target, _end, GetErrorCode(target, DRNG03, DRNG04));
        return true;
    }

    private static string GetErrorCode(JDateTime target, string date, string time) {
        return target.DateTime.Type == DATE_TYPE ? date : time;
    }

    private bool FailOnStartDate(JDateTime target, JDateTime start, string code)
    {
        var type = target.DateTime.Type;
        return FailWith(new JsonSchemaException(
            new ErrorDetail(code, $"{type} is earlier than start {type}"),
            new ExpectedDetail(start, $"a {type} from or after {start}"),
            new ActualDetail(target, $"found {target} which is before start {type}")
        ));
    }

    private bool FailOnEndDate(JDateTime target, JDateTime end, string code)
    {
        var type = target.DateTime.Type;
        return FailWith(new JsonSchemaException(
            new ErrorDetail(code, $"{type} is later than end {type}"),
            new ExpectedDetail(end, $"a {type} until or before {end}"),
            new ActualDetail(target, $"found {target} which is after end {type}")
        ));
    }

    public bool Range(JDateTime target, JUndefined start, JString end)
    {
        var _end = GetDateTime(target.GetDateTimeParser(), end);
        if(ReferenceEquals(_end, null)) return false;
        if(target.DateTime.Compare(_end.DateTime) <= 0) return true;
        return FailOnEndDate(target, _end, GetErrorCode(target, DRNG05, DRNG06));
    }

    public bool Range(JDateTime target, JString start, JUndefined end)
    {
        var _start = GetDateTime(target.GetDateTimeParser(), start);
        if(ReferenceEquals(_start, null)) return false;
        if(target.DateTime.Compare(_start.DateTime) >= 0) return true;
        return FailOnStartDate(target, _start, GetErrorCode(target, DRNG07, DRNG08));
    }

    public bool Start(JDateTime target, JString reference)
    {
        var dateTime = GetDateTime(target.GetDateTimeParser(), reference);
        if(ReferenceEquals(dateTime, null)) return false;
        if(target.DateTime.Compare(dateTime.DateTime) < 0)
        {
            var type = target.DateTime.Type;
            var code = type == DATE_TYPE ? STRT01 : STRT02;
            return FailWith(new JsonSchemaException(
                new ErrorDetail(code, $"{type} is earlier than specified"),
                new ExpectedDetail(dateTime, $"a {type} from or after {dateTime}"),
                new ActualDetail(target, $"found {target} which is before limit")
            ));
        }
        return true;
    }

    public bool End(JDateTime target, JString reference)
    {
        var dateTime = GetDateTime(target.GetDateTimeParser(), reference);
        if(ReferenceEquals(dateTime, null)) return false;
        if(target.DateTime.Compare(dateTime.DateTime) > 0)
        {
            var type = target.DateTime.Type;
            var code = type == DATE_TYPE ? ENDE01 : ENDE02;
            return FailWith(new JsonSchemaException(
                new ErrorDetail(code, $"{type} is later than specified"),
                new ExpectedDetail(dateTime, $"a {type} until or before {dateTime}"),
                new ActualDetail(target, $"found {target} which is after limit")
            ));
        }
        return true;
    }

    private JDateTime? GetDateTime(DateTimeParser parser, JString dateTime)
    {
        if(dateTime.Derived is JDateTime _result
           && _result.DateTime.Type == parser.Type) return _result;
        var _dateTime = new DateTimeAgent(parser).Parse(Function, dateTime);
        if(ReferenceEquals(_dateTime, null)) return null;
        dateTime.Derived = _dateTime.CreateNode(dateTime);
        return (JDateTime) dateTime.Derived;
    }
}