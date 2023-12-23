using System.Text.RegularExpressions;
using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Types;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Functions;

public sealed partial class CoreFunctions
{
    // Based on SMTP protocol RFC 5322
    private static readonly Regex EmailRegex = new(
        "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$", RegexOptions.Compiled);

    // Based on ITU-T E.163 and E.164 (extended)
    private static readonly Regex PhoneRegex = new(@"^\+?[0-9\s-()]+$", RegexOptions.Compiled);

    public bool Elements(JArray target, params JNode[] items)
    {
        return items.Where(n => !target.Elements.Contains(n))
            .ForEachTrue(FailWithNode);

        bool FailWithNode(JNode node)
        {
            return FailWith(new JsonSchemaException(
                new ErrorDetail(ELEM01, "Value is not an element of array"),
                new ExpectedDetail(Function, $"array with value {node}"),
                new ActualDetail(target, $"not found in {target.GetOutline()}")));
        }
    }

    public bool Keys(JObject target, params JString[] items)
    {
        return target.Properties.ContainsKeys(items)
            .ForEachTrue(FailWithNode);

        bool FailWithNode(string node)
        {
            return FailWith(new JsonSchemaException(
                new ErrorDetail(KEYS01, "Object does not contain the key"),
                new ExpectedDetail(Function, $"object with key {node.Quote()}"),
                new ActualDetail(target, $"does not contain in {target.GetOutline()}")));
        }
    }

    public bool Values(JObject target, params JNode[] items)
    {
        return target.Properties.ContainsValues(items)
            .ForEachTrue(FailWithNode);

        bool FailWithNode(JNode node)
        {
            return FailWith(new JsonSchemaException(
                new ErrorDetail(VALU01, "Object does not contain the value"),
                new ExpectedDetail(Function, $"object with value {node}"),
                new ActualDetail(target, $"does not contain in {target.GetOutline()}")));
        }
    }

    public bool Regex(JString target, JString pattern)
    {
        var regex = new Regex($"^{(string) pattern}$");
        bool result = regex.IsMatch(target);
        if(!result) return FailWith(new JsonSchemaException(
            new ErrorDetail(REGX01, "Regex pattern does not match"),
            new ExpectedDetail(Function, $"string of pattern {pattern}"),
            new ActualDetail(target, $"found {target.GetOutline()} that mismatches with pattern")));
        return true;
    }

    public bool Email(JString target)
    {
        var result = EmailRegex.IsMatch(target);
        if(!result) return FailWith(new JsonSchemaException(
            new ErrorDetail(EMAL01, "Invalid email address"),
            new ExpectedDetail(Function, "a valid email address"),
            new ActualDetail(target, $"found {target} that is invalid")));
        return true;
    }

    public bool Url(JString target)
    {
        // Handle Uri based on RFC 3986
        bool result = Uri.TryCreate(target, UriKind.Absolute, out Uri? uriResult);
        if(!result || uriResult == null) return FailWith(new JsonSchemaException(
            new ErrorDetail(URLA01, "Invalid url address"),
            new ExpectedDetail(Function, "a valid url address"),
            new ActualDetail(target, $"found {target} that is invalid")));
        result &= uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps;
        if(!result) return FailWith(new JsonSchemaException(
            new ErrorDetail(URLA02, "Invalid url address scheme"),
            new ExpectedDetail(Function, "HTTP or HTTPS scheme"),
            new ActualDetail(target, $"found {uriResult.Scheme.Quote()} from {
                target} that has invalid scheme")));
        return true;
    }

    public bool Url(JString target, JString scheme)
    {
        bool result = Uri.TryCreate(target, UriKind.Absolute, out Uri? uriResult);
        if(!result || uriResult == null) return FailWith(
            new JsonSchemaException(new ErrorDetail(URLA03, "Invalid url address"),
            new ExpectedDetail(Function, "a valid url address"),
            new ActualDetail(target, $"found {target} that is invalid")));
        result &= uriResult.Scheme.Equals(scheme);
        if(!result) return FailWith(new JsonSchemaException(
            new ErrorDetail(URLA04, "Mismatch url address scheme"),
            new ExpectedDetail(Function, $"scheme {scheme} for url address"),
            new ActualDetail(target, $"found {uriResult.Scheme.Quote()} from {
                target} that does not matched")));
        return true;
    }

    public bool Phone(JString target)
    {
        bool result = PhoneRegex.IsMatch(target);
        if(!result) return FailWith(new JsonSchemaException(
            new ErrorDetail(PHON01, "Invalid phone number format"),
            new ExpectedDetail(Function, "a valid phone number"),
            new ActualDetail(target, $"found {target} that is invalid")));
        return true;
    }
}