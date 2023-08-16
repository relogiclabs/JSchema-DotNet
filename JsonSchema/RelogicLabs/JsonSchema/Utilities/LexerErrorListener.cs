using Antlr4.Runtime;
using RelogicLabs.JsonSchema.Exceptions;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Utilities;

internal abstract class LexerErrorListener : IAntlrErrorListener<int>
{
    public static readonly LexerErrorListener SchemaListener = new SchemaLexerErrorListener();
    public static readonly LexerErrorListener JsonListener = new JsonLexerErrorListener();

    protected abstract CommonException CreateException(string message, Exception? innerException);
    protected abstract string GetMessageFormat();
    
    private class SchemaLexerErrorListener : LexerErrorListener
    {
        protected override CommonException CreateException(string message, 
            Exception? innerException) => new SchemaLexerException(SLEX01, 
            message, innerException);

        protected override string GetMessageFormat() 
            => $"Schema (Line {{0}}:{{1}}) [{SLEX01}]: {{2}} (error on {{3}})";
    }
    
    private class JsonLexerErrorListener : LexerErrorListener
    {
        protected override CommonException CreateException(string message, 
            Exception? innerException) => new JsonLexerException(JLEX01, 
            message, innerException);

        protected override string GetMessageFormat() 
            => $"Json (Line {{0}}:{{1}}) [{JLEX01}]: {{2}} (error on {{3}})";
    }
    
    public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, 
        int line, int charPositionInLine, string msg, RecognitionException e)
    {
        var lexer = (Lexer) recognizer;
        var message = string.Format(GetMessageFormat(), line, charPositionInLine, msg, lexer.Text);
        throw CreateException(message, e);
    }
}