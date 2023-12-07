using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Types;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Functions;

public sealed partial class CoreFunctions
{
    public bool Enum(JString target, params JString[] items)
    {
        if(!items.Contains(target))
            return FailWith(new JsonSchemaException(
                new ErrorDetail(ENUM01, "String is not in enum list"),
                new ExpectedDetail(Function, $"string in list {items.ToString(", ", "[", "]")}"),
                new ActualDetail(target, $"string {target.GetOutline()} is not found in list")));
        return true;
    }

    public bool Enum(JNumber target, params JNumber[] items)
    {
        if(!items.Contains(target))
            return FailWith(new JsonSchemaException(
                new ErrorDetail(ENUM02, "Number is not in enum list"),
                new ExpectedDetail(Function, $"number in list {items.ToString(", ", "[", "]")}"),
                new ActualDetail(target, $"number {target} is not found in list")));
        return true;
    }

    public bool Minimum(JNumber target, JNumber minimum)
    {
        if(target.Compare(minimum) < 0)
            return FailWith(new JsonSchemaException(
                new ErrorDetail(MINI01, "Number is less than provided minimum"),
                new ExpectedDetail(Function, $"a number greater than or equal to {minimum}"),
                new ActualDetail(target, $"number {target} is less than {minimum}")));
        return true;
    }

    public bool Minimum(JNumber target, JNumber minimum, JBoolean exclusive)
    {
        if(target.Compare(minimum) < 0)
            return FailWith(new JsonSchemaException(
                new ErrorDetail(MINI02, "Number is less than provided minimum"),
                new ExpectedDetail(Function, $"a number {RelationTo()} {minimum}"),
                new ActualDetail(target, $"number {target} is less than {minimum}")));
        if(exclusive && target.Compare(minimum) == 0)
            return FailWith(new JsonSchemaException(
                new ErrorDetail(MINI03, "Number is equal to provided minimum"),
                new ExpectedDetail(Function, $"a number {RelationTo()} {minimum}"),
                new ActualDetail(target, $"number {target} is equal to {minimum}")));
        return true;

        string RelationTo() => exclusive
            ? "greater than"
            : "greater than or equal to";
    }

    public bool Maximum(JNumber target, JNumber maximum)
    {
        if(target.Compare(maximum) > 0)
            return FailWith(new JsonSchemaException(
                new ErrorDetail(MAXI01, "Number is greater than provided maximum"),
                new ExpectedDetail(Function, $"a number less than or equal {maximum}"),
                new ActualDetail(target, $"number {target} is greater than {maximum}")));
        return true;
    }

    public bool Maximum(JNumber target, JNumber maximum, JBoolean exclusive)
    {
        if(target.Compare(maximum) > 0)
            return FailWith(new JsonSchemaException(
                new ErrorDetail(MAXI02, "Number is greater than provided maximum"),
                new ExpectedDetail(Function, $"a number {RelationTo()} {maximum}"),
                new ActualDetail(target, $"number {target} is greater than {maximum}")));
        if(exclusive && target.Compare(maximum) == 0)
            return FailWith(new JsonSchemaException(
                new ErrorDetail(MAXI03, "Number is equal to provided maximum"),
                new ExpectedDetail(Function, $"a number {RelationTo()} {maximum}"),
                new ActualDetail(target, $"number {target} is equal to {maximum}")));
        return true;

        string RelationTo() => exclusive
            ? "less than"
            : "less than or equal to";
    }

    public bool Positive(JNumber target)
    {
        if(target.Compare(0) <= 0)
            return FailWith(new JsonSchemaException(
                new ErrorDetail(POSI01, "Number is not positive"),
                new ExpectedDetail(Function, "a positive number"),
                new ActualDetail(target, $"number {target} is less than or equal to zero")));
        return true;
    }

    public bool Negative(JNumber target)
    {
        if(target.Compare(0) >= 0)
            return FailWith(new JsonSchemaException(
                new ErrorDetail(NEGI01, "Number is not negative"),
                new ExpectedDetail(Function, "a negative number"),
                new ActualDetail(target, $"number {target} is greater than or equal to zero")));
        return true;
    }

    public bool Positive(JNumber target, JNumber reference)
    {
        if(target.Compare(reference) < 0)
            return FailWith(new JsonSchemaException(
                new ErrorDetail(POSI02, "Number is not positive from reference"),
                new ExpectedDetail(Function, $"a positive number from {reference}"),
                new ActualDetail(target, $"number {target} is less than reference")));
        return true;
    }

    public bool Negative(JNumber target, JNumber reference)
    {
        if(target.Compare(reference) > 0)
            return FailWith(new JsonSchemaException(
                new ErrorDetail(NEGI02, "Number is not negative from reference"),
                new ExpectedDetail(Function, $"a negative number from {reference}"),
                new ActualDetail(target, $"number {target} is greater than reference")));
        return true;
    }

    public bool Range(JNumber target, JNumber minimum, JNumber maximum)
    {
        if(target.Compare(minimum) < 0)
            return FailWith(new JsonSchemaException(
                new ErrorDetail(RANG01, "Number is outside of range"),
                new ExpectedDetail(Function, $"number in range [{minimum}, {maximum}]"),
                new ActualDetail(target, $"number {target} is less than {minimum}")));
        if(target.Compare(maximum) > 0)
            return FailWith(new JsonSchemaException(
                new ErrorDetail(RANG02, "Number is outside of range"),
                new ExpectedDetail(Function, $"number in range [{minimum}, {maximum}]"),
                new ActualDetail(target, $"number {target} is greater than {maximum}")));
        return true;
    }

    public bool Range(JNumber target, JNumber minimum, JUndefined undefined)
    {
        if(target.Compare(minimum) < 0)
            return FailWith(new JsonSchemaException(
                new ErrorDetail(RANG03, "Number is outside of range"),
                new ExpectedDetail(Function, $"number in range [{minimum}, {undefined}]"),
                new ActualDetail(target, $"number {target} is less than {minimum}")));
        return true;
    }

    public bool Range(JNumber target, JUndefined undefined, JNumber maximum)
    {
        if(target.Compare(maximum) > 0)
            return FailWith(new JsonSchemaException(
                new ErrorDetail(RANG04, "Number is outside of range"),
                new ExpectedDetail(Function, $"number in range [{undefined}, {maximum}]"),
                new ActualDetail(target, $"number {target} is greater than {maximum}")));
        return true;
    }

    public bool Nonempty(JString target)
    {
        var _length = target.Value.Length;
        if(_length <= 0) return FailWith(new JsonSchemaException(
            new ErrorDetail(NEMT01, "String is empty"),
            new ExpectedDetail(Function, "Non empty string"),
            new ActualDetail(target, "found empty string")));
        return true;
    }

    public bool Nonempty(JArray target)
    {
        var _length = target.Elements.Count;
        if(_length <= 0) return FailWith(new JsonSchemaException(
            new ErrorDetail(NEMT02, "Array is empty"),
            new ExpectedDetail(Function, "Non empty array"),
            new ActualDetail(target, "found empty array")));
        return true;
    }

    public bool Nonempty(JObject target)
    {
        var _length = target.Properties.Count;
        if(_length <= 0) return FailWith(new JsonSchemaException(
            new ErrorDetail(NEMT03, "Object is empty"),
            new ExpectedDetail(Function, "Non empty object"),
            new ActualDetail(target, "found empty object")));
        return true;
    }
}