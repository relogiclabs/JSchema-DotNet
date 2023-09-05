using Antlr4.Runtime;
using RelogicLabs.JsonSchema.Antlr;
using RelogicLabs.JsonSchema.Types;
using RelogicLabs.JsonSchema.Utilities;

namespace RelogicLabs.JsonSchema.Tree;

public class SchemaTree
{
    public JRoot Root { get; }
    
    public SchemaTree(RuntimeContext context, string input)
    {
        SchemaLexer schemaLexer = new(CharStreams.fromString(input));
        schemaLexer.RemoveErrorListeners();
        schemaLexer.AddErrorListener(LexerErrorListener.Schema);
        SchemaParser schemaParser = new(new CommonTokenStream(schemaLexer));
        schemaParser.RemoveErrorListeners();
        schemaParser.AddErrorListener(ParserErrorListener.Schema);
        Root = (JRoot) new SchemaTreeVisitor(context).Visit(schemaParser.schema());
    }
}