using System.Text.RegularExpressions;
using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Types;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Functions;

public class CoreFunctions : FunctionBase
{
    public CoreFunctions(RuntimeContext runtime) : base(runtime) { }

    public bool Length(JString source, JInteger length)
    {
        var _length = source.Value.Length;
        if(_length != length.Value)
            return FailWith(new JsonSchemaException(
                new ErrorDetail(SLEN01, "Invalid string length"),
                new ExpectedDetail(Function, $"length {length}"),
                new ActualDetail(source, $"found {_length} for \"{source}\"")));
        return true;
    }
    
    public bool Length(JArray source, JInteger length)
    {
        var _length = source.Elements.Count;
        if(_length != length.Value)
            return FailWith(new JsonSchemaException(
                new ErrorDetail(ALEN01, "Invalid array length"),
                new ExpectedDetail(Function, $"length {length}"),
                new ActualDetail(source, $"found {_length} for \"{source.ToOutline()}\"")));
        return true;
    }

    public bool Length(JString source, JInteger minLength, JInteger maxLength)
    {
        int length = source.Value.Length;
        if(length < minLength)
            return FailWith(new JsonSchemaException(new ErrorDetail(SLEN02, 
                    $"String {source.ToOutline()} length is outside of range"),
                new ExpectedDetail(Function, $"length in range [{minLength}, {maxLength}]"),
                new ActualDetail(source, $"found {length} that is less than {minLength}")));
        if(length > maxLength)
            return FailWith(new JsonSchemaException(new ErrorDetail(SLEN03,
                    $"String {source.ToOutline()} length is outside of range"),
                new ExpectedDetail(Function, $"length in range [{minLength}, {maxLength}]"),
                new ActualDetail(source, $"found {length} that is greater than {maxLength}")));
        return true;
    }
    
    public bool Length(JString source, JInteger minLength, JUnknown unknown)
    {
        int length = source.Value.Length;
        if(length < minLength)
            return FailWith(new JsonSchemaException(new ErrorDetail(SLEN04,
                    $"String {source.ToOutline()} length is outside of range"),
                new ExpectedDetail(Function, $"length in range [{minLength}, {unknown}]"),
                new ActualDetail(source, $"found {length} that is less than {minLength}")));
        return true;
    }
    
    public bool Length(JString source, JUnknown unknown, JInteger maxLength)
    {
        int length = source.Value.Length;
        if(length > maxLength)
            return FailWith(new JsonSchemaException(new ErrorDetail(SLEN05,
                    $"String {source.ToOutline()} length is outside of range"),
                new ExpectedDetail(Function, $"length in range [{unknown}, {maxLength}]"),
                new ActualDetail(source, $"found {length} that is greater than {maxLength}")));
        return true;
    }
    
    public bool Length(JArray source, JInteger minLength, JInteger maxLength)
    {
        int length = source.Elements.Count;
        if(length < minLength)
            return FailWith(new JsonSchemaException(new ErrorDetail(ALEN02,
                    $"Array {source.ToOutline()} length is outside of range"),
                new ExpectedDetail(Function, $"length in range [{minLength}, {maxLength}]"),
                new ActualDetail(source, $"found {length} that is less than {minLength}")));
        if(length > maxLength)
            return FailWith(new JsonSchemaException(new ErrorDetail(ALEN03,
                    $"String {source.ToOutline()} length is outside of range"),
                new ExpectedDetail(Function, $"length in range [{minLength}, {maxLength}]"),
                new ActualDetail(source, $"found {length} that is greater than {maxLength}")));
        return true;
    }
    
    public bool Length(JArray source, JInteger minLength, JUnknown unknown)
    {
        int length = source.Elements.Count;
        if(length < minLength)
            return FailWith(new JsonSchemaException(new ErrorDetail(ALEN04,
                    $"Array {source.ToOutline()} length is outside of range"),
                new ExpectedDetail(Function, $"length in range [{minLength}, {unknown}]"),
                new ActualDetail(source, $"found {length} that is less than {minLength}")));
        return true;
    }
    
    public bool Length(JArray source, JUnknown unknown, JInteger maxLength)
    {
        int length = source.Elements.Count;
        if(length > maxLength)
            return FailWith(new JsonSchemaException(new ErrorDetail(ALEN05,
                    $"String {source.ToOutline()} length is outside of range"),
                new ExpectedDetail(Function, $"length in range [{unknown}, {maxLength}]"),
                new ActualDetail(source, $"found {length} that is greater than {maxLength}")));
        return true;
    }

    public bool Elements(JArray source, params JNode[] nodes)
    {
        return nodes.Where(n => !source.Elements.Contains(n))
            .Select(FailWithNode).ForEachTrue();
        
        bool FailWithNode(JNode node)
        {
            return FailWith(new JsonSchemaException(new ErrorDetail(ELEM01,
                    "Value is not an element of array"),
                new ExpectedDetail(Function, $"array with value {node}"),
                new ActualDetail(source, $"not found in {source.ToOutline()}")));
        }
    }

    public bool Keys(JObject source, params JString[] nodes)
    {
        return nodes.Where(n => !source.Properties.ContainsKey(n))
            .Select(FailWithNode).ForEachTrue();
        
        bool FailWithNode(JNode node)
        {
            return FailWith(new JsonSchemaException(new ErrorDetail(KEYS01,
                    "Object does not contain the key"),
                new ExpectedDetail(Function, $"object with key {node}"),
                new ActualDetail(source, $"does not contain in {source.ToOutline()}")));
        }
    }
    
    public bool Values(JObject source, params JNode[] nodes)
    {
        return nodes.Where(n => !source.Properties.ContainsValue(n))
            .Select(FailWithNode).ForEachTrue();
        
        bool FailWithNode(JNode node)
        {
            return FailWith(new JsonSchemaException(new ErrorDetail(VALU01,
                    $"Object does not contain the value"),
                new ExpectedDetail(Function, $"object with value {node}"),
                new ActualDetail(source, $"does not contain in {source.ToOutline()}")));
        }
    }

    public bool Enum(JString source, params JString[] strings)
    {
        if(!strings.Contains(source))
            return FailWith(new JsonSchemaException(new ErrorDetail(ENUM01,
                    $"String is not in enum list"),
                new ExpectedDetail(Function, $"string in list {strings.Select(s => s.ToJson())
                    .ToString(", ", "[", "]")}"),
                new ActualDetail(source, $"string {source} is not found in list")));
        return true;
    }
    
    public bool Enum(JNumber source, params JNumber[] numbers)
    {
        if(!numbers.Contains(source))
            return FailWith(new JsonSchemaException(new ErrorDetail(ENUM02,
                    $"Number is not in enum list"),
                new ExpectedDetail(Function, $"number in list {numbers.Select(s => s.ToJson())
                    .ToString(", ", "[", "]")}"),
                new ActualDetail(source, $"number {source} is not found in list")));
        return true;
    }

    public bool Range(JNumber source, JNumber minNumber, JNumber maxNumber)
    {
        if(source.Compare(minNumber) < 0)
            return FailWith(new JsonSchemaException(new ErrorDetail(RANG01,
                    $"Number is outside of range"),
                new ExpectedDetail(Function, $"number in range [{minNumber}, {maxNumber}]"),
                new ActualDetail(source, $"number {source} is less than {minNumber}")));
        if(source.Compare(maxNumber) > 0)
            return FailWith(new JsonSchemaException(new ErrorDetail(RANG02,
                    $"Number is outside of range"),
                new ExpectedDetail(Function, $"number in range [{minNumber}, {maxNumber}]"),
                new ActualDetail(source, $"number {source} is greater than {maxNumber}")));
        return true;
    }

    public bool Positive(JNumber source)
    {
        if(source.Compare(0) <= 0)
            return FailWith(new JsonSchemaException(new ErrorDetail(POSI01,
                    $"Number is not positive"),
                new ExpectedDetail(Function, $"a positive number"),
                new ActualDetail(source, $"number {source} is less than or equal to zero")));
        return true;
    }

    public bool Negative(JNumber source)
    {
        if(source.Compare(0) >= 0)
            return FailWith(new JsonSchemaException(new ErrorDetail(NEGI01,
                    $"Number is not negative"),
                new ExpectedDetail(Function, $"a negative number"),
                new ActualDetail(source, $"number {source} is greater than or equal to zero")));
        return true;
    }
    
    public bool Range(JNumber source, JNumber minNumber, JUnknown unknown)
    {
        if(source.Compare(minNumber) < 0)
            return FailWith(new JsonSchemaException(new ErrorDetail(RANG03,
                    $"Number is outside of range"),
                new ExpectedDetail(Function, $"number in range [{minNumber}, {unknown}]"),
                new ActualDetail(source, $"number {source} is less than {minNumber}")));
        return true;
    }
    
    public bool Range(JNumber source, JUnknown unknown, JNumber maxNumber)
    {
        if(source.Compare(maxNumber) > 0)
            return FailWith(new JsonSchemaException(new ErrorDetail(RANG04,
                    $"Number is outside of range"),
                new ExpectedDetail(Function, $"number in range [{unknown}, {maxNumber}]"),
                new ActualDetail(source, $"number {source} is greater than {maxNumber}")));
        return true;
    }

    public bool Regex(JString source, JString pattern)
    {
        var regex = new Regex(((string) pattern).ToString("^", "$"));
        bool result = regex.IsMatch(source);
        if(!result) return FailWith(new JsonSchemaException(new ErrorDetail(REGX01,
                $"Regex pattern does not match"),
            new ExpectedDetail(Function, $"string of pattern {pattern}"),
            new ActualDetail(source, $"found {source.ToOutline()} that mismatches with pattern")));
        return true;
    }

    public bool Email(JString source)
    {
        // Match with all common email addresses while excluding very uncommon ones
        Regex email = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        bool result = email.IsMatch(source);
        
        if(!result) return FailWith(new JsonSchemaException(new ErrorDetail(EMAL01,
            $"Invalid email address"),
            new ExpectedDetail(Function, $"a valid email address"),
            new ActualDetail(source, $"found {source} that is invalid")));
        return true;
    }

    public bool Url(JString source)
    {
        bool result = Uri.TryCreate(source, UriKind.Absolute, out Uri? uriResult);
        if(!result || uriResult == null) return FailWith(new JsonSchemaException(
            new ErrorDetail(URLA01, $"Invalid url address"),
            new ExpectedDetail(Function, "a valid url address"),
            new ActualDetail(source, $"found {source} that is invalid")));
        result &= uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps;
        if(!result) return FailWith(new JsonSchemaException(
            new ErrorDetail(URLA02, $"Invalid url address scheme"),
            new ExpectedDetail(Function, "HTTP or HTTPS scheme"),
            new ActualDetail(source, $"found {uriResult.Scheme.DoubleQuote()} from {
                source} that has invalid scheme")));
        return true;
    }
    
    public bool Url(JString source, JString scheme)
    {
        bool result = Uri.TryCreate(source, UriKind.Absolute, out Uri? uriResult);
        if(!result || uriResult == null) return FailWith(
            new JsonSchemaException(new ErrorDetail(URLA03, $"Invalid url address"),
            new ExpectedDetail(Function, "a valid url address"),
            new ActualDetail(source, $"found {source} that is invalid")));
        result &= uriResult.Scheme.Equals(scheme);
        if(!result) return FailWith(new JsonSchemaException(
            new ErrorDetail(URLA04, "Mismatch url address scheme"),
            new ExpectedDetail(Function, $"scheme {scheme} for url address"),
            new ActualDetail(source, $"found {uriResult.Scheme.DoubleQuote()} from {
                source} that does not matched")));
        return true;
    }

    public bool Phone(JString source)
    {
        // Match with common phone number formats while excluding very uncommon ones
        Regex phone = new Regex(@"^\+?[0-9\s-()]+$");
        bool result = phone.IsMatch(source);
        if(!result) return FailWith(new JsonSchemaException(
            new ErrorDetail(PHON01, "Invalid phone number format"),
            new ExpectedDetail(Function, "a valid phone number"),
            new ActualDetail(source, $"found {source} that is invalid")));
        return true;
    }
}