using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace RelogicLabs.JSchema.Tree;

public sealed class Context
{
    public ParserRuleContext Parser { get; }
    public RuntimeContext Runtime { get; }

    public Context(ParserRuleContext parser, RuntimeContext runtime)
    {
        Parser = parser;
        Runtime = runtime;
    }

    public Location GetLocation()
    {
        var token = Parser.Start;
        return new Location(token.Line, token.Column);
    }

    public string GetText()
    {
        IToken start = Parser.Start;
        IToken stop = Parser.Stop;
        if(start == null || stop == null) return string.Empty;
        ICharStream cs = start.TokenSource.InputStream;
        return cs.GetText(new Interval(start.StartIndex, stop.StopIndex));
    }
}