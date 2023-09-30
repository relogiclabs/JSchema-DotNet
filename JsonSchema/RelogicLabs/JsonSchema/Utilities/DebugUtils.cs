using Antlr4.Runtime;
using RelogicLabs.JsonSchema.Time;
using RelogicLabs.JsonSchema.Tree;

namespace RelogicLabs.JsonSchema.Utilities;

// External logging library may consider
// But create additional dependency
internal static class DebugUtils
{
    public static bool DebugPrint = false;

    internal static void Print(SchemaTree schemaTree, JsonTree jsonTree)
    {
        if(!DebugPrint) return;
        Console.WriteLine("[DEBUG] Schema interpretation:");
        Console.WriteLine(schemaTree.Root);
        Console.WriteLine("---");
        Console.WriteLine("[DEBUG] Json interpretation:");
        Console.WriteLine(jsonTree.Root);
        Console.WriteLine("---");
    }

    internal static void Print(JsonTree expectedTree, JsonTree actualTree)
    {
        if(!DebugPrint) return;
        Console.WriteLine("[DEBUG] Expected Json interpretation:");
        Console.WriteLine(expectedTree.Root);
        Console.WriteLine("---");
        Console.WriteLine("[DEBUG] Actual Json interpretation:");
        Console.WriteLine(actualTree.Root);
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
