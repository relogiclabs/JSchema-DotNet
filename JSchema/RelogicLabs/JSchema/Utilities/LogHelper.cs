using Antlr4.Runtime;
using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Time;
using RelogicLabs.JSchema.Tree;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.MessageFormatter;

namespace RelogicLabs.JSchema.Utilities;

// Logging library may require
internal static class LogHelper
{
    public const int ERROR = 3;
    public const int INFO = 2;
    public const int DEBUG = 1;

    public static int Level = ERROR;

    internal static void Debug(IDataTree expected, IDataTree actual)
    {
        if(Level > DEBUG) return;
        Console.WriteLine($"[DEBUG] Expected {expected.Type} tree interpretation:");
        Console.WriteLine(expected.Root);
        Console.WriteLine("---");
        Console.WriteLine($"[DEBUG] Actual {actual.Type} tree interpretation:");
        Console.WriteLine(actual.Root);
        Console.WriteLine("---");
    }

    internal static void Debug(IRecognizer recognizer)
    {
        if(Level > DEBUG) return;
        IList<string> stack = ((Parser) recognizer).GetRuleInvocationStack();
        Console.Error.WriteLine($"[DEBUG] Rule stack: {stack.Reverse().Join(" > ")}");
    }

    internal static void Debug(DateTimeContext context)
    {
        if(Level > DEBUG) return;
        Console.WriteLine($"[DEBUG] Date and time interpretation: {context}");
    }

    internal static void Debug(Exception exception)
    {
        if(Level > DEBUG) return;
        Console.Error.WriteLine("[DEBUG] Print of exception: " + exception);
    }

    internal static void Log(Exception exception, IToken token)
    {
        if(Level > INFO) return;
        Console.WriteLine("[INFO] [TRYOF ERROR]: " + exception.Message);
        if(Level > DEBUG) return;
        var ex = exception is CommonException ? exception
            : new ScriptRuntimeException(FormatForSchema(TRYS01,
                exception.Message, token), exception);
        Console.WriteLine("[DEBUG] [TRYOF ERROR]: " + ex);
    }
}