using Antlr4.Runtime;
using RelogicLabs.JsonSchema.Date;
using RelogicLabs.JsonSchema.Tree;

namespace RelogicLabs.JsonSchema.Utilities;

// External logging library may consider
// But create additional dependency
public static class DebugUtils
{
    public static bool DebugPrint = false;
    
    internal static void Print(SchemaTree schemaTree, JsonTree jsonTree)
    {
        if(!DebugPrint) return;
        Console.WriteLine("Schema Interpretation:");
        Console.WriteLine(schemaTree.Root.ToJson());
        Console.WriteLine("---");
        Console.WriteLine("Json Interpretation:");
        Console.WriteLine(jsonTree.Root.ToJson());
        Console.WriteLine("---");
    }

    internal static void Print(IRecognizer recognizer)
    {
        if(!DebugPrint) return;
        IList<string> stack = ((Parser) recognizer).GetRuleInvocationStack();
        Console.Error.WriteLine($"Rule Stack: {stack.Reverse().ToString(" > ")}");
    }

    internal static void Print(DateContext dateContext)
    {
        if(!DebugPrint) return;
        Console.WriteLine($"Date Parsed: {dateContext}");
    }
}
