using RelogicLabs.JSchema.Nodes;
using RelogicLabs.JSchema.Utilities;

namespace RelogicLabs.JSchema.Tree;

internal sealed record FunctionKey(string FunctionName, int ParameterCount)
{
    public const char EscapedPrefix = '_';

    public FunctionKey(JFunction caller)
        : this(caller.Name, caller.Arguments.Count + 1) { }

    public FunctionKey(IEFunction function)
        : this(FormatName(function.Name), function.Arity) { }

    private static string FormatName(string name)
        => '@' + name.TrimStart(EscapedPrefix).Uncapitalize();
}