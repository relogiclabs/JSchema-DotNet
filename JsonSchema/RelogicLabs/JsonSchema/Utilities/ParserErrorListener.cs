using Antlr4.Runtime;
using RelogicLabs.JsonSchema.Exceptions;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Utilities;

internal abstract class ParserErrorListener : IAntlrErrorListener<IToken>
{

    public static readonly ParserErrorListener Schema = new SchemaParserErrorListener();
    public static readonly ParserErrorListener Json = new JsonParserErrorListener();

    protected abstract CommonException CreateException(string message, Exception? innerException);
    protected abstract string GetMessageFormat();


    private sealed class SchemaParserErrorListener : ParserErrorListener
    {
        protected override CommonException CreateException(string message,
            Exception? innerException) => new SchemaParserException(SPRS01,
            message, innerException);

        protected override string GetMessageFormat()
            => $"Schema (Line {{0}}:{{1}}) [{SPRS01}]: {{2}} (error on '{{3}}')";
    }

    private sealed class JsonParserErrorListener : ParserErrorListener
    {
        protected override CommonException CreateException(string message,
            Exception? innerException) => new JsonParserException(JPRS01,
            message, innerException);

        protected override string GetMessageFormat()
            => $"Json (Line {{0}}:{{1}}) [{JPRS01}]: {{2}} (error on '{{3}}')";
    }

    public void SyntaxError(TextWriter output, IRecognizer recognizer,
        IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        DebugUtilities.Print(recognizer);
        var message = string.Format(GetMessageFormat(), line, charPositionInLine, msg, offendingSymbol);
        throw CreateException(message, e);
    }
}