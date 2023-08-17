using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Utilities;

namespace RelogicLabs.JsonSchema;

public static class JsonAssert
{
    /// <summary>
    /// Tests whether the specified JSON string conforms to the given Schema string
    /// and throws an exception if the JSON string does not conform to the Schema.
    /// </summary>
    /// <param name="schemaExpected">The expected Schema to compare.</param>
    /// <param name="jsonActual">The actual JSON to compare.</param>
    public static void IsValid(string schemaExpected, string jsonActual)
    {
        RuntimeContext runtime = new(MessageFormatter.SchemaAssertion, true);
        SchemaTree schemaTree = new(runtime, schemaExpected);
        JsonTree jsonTree = new(runtime, jsonActual);
        DebugUtils.Print(schemaTree, jsonTree);
        if(!schemaTree.Root.Match(jsonTree.Root)) 
            throw new InvalidOperationException("Exception not thrown");
    }

    /// <summary>
    /// Tests if the provided JSON strings are logically equivalent, meaning their structural
    /// composition and internal data are identical. If the JSON strings are not equivalent,
    /// an exception is thrown.
    /// </summary>
    /// <param name="jsonExpected">The expected JSON to compare.</param>
    /// <param name="jsonActual">The actual JSON to compare.</param>
    public static void AreEqual(string jsonExpected, string jsonActual)
    {
        RuntimeContext runtime = new(MessageFormatter.JsonAssertion, true);
        JsonTree expectedTree = new(runtime, jsonExpected);
        JsonTree actualTree = new(runtime, jsonActual);
        if(!expectedTree.Root.Match(actualTree.Root))
            throw new InvalidOperationException("Exception not thrown");
    }
}