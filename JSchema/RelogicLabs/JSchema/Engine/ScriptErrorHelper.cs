using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Script;
using RelogicLabs.JSchema.Types;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.MessageFormatter;

namespace RelogicLabs.JSchema.Engine;

internal static class ScriptErrorHelper
{
    internal const string OpAddition = "addition";
    internal const string OpSubtraction = "subtraction";
    internal const string OpMultiplication = "multiplication";
    internal const string OpDivision = "division";
    internal const string OpModulus = "modulus";
    internal const string OpUnaryPlus = "unary plus";
    internal const string OpUnaryMinus = "unary minus";
    internal const string OpBracket = "bracket";
    internal const string OpIndex = "index";
    internal const string OpRange = "range";
    internal const string OpRangeSetup = "range setup";
    internal const string OpComparison = "comparison";
    internal const string OpProperty = "property access";
    internal const string OpIncrement = "increment";
    internal const string OpDecrement = "decrement";
    internal const string OpBracketedAssignment = "bracketed assignment";
    internal const string OpPropertyAssignment = "property assignment";
    internal const string OpAdditionAssignment = "addition assignment";
    internal const string OpSubtractionAssignment = "subtraction assignment";
    internal const string OpMultiplicationAssignment = "multiplication assignment";
    internal const string OpDivisionAssignment = "division assignment";
    internal const string OpModulusAssignment = "modulus assignment";

    internal static InvalidFunctionException FailOnDuplicateParameterName(ITerminalNode node)
        => new(FormatForSchema(FPRM01, $"Duplicate parameter name '{node.GetText()}'", node.Symbol));

    internal static ScriptRuntimeException FailOnInvalidReturnType(IEValue value, IToken token)
        => new(FormatForSchema(RETN01, $"Invalid return type {value.Type} for constraint function",
            token));

    internal static ScriptRuntimeException FailOnPropertyNotExist(string code, IEObject instance,
                string property, IToken token)
        => new(FormatForSchema(code, $"Property '{property}' not exist in {instance.Type}", token));

    internal static Exception FailOnIndexOutOfBounds(IEString value, IEInteger index, IToken token,
                Exception innerException)
    {
        var indexValue = index.Value;
        if(indexValue < 0) return new ScriptRuntimeException(FormatForSchema(SIDX02,
                $"Invalid negative index {index} for string starts at 0", token), innerException);
        var length = value.Length;
        if(indexValue >= length) return new ScriptRuntimeException(FormatForSchema(SIDX01,
                $"Index {index} out of bounds for string length {length}", token), innerException);
        return innerException;
    }

    internal static Exception FailOnIndexOutOfBounds(IEArray value, IEInteger index, IToken token,
                Exception innerException)
    {
        var indexValue = index.Value;
        if(indexValue < 0) return new ScriptRuntimeException(FormatForSchema(AIDX02,
                $"Invalid negative index {index} for array starts at 0", token), innerException);
        var count = value.Count;
        if(indexValue >= count) return new ScriptRuntimeException(FormatForSchema(AIDX01,
                $"Index {index} out of bounds for array length {count}", token), innerException);
        return innerException;
    }

    internal static Exception FailOnInvalidRangeIndex(IEString value, GRange range, IToken token,
                Exception innerException)
    {
        int length = value.Length, start = range.GetStart(length), end = range.GetEnd(length);
        if(start < 0 || start > length) throw new ScriptRuntimeException(FormatForSchema(SRNG01,
                $"Range start index {start} out of bounds for string length {length}",
                token), innerException);
        if(end < 0 || end > length) return new ScriptRuntimeException(FormatForSchema(SRNG02,
                $"Range end index {end} out of bounds for string length {length}",
                token), innerException);
        if(start > end) return new ScriptRuntimeException(FormatForSchema(SRNG03,
                $"Range start index {start} > end index {end} for string length {length}",
                token), innerException);
        return innerException;
    }

    internal static Exception FailOnInvalidRangeIndex(IEArray value, GRange range, IToken token,
                Exception innerException)
    {
        int count = value.Count, start = range.GetStart(count), end = range.GetEnd(count);
        if(start < 0 || start > count) throw new ScriptRuntimeException(FormatForSchema(ARNG01,
                $"Range start index {start} out of bounds for array length {count}",
                token), innerException);
        if(end < 0 || end > count) return new ScriptRuntimeException(FormatForSchema(ARNG02,
                $"Range end index {end} out of bounds for array length {count}",
                token), innerException);
        if(start > end) return new ScriptRuntimeException(FormatForSchema(ARNG03,
                $"Range start index {start} > end index {end} for array length {count}",
                token), innerException);
        return innerException;
    }

    internal static ScriptRuntimeException FailOnInvalidLValueIncrement(string code, IToken token)
        => new(FormatForSchema(code, "Invalid l-value for increment (readonly)", token));

    internal static ScriptRuntimeException FailOnInvalidLValueDecrement(string code, IToken token)
        => new(FormatForSchema(code, "Invalid l-value for decrement (readonly)", token));

    internal static ScriptRuntimeException FailOnInvalidLValueAssignment(string code, IToken token)
        => new(FormatForSchema(code, "Invalid l-value for assignment (readonly)", token));

    internal static ScriptInvocationException FailOnTargetNotFound(IToken token)
        => new(TRGT01, "Target not found for function '~%0'", token);

    internal static ScriptInvocationException FailOnCallerNotFound(IToken token)
        => new(CALR01, "Caller not found for function '~%0'", token);

    internal static ScriptRuntimeException FailOnIdentifierNotFound(string code, IToken identifier)
        => new(FormatForSchema(code, $"Identifier '{identifier.Text}' not found", identifier));

    internal static ScriptRuntimeException FailOnFunctionNotFound(string name, int argCount,
                IToken token)
        => new(FormatForSchema(FNVK01, $"Function '{name}' with {argCount} parameter(s) not found",
            token));

    internal static ScriptRuntimeException FailOnRuntime(string code, string message, IToken token,
                Exception innerException)
        => new(FormatForSchema(code, message, token), innerException);

    internal static ScriptInvocationException FailOnVariadicArity(string code)
        => new(code, "Too few arguments for invocation of variadic function '~%0'");

    internal static ScriptInvocationException FailOnFixedArity(string code)
        => new(code, "Invalid number of arguments for function '~%0'");

    internal static SystemOperationException FailOnSystemException(string code,
                Exception exception, IToken token)
        => new(FormatForSchema(code, exception.Message, token), exception);

    internal static ScriptOperationException FailOnOperation(string code, string operation,
                IEValue value, IToken token, Exception? innerException = null)
        => new(FormatForSchema(code, $"Invalid {operation} operation on type {
            value.Type}", token), innerException);

    internal static ScriptRuntimeException FailOnOperation(string code, string operation,
                IEValue value, ITerminalNode node)
        => FailOnOperation(code, operation, value, node.Symbol);

    internal static ScriptOperationException FailOnOperation(string code, string operation,
                IEValue value1, IEValue value2, IToken token, Exception? innerException = null)
        => new(FormatForSchema(code, $"Invalid {operation} operation on types {
            value1.Type} and {value2.Type}", token), innerException);

    internal static ScriptRuntimeException FailOnOperation(string code, string operation,
                IEValue value1, IEValue value2, ITerminalNode node)
        => FailOnOperation(code, operation, value1, value2, node.Symbol);

    internal static ScriptRuntimeException FailOnStringUpdate(string code, ITerminalNode node)
        => new(FormatForSchema(code, "Immutable string characters cannot be updated", node.Symbol));

    internal static ScriptRuntimeException FailOnArrayRangeUpdate(string code, ITerminalNode node)
        => new(FormatForSchema(code, "Update a range of elements in array is not supported",
            node.Symbol));
}