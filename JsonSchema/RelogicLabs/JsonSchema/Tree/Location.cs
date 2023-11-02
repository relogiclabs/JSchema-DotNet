using Antlr4.Runtime;

namespace RelogicLabs.JsonSchema.Tree;

public readonly record struct Location(int Line, int Column)
{
    public static Location From(IToken token) => new(token.Line, token.Column);
    public override string ToString() => $"{Line}:{Column}";
}