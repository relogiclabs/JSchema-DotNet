using Antlr4.Runtime;
using RelogicLabs.JsonSchema.Exceptions;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Utilities;

internal abstract class LexerErrorListener : IAntlrErrorListener<int>
{
    public static readonly LexerErrorListener Schema = new SchemaLexerErrorListener();
    public static readonly LexerErrorListener Json = new JsonLexerErrorListener();
    public static readonly LexerErrorListener Date = new DateLexerErrorListener();

    protected abstract CommonException CreateException(string message, Exception? innerException);
    protected abstract string GetMessageFormat();
    
    private class SchemaLexerErrorListener : LexerErrorListener
    {
        protected override CommonException CreateException(string message, 
            Exception? innerException) => new SchemaLexerException(SLEX01, 
            message, innerException);

        protected override string GetMessageFormat() 
            => $"Schema (Line {{0}}:{{1}}) [{SLEX01}]: {{2}} (error on '{{3}}')";
    }
    
    private class JsonLexerErrorListener : LexerErrorListener
    {
        protected override CommonException CreateException(string message, 
            Exception? innerException) => new JsonLexerException(JLEX01, 
            message, innerException);

        protected override string GetMessageFormat() 
            => $"Json (Line {{0}}:{{1}}) [{JLEX01}]: {{2}} (error on '{{3}}')";
    }

    private class DateLexerErrorListener : LexerErrorListener
    {
        protected override CommonException CreateException(string message, 
            Exception? innerException) => new DateLexerException(DLEX01, message, 
            innerException);

        protected override string GetMessageFormat() 
            => "Invalid date pattern ({0}, error on '{1}')";
    }
    
    public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, 
        int line, int charPositionInLine, string msg, RecognitionException e)
    {
        var lexer = (Lexer) recognizer;
        var message = this == Date? 
            string.Format(GetMessageFormat(), msg, lexer.Text) : 
            string.Format(GetMessageFormat(), line, charPositionInLine, msg, lexer.Text);
        throw CreateException(message, e);
    }
}