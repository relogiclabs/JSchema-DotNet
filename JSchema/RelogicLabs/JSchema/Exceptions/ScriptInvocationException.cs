using Antlr4.Runtime;

namespace RelogicLabs.JSchema.Exceptions;

public class ScriptInvocationException : ScriptTemplateException
{
    public IToken? Token { get; }

    public ScriptInvocationException(string code, string template, IToken? token = null,
        Exception? innerException = null)
        : base(code, template, innerException) => Token = token;

    public IToken GetToken(IToken defaultToken) => Token ?? defaultToken;
}