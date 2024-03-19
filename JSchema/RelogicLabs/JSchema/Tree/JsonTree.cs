using Antlr4.Runtime;
using RelogicLabs.JSchema.Antlr;
using RelogicLabs.JSchema.Nodes;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Tree.TreeType;

namespace RelogicLabs.JSchema.Tree;

public sealed class JsonTree : IDataTree
{
    public RuntimeContext Runtime { get; }
    public JRoot Root { get; }
    public TreeType Type => JSON_TREE;

    public JsonTree(RuntimeContext runtime, string input)
    {
        Runtime = runtime;
        JsonLexer jsonLexer = new(CharStreams.fromString(input));
        jsonLexer.RemoveErrorListeners();
        jsonLexer.AddErrorListener(LexerErrorListener.Json);
        JsonParser jsonParser = new(new CommonTokenStream(jsonLexer));
        jsonParser.RemoveErrorListeners();
        jsonParser.AddErrorListener(ParserErrorListener.Json);
        Root = (JRoot) new JsonTreeVisitor(runtime).Visit(jsonParser.json());
    }

    public bool Match(IDataTree dataTree)
    {
        var result = Root.Match(dataTree.Root);
        result &= Runtime.InvokeFutures();
        return result;
    }
}