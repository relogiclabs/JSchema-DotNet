using System.Text;
using Antlr4.Runtime;
using RelogicLabs.JSchema.Exceptions;
using static RelogicLabs.JSchema.Message.ErrorCode;

namespace RelogicLabs.JSchema.Utilities;

internal abstract class LexerErrorListener : IAntlrErrorListener<int>
{
    private static readonly string[] NewLines = { "\r\n", "\r", "\n" };

    public static readonly LexerErrorListener Schema = new SchemaErrorListener();
    public static readonly LexerErrorListener Json = new JsonErrorListener();
    public static readonly LexerErrorListener DateTime = new DateTimeErrorListener();

    protected abstract CommonException FailOnSyntaxError(string message, Exception? innerException);
    protected abstract string GetMessageFormat();

    private sealed class SchemaErrorListener : LexerErrorListener
    {
        protected override SchemaLexerException FailOnSyntaxError(string message,
            Exception? innerException) => new(SLEX01, message, innerException);

        protected override string GetMessageFormat()
            => $"Schema (Line {{0}}:{{1}}) [{SLEX01}]: {{2}} (Line: {{3}})";
    }

    private sealed class JsonErrorListener : LexerErrorListener
    {
        protected override JsonLexerException FailOnSyntaxError(string message,
            Exception? innerException) => new(JLEX01, message, innerException);

        protected override string GetMessageFormat()
            => $"Json (Line {{0}}:{{1}}) [{JLEX01}]: {{2}} (Line: {{3}})";
    }

    private sealed class DateTimeErrorListener : LexerErrorListener
    {
        protected override DateTimeLexerException FailOnSyntaxError(string message,
            Exception? innerException) => new(DLEX01, message, innerException);

        protected override string GetMessageFormat()
            => "Invalid date-time pattern ({0}, error on '{1}')";
    }

    public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol,
                int line, int charPositionInLine, string msg, RecognitionException e)
    {
        if(this == DateTime) throw FailOnSyntaxError(string.Format(GetMessageFormat(),
            msg, ((Lexer) recognizer).Text), e);
        var errorLine = recognizer.InputStream.ToString()!
            .Split(NewLines, StringSplitOptions.None)[line - 1]
            .Insert(charPositionInLine, "<|>").Trim();
        throw FailOnSyntaxError(string.Format(GetMessageFormat(), line, charPositionInLine,
            msg, errorLine), e);
    }
}