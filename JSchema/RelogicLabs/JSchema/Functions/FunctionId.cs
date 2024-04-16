using RelogicLabs.JSchema.Nodes;
using RelogicLabs.JSchema.Script;
using RelogicLabs.JSchema.Tree;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Script.IRFunction;
using static RelogicLabs.JSchema.Tree.IEFunction;

namespace RelogicLabs.JSchema.Functions;

internal static class FunctionId
{
    private const char EscapedPrefix = '_';

    public static string Generate(JFunction caller, bool variadic = false)
        => $"{caller.Name}#{(variadic ? VariadicArity : caller.Arguments.Count + 1)}";

    public static string Generate(IEFunction function)
        => $"{ConstraintPrefix}{function.Name.TrimStart(EscapedPrefix)
            .Uncapitalize()}#{function.Arity}";

    public static string Generate(string baseName, GParameter[] parameters, bool constraint)
    {
        var arity = HasVariadic(parameters) ? VariadicArity : parameters.Length;
        return constraint ? $"{ConstraintPrefix}{baseName.TrimStart(EscapedPrefix)}#{arity}"
            : $"{baseName}#{arity}";
    }
}