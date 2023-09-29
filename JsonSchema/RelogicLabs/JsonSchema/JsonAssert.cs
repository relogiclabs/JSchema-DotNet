using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Utilities;

namespace RelogicLabs.JsonSchema;

/// <summary>
/// Provides assertion functionalities to validate JSON document against JSON Schema.
/// </summary>
public class JsonAssert
{
    public RuntimeContext Runtime { get; }
    public SchemaTree SchemaTree { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonAssert"/> class for the
    /// specified Schema string.
    /// </summary>
    /// <param name="schema">A Schema string for validation or conformation.</param>
    public JsonAssert(string schema)
    {
        Runtime = new(MessageFormatter.SchemaAssertion, true);
        SchemaTree = new(Runtime, schema);
    }
    
    /// <summary>
    /// Tests whether the input JSON string conforms to the Schema specified
    /// in the <see cref="JsonAssert"/> constructor.
    /// </summary>
    /// <param name="json">The actual JSON to conform or validate.</param>
    public void IsValid(string json)
    {
        Runtime.Exceptions.Clear();
        JsonTree jsonTree = new(Runtime, json);
        DebugUtils.Print(SchemaTree, jsonTree);
        if(!SchemaTree.Root.Match(jsonTree.Root)) 
            throw new InvalidOperationException("Exception not thrown");
    }
    
    /// <summary>
    /// Tests whether the specified JSON string conforms to the given Schema string
    /// and throws an exception if the JSON string does not conform to the Schema.
    /// </summary>
    /// <param name="schemaExpected">The expected Schema to conform or validate.</param>
    /// <param name="jsonActual">The actual JSON to conform or validate.</param>
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
        DebugUtils.Print(expectedTree, actualTree);
        if(!expectedTree.Root.Match(actualTree.Root))
            throw new InvalidOperationException("Exception not thrown");
    }
}