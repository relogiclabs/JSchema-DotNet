using Antlr4.Runtime;
using RelogicLabs.JsonSchema.Antlr;
using RelogicLabs.JsonSchema.Types;
using RelogicLabs.JsonSchema.Utilities;

namespace RelogicLabs.JsonSchema.Tree;

public sealed class JsonTree
{
    public JRoot Root { get; }

    public JsonTree(RuntimeContext context, string input)
    {
        JsonLexer jsonLexer = new(CharStreams.fromString(input));
        jsonLexer.RemoveErrorListeners();
        jsonLexer.AddErrorListener(LexerErrorListener.Json);
        JsonParser jsonParser = new(new CommonTokenStream(jsonLexer));
        jsonParser.RemoveErrorListeners();
        jsonParser.AddErrorListener(ParserErrorListener.Json);
        Root = (JRoot) new JsonTreeVisitor(context).Visit(jsonParser.json());
    }
}