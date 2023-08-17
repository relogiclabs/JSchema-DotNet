using System.Diagnostics.CodeAnalysis;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Tree;

public class Context
{
    public required ParserRuleContext Parser { get; init; }
    public required RuntimeContext Runtime { get; init; }
    internal MessageFormatter MessageFormatter => Runtime.MessageFormatter;

    [SetsRequiredMembers]
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