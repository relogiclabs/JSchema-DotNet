using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Script;
using RelogicLabs.JSchema.Types;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.MessageFormatter;
using static RelogicLabs.JSchema.Message.OutlineFormatter;

namespace RelogicLabs.JSchema.Engine;

internal static class ScriptErrorHelper
{
    internal const string ADDITION = "addition";
    internal const string SUBTRACTION = "subtraction";
    internal const string MULTIPLICATION = "multiplication";
    internal const string DIVISION = "division";
    internal const string INDEX = "index";
    internal const string RANGE = "range";
    internal const string COMPARISON = "comparison";
    internal const string PROPERTY = "property access";
    internal const string INCREMENT = "increment";
    internal const string DECREMENT = "decrement";
    internal const string NEGATION = "negation";

    internal static InvalidFunctionException FailOnDuplicateParameterName(ITerminalNode node)
        => new(FormatForSchema(FUNS01, $"Duplicate parameter name '{node.GetText()}'", node.Symbol));

    internal static ScriptRuntimeException FailOnInvalidReturnType(IEValue value, IToken token)
        => new(FormatForSchema(RETN01, $"Invalid return type {
            value.Type} for constraint function", token));

    internal static ScriptRuntimeException FailOnPropertyNotExist(IEObject source, string property,
                IToken token)
        => new(FormatForSchema(PRPS02, $"Property '{property}' not exist in readonly {
            source.Type}", token));

    internal static Exception FailOnIndexOutOfBounds(IEString source, IEInteger index, IToken token,
                Exception innerException)
    {
        var indexValue = index.Value;
        if(indexValue < 0) return new ScriptRuntimeException(FormatForSchema(INDX03,
                $"Invalid negative index {index} for string starts at 0", token), innerException);
        var length = source.Length;
        if(indexValue >= length) return new ScriptRuntimeException(FormatForSchema(INDX02,
                $"Index {index} out of bounds for string length {length}", token), innerException);
        return innerException;
    }

    internal static Exception FailOnIndexOutOfBounds(IEArray array, IEInteger index, IToken token,
                Exception innerException)
    {
        var indexValue = index.Value;
        if(indexValue < 0) return new ScriptRuntimeException(FormatForSchema(INDX05,
                $"Invalid negative index {index} for array starts at 0", token), innerException);
        var count = array.Count;
        if(indexValue >= count) return new ScriptRuntimeException(FormatForSchema(INDX04,
                $"Index {index} out of bounds for array length {count}", token), innerException);
        return innerException;
    }

    internal static Exception FailOnInvalidRangeIndex(IEString source, GRange range, IToken token,
                Exception innerException)
    {
        int length = source.Length, start = range.GetStart(length), end = range.GetEnd(length);
        if(start < 0 || start > length) throw new ScriptRuntimeException(FormatForSchema(RNGS04,
                $"Range start index {start} out of bounds for string length {length}",
                token), innerException);
        if(end < 0 || end > length) return new ScriptRuntimeException(FormatForSchema(RNGS05,
                $"Range end index {end} out of bounds for string length {length}",
                token), innerException);
        if(start > end) return new ScriptRuntimeException(FormatForSchema(RNGS06,
                $"Range start index {start} > end index {end} for string {CreateOutline(source)}",
                token), innerException);
        return innerException;
    }

    internal static Exception FailOnInvalidRangeIndex(IEArray array, GRange range, IToken token,
                Exception innerException)
    {
        int count = array.Count, start = range.GetStart(count), end = range.GetEnd(count);
        if(start < 0 || start > count) throw new ScriptRuntimeException(FormatForSchema(RNGS07,
                $"Range start index {start} out of bounds for array length {count}",
                token), innerException);
        if(end < 0 || end > count) return new ScriptRuntimeException(FormatForSchema(RNGS08,
                $"Range end index {end} out of bounds for array length {count}",
                token), innerException);
        if(start > end) return new ScriptRuntimeException(FormatForSchema(RNGS09,
                $"Range start index {start} > end index {end} for array {CreateOutline(array)}",
                token), innerException);
        return innerException;
    }

    internal static ScriptRuntimeException FailOnInvalidLValueIncrement(string code, IToken token)
        => new(FormatForSchema(code, "Invalid l-value for increment", token));

    internal static ScriptRuntimeException FailOnInvalidLValueDecrement(string code, IToken token)
        => new(FormatForSchema(code, "Invalid l-value for decrement", token));

    internal static ScriptRuntimeException FailOnInvalidLValueAssignment(IToken token)
        => new(FormatForSchema(ASIN01, "Invalid l-value for assignment", token));

    internal static ScriptInvocationException FailOnTargetNotFound(IToken token)
        => new(TRGT01, "Target not found for function '~%0'", token);

    internal static ScriptInvocationException FailOnCallerNotFound(IToken token)
        => new(CALR01, "Caller not found for function '~%0'", token);

    internal static ScriptRuntimeException FailOnIdentifierNotFound(IToken identifier)
        => new(FormatForSchema(VARD02, $"Identifier '{identifier.Text}' not found", identifier));

    internal static ScriptRuntimeException FailOnFunctionNotFound(string name, int arity,
                IToken token)
        => new(FormatForSchema(FUNS04, $"Function '{name}' with {arity} parameter(s) not found",
            token));

    internal static ScriptRuntimeException FailOnRuntime(string code, string message, IToken token,
                Exception innerException)
        => new(FormatForSchema(code, message, token), innerException);

    internal static ScriptInvocationException FailOnVariadicArgument(string code)
        => new(code, "Too few arguments for invocation of variadic function '~%0'");

    internal static ScriptInvocationException FailOnFixedArgument(string code)
        => new(code, "Invalid number of arguments for function '~%0'");

    internal static SystemOperationException FailOnSystemException(string code,
                Exception exception, IToken token)
        => new(FormatForSchema(code, exception.Message, token), exception);

    internal static ScriptOperationException FailOnOperation(string code, string operationName,
                IEValue value, IToken token, Exception? innerException = null)
        => new(FormatForSchema(code, $"Invalid {operationName} operation on type {
            value.Type}", token), innerException);

    internal static ScriptRuntimeException FailOnOperation(string code, string operationName,
                IEValue value, ITerminalNode node)
        => FailOnOperation(code, operationName, value, node.Symbol);

    internal static ScriptOperationException FailOnOperation(string code, string operationName,
                IEValue value1, IEValue value2, IToken token, Exception? innerException = null)
        => new(FormatForSchema(code, $"Invalid {operationName} operation on types {
            value1.Type} and {value2.Type}", token), innerException);

    internal static ScriptRuntimeException FailOnOperation(string code, string operationName,
                IEValue value1, IEValue value2, ITerminalNode node)
        => FailOnOperation(code, operationName, value1, value2, node.Symbol);
}