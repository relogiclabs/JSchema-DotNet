using Antlr4.Runtime;
using RelogicLabs.JsonSchema.Antlr;
using RelogicLabs.JsonSchema.Types;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Tree.TreeType;

namespace RelogicLabs.JsonSchema.Tree;

public sealed class SchemaTree : IDataTree
{
    public RuntimeContext Runtime { get; }
    public JRoot Root { get; }
    public TreeType Type => SCHEMA_TREE;

    public SchemaTree(RuntimeContext context, string input)
    {
        Runtime = context;
        SchemaLexer schemaLexer = new(CharStreams.fromString(input));
        schemaLexer.RemoveErrorListeners();
        schemaLexer.AddErrorListener(LexerErrorListener.Schema);
        SchemaParser schemaParser = new(new CommonTokenStream(schemaLexer));
        schemaParser.RemoveErrorListeners();
        schemaParser.AddErrorListener(ParserErrorListener.Schema);
        Root = (JRoot) new SchemaTreeVisitor(context).Visit(schemaParser.schema());
    }

    public bool Match(IDataTree dataTree)
    {
        var result = Root.Match(dataTree.Root);
        result &= Runtime.InvokeValidators();
        return result;
    }
}