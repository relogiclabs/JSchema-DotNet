using System.Text;
using Antlr4.Runtime;
using RelogicLabs.JSchema.Exceptions;
using static RelogicLabs.JSchema.Message.ErrorCode;

namespace RelogicLabs.JSchema.Utilities;

internal abstract class ParserErrorListener : IAntlrErrorListener<IToken>
{
    private static readonly string[] NewLines = { "\r\n", "\r", "\n" };

    public static readonly ParserErrorListener Schema = new SchemaErrorListener();
    public static readonly ParserErrorListener Json = new JsonErrorListener();

    protected abstract CommonException FailOnSyntaxError(string message, Exception? innerException);
    protected abstract string GetMessageFormat();


    private sealed class SchemaErrorListener : ParserErrorListener
    {
        protected override SchemaParserException FailOnSyntaxError(string message,
            Exception? innerException) => new(SPRS01, message, innerException);

        protected override string GetMessageFormat()
            => $"Schema (Line {{0}}:{{1}}) [{SPRS01}]: {{2}} (Line: {{3}})";
    }

    private sealed class JsonErrorListener : ParserErrorListener
    {
        protected override JsonParserException FailOnSyntaxError(string message,
            Exception? innerException) => new(JPRS01, message, innerException);

        protected override string GetMessageFormat()
            => $"Json (Line {{0}}:{{1}}) [{JPRS01}]: {{2}} (Line: {{3}})";
    }

    public void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol,
                int line, int charPositionInLine, string msg, RecognitionException e)
    {
        LogHelper.Debug(recognizer);
        var errorLine = ((CommonTokenStream) recognizer.InputStream)
            .TokenSource.InputStream.ToString()!
            .Split(NewLines, StringSplitOptions.None)[line - 1]
            .Insert(charPositionInLine, "<|>").Trim();
        throw FailOnSyntaxError(string.Format(GetMessageFormat(), line, charPositionInLine,
            msg, errorLine), e);
    }
}