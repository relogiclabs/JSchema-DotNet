using Antlr4.Runtime;
using RelogicLabs.JsonSchema.Time;
using RelogicLabs.JsonSchema.Tree;

namespace RelogicLabs.JsonSchema.Utilities;

// External logging library may consider
// Kept it lightweight for now
internal static class DebugUtilities
{
    public static bool DebugPrint = false;

    internal static void Print(IDataTree expected, IDataTree actual)
    {
        if(!DebugPrint) return;
        Console.WriteLine($"[DEBUG] Expected {expected.Type} tree interpretation:");
        Console.WriteLine(expected.Root);
        Console.WriteLine("---");
        Console.WriteLine($"[DEBUG] Actual {actual.Type} tree interpretation:");
        Console.WriteLine(actual.Root);
        Console.WriteLine("---");
    }

    internal static void Print(IRecognizer recognizer)
    {
        if(!DebugPrint) return;
        IList<string> stack = ((Parser) recognizer).GetRuleInvocationStack();
        Console.Error.WriteLine($"[DEBUG] Rule stack: {stack.Reverse().Join(" > ")}");
    }

    internal static void Print(DateTimeContext context)
    {
        if(!DebugPrint) return;
        Console.WriteLine($"[DEBUG] Date and time interpretation: {context}");
    }

    internal static void Print(Exception exception)
    {
        if(!DebugPrint) return;
        Console.Error.WriteLine("[DEBUG] Print of exception: " + exception);
    }
}