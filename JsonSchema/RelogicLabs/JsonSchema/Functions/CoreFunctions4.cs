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
        => !ReferenceEquals(new DateTimeFunction(pattern, type).Parse(Function, target), null);

    public bool Before(JDateTime target, JString reference)
    {
        var dateTime = GetDateTime(target.GetParser(), reference);
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
        var dateTime = GetDateTime(target.GetParser(), reference);
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
        var _start = GetDateTime(target.GetParser(), start);
        if(ReferenceEquals(_start, null)) return false;
        var _end = GetDateTime(target.GetParser(), end);
        if(ReferenceEquals(_end, null)) return false;
        if(target.DateTime.Compare(_start.DateTime) < 0)
        {
            var type = target.DateTime.Type;
            var code = type == DATE_TYPE ? DRNG01 : DRNG02;
            return FailWith(new JsonSchemaException(
                new ErrorDetail(code, $"{type} is earlier than start {type}"),
                new ExpectedDetail(_start, $"a {type} from or after {_start}"),
                new ActualDetail(target, $"found {target} which is before start {type}")
            ));
        }
        if(target.DateTime.Compare(_end.DateTime) > 0)
        {
            var type = target.DateTime.Type;
            var code = type == DATE_TYPE ? DRNG03 : DRNG04;
            return FailWith(new JsonSchemaException(
                new ErrorDetail(code, $"{type} is later than end {type}"),
                new ExpectedDetail(_end, $"a {type} until or before {_end}"),
                new ActualDetail(target, $"found {target} which is after end {type}")
            ));
        }
        return true;
    }

    public bool Range(JDateTime target, JUndefined start, JString end)
    {
        var _end = GetDateTime(target.GetParser(), end);
        if(ReferenceEquals(_end, null)) return false;
        if(target.DateTime.Compare(_end.DateTime) > 0)
        {
            var type = target.DateTime.Type;
            var code = type == DATE_TYPE ? DRNG05 : DRNG06;
            return FailWith(new JsonSchemaException(
                new ErrorDetail(code, $"{type} is later than end {type}"),
                new ExpectedDetail(_end, $"a {type} until or before {_end}"),
                new ActualDetail(target, $"found {target} which is after end {type}")
            ));
        }
        return true;
    }

    public bool Range(JDateTime target, JString start, JUndefined end)
    {
        var _start = GetDateTime(target.GetParser(), start);
        if(ReferenceEquals(_start, null)) return false;
        if(target.DateTime.Compare(_start.DateTime) < 0)
        {
            var type = target.DateTime.Type;
            var code = type == DATE_TYPE ? DRNG07 : DRNG08;
            return FailWith(new JsonSchemaException(
                new ErrorDetail(code, $"{type} is earlier than start {type}"),
                new ExpectedDetail(_start, $"a {type} from or after {_start}"),
                new ActualDetail(target, $"found {target} which is before start {type}")
            ));
        }
        return true;
    }

    public bool Start(JDateTime target, JString reference)
    {
        var dateTime = GetDateTime(target.GetParser(), reference);
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
        var dateTime = GetDateTime(target.GetParser(), reference);
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
        var _dateTime = new DateTimeFunction(parser).Parse(Function, dateTime);
        if(ReferenceEquals(_dateTime, null)) return null;
        dateTime.Derived = _dateTime.Create(dateTime);
        return (JDateTime) dateTime.Derived;
    }
}