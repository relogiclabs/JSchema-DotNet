using Antlr4.Runtime;
using RelogicLabs.JsonSchema.Antlr;
using RelogicLabs.JsonSchema.Types;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Tree.TreeType;

namespace RelogicLabs.JsonSchema.Tree;

public sealed class JsonTree : IDataTree
{
    public RuntimeContext Runtime { get; }
    public JRoot Root { get; }
    public TreeType Type => JSON_TREE;

    public JsonTree(RuntimeContext context, string input)
    {
        Runtime = context;
        JsonLexer jsonLexer = new(CharStreams.fromString(input));
        jsonLexer.RemoveErrorListeners();
        jsonLexer.AddErrorListener(LexerErrorListener.Json);
        JsonParser jsonParser = new(new CommonTokenStream(jsonLexer));
        jsonParser.RemoveErrorListeners();
        jsonParser.AddErrorListener(ParserErrorListener.Json);
        Root = (JRoot) new JsonTreeVisitor(context).Visit(jsonParser.json());
    }

    public bool Match(IDataTree dataTree)
    {
        var result = Root.Match(dataTree.Root);
        result &= Runtime.InvokeValidators();
        return result;
    }
}