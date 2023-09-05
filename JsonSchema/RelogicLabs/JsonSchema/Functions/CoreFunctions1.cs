using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Types;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Functions;

public partial class CoreFunctions : FunctionBase
{
    public CoreFunctions(RuntimeContext runtime) : base(runtime) { }

    public bool Length(JString target, JInteger length)
    {
        var _length = target.Value.Length;
        if(_length != length) return FailWith(new JsonSchemaException(
                new ErrorDetail(SLEN01, "Invalid string length"),
                new ExpectedDetail(Function, $"length {length}"),
                new ActualDetail(target, $"found {_length} for \"{target}\"")));
        return true;
    }
    
    public bool Length(JArray target, JInteger length)
    {
        var _length = target.Elements.Count;
        if(_length != length) return FailWith(new JsonSchemaException(
                new ErrorDetail(ALEN01, "Invalid array length"),
                new ExpectedDetail(Function, $"length {length}"),
                new ActualDetail(target, $"found {_length} for {target.ToOutline()}")));
        return true;
    }

    public bool Length(JObject target, JInteger length)
    {
        var _length = target.Properties.Count;
        if(_length != length) return FailWith(new JsonSchemaException(
                new ErrorDetail(OLEN01, "Invalid object size or length"),
                new ExpectedDetail(Function, $"length {length}"),
                new ActualDetail(target, $"found {_length} for {target.ToOutline()}")));
        return true;
    }

    public bool Length(JString target, JInteger minimum, JInteger maximum)
    {
        var length = target.Value.Length;
        if(length < minimum)
            return FailWith(new JsonSchemaException(new ErrorDetail(SLEN02, 
                    $"String \"{target.ToOutline()}\" length is outside of range"),
                new ExpectedDetail(Function, $"length in range [{minimum}, {maximum}]"),
                new ActualDetail(target, $"found {length} that is less than {minimum}")));
        if(length > maximum)
            return FailWith(new JsonSchemaException(new ErrorDetail(SLEN03,
                    $"String {target.ToOutline()} length is outside of range"),
                new ExpectedDetail(Function, $"length in range [{minimum}, {maximum}]"),
                new ActualDetail(target, $"found {length} that is greater than {maximum}")));
        return true;
    }
    
    public bool Length(JString target, JInteger minimum, JUndefined undefined)
    {
        var length = target.Value.Length;
        if(length < minimum)
            return FailWith(new JsonSchemaException(new ErrorDetail(SLEN04,
                    $"String \"{target.ToOutline()}\" length is outside of range"),
                new ExpectedDetail(Function, $"length in range [{minimum}, {undefined}]"),
                new ActualDetail(target, $"found {length} that is less than {minimum}")));
        return true;
    }
    
    public bool Length(JString target, JUndefined undefined, JInteger maximum)
    {
        var length = target.Value.Length;
        if(length > maximum)
            return FailWith(new JsonSchemaException(new ErrorDetail(SLEN05,
                    $"String \"{target.ToOutline()}\" length is outside of range"),
                new ExpectedDetail(Function, $"length in range [{undefined}, {maximum}]"),
                new ActualDetail(target, $"found {length} that is greater than {maximum}")));
        return true;
    }
    
    public bool Length(JArray target, JInteger minimum, JInteger maximum)
    {
        var length = target.Elements.Count;
        if(length < minimum)
            return FailWith(new JsonSchemaException(new ErrorDetail(ALEN02,
                    $"Array {target.ToOutline()} length is outside of range"),
                new ExpectedDetail(Function, $"length in range [{minimum}, {maximum}]"),
                new ActualDetail(target, $"found {length} that is less than {minimum}")));
        if(length > maximum)
            return FailWith(new JsonSchemaException(new ErrorDetail(ALEN03,
                    $"Array {target.ToOutline()} length is outside of range"),
                new ExpectedDetail(Function, $"length in range [{minimum}, {maximum}]"),
                new ActualDetail(target, $"found {length} that is greater than {maximum}")));
        return true;
    }
    
    public bool Length(JArray target, JInteger minimum, JUndefined undefined)
    {
        var length = target.Elements.Count;
        if(length < minimum)
            return FailWith(new JsonSchemaException(new ErrorDetail(ALEN04,
                    $"Array {target.ToOutline()} length is outside of range"),
                new ExpectedDetail(Function, $"length in range [{minimum}, {undefined}]"),
                new ActualDetail(target, $"found {length} that is less than {minimum}")));
        return true;
    }
    
    public bool Length(JArray target, JUndefined undefined, JInteger maximum)
    {
        var length = target.Elements.Count;
        if(length > maximum)
            return FailWith(new JsonSchemaException(new ErrorDetail(ALEN05,
                    $"Array {target.ToOutline()} length is outside of range"),
                new ExpectedDetail(Function, $"length in range [{undefined}, {maximum}]"),
                new ActualDetail(target, $"found {length} that is greater than {maximum}")));
        return true;
    }

    public bool Length(JObject target, JInteger minimum, JInteger maximum)
    {
        var length = target.Properties.Count;
        if(length < minimum)
            return FailWith(new JsonSchemaException(new ErrorDetail(OLEN02,
                    $"Object {target.ToOutline()} size or length is outside of range"),
                new ExpectedDetail(Function, $"length in range [{minimum}, {maximum}]"),
                new ActualDetail(target, $"found {length} that is less than {minimum}")));
        if(length > maximum)
            return FailWith(new JsonSchemaException(new ErrorDetail(OLEN03,
                    $"Object {target.ToOutline()} size or length is outside of range"),
                new ExpectedDetail(Function, $"length in range [{minimum}, {maximum}]"),
                new ActualDetail(target, $"found {length} that is greater than {maximum}")));
        return true;
    }
    
    public bool Length(JObject target, JInteger minimum, JUndefined undefined)
    {
        var length = target.Properties.Count;
        if(length < minimum)
            return FailWith(new JsonSchemaException(new ErrorDetail(OLEN04,
                    $"Object {target.ToOutline()} size or length is outside of range"),
                new ExpectedDetail(Function, $"length in range [{minimum}, {undefined}]"),
                new ActualDetail(target, $"found {length} that is less than {minimum}")));
        return true;
    }
    
    public bool Length(JObject target, JUndefined undefined, JInteger maximum)
    {
        var length = target.Properties.Count;
        if(length > maximum)
            return FailWith(new JsonSchemaException(new ErrorDetail(OLEN05,
                    $"Object {target.ToOutline()} size or length is outside of range"),
                new ExpectedDetail(Function, $"length in range [{undefined}, {maximum}]"),
                new ActualDetail(target, $"found {length} that is greater than {maximum}")));
        return true;
    }
}