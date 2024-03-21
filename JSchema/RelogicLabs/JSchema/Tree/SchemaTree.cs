using Antlr4.Runtime;
using RelogicLabs.JSchema.Antlr;
using RelogicLabs.JSchema.Engine;
using RelogicLabs.JSchema.Nodes;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Tree.TreeType;

namespace RelogicLabs.JSchema.Tree;

public sealed class SchemaTree : IDataTree
{
    public RuntimeContext Runtime { get; }
    public JRoot Root { get; }
    public TreeType Type => SCHEMA_TREE;

    public SchemaTree(RuntimeContext runtime, string input)
    {
        Runtime = runtime;
        var schemaLexer = new SchemaLexer(CharStreams.fromString(input));
        schemaLexer.RemoveErrorListeners();
        schemaLexer.AddErrorListener(LexerErrorListener.Schema);
        var schemaParser = new SchemaParser(new CommonTokenStream(schemaLexer));
        schemaParser.RemoveErrorListeners();
        schemaParser.AddErrorListener(ParserErrorListener.Schema);
        var schemaParseTree = schemaParser.schema();
        var scriptVisitor = new ScriptTreeVisitor(runtime);
        var evaluator = scriptVisitor.Visit(schemaParseTree);
        Root = (JRoot) new SchemaTreeVisitor(scriptVisitor).Visit(schemaParseTree);
        evaluator(runtime.ScriptContext);
    }

    public bool Match(IDataTree dataTree)
    {
        var result = Root.Match(dataTree.Root);
        result &= Runtime.InvokeFutures();
        return result;
    }
}