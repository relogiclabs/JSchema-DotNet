using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Tree.TreeType;

namespace RelogicLabs.JsonSchema;

/// <summary>
/// Provides assertion functionalities to validate Json document against a Schema or Json.
/// </summary>
public class JsonAssert
{
    public RuntimeContext Runtime { get; }
    public IDataTree DataTree { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonAssert"/> class for the
    /// specified Schema string.
    /// </summary>
    /// <param name="schema">A Schema string for validation or conformation.</param>
    public JsonAssert(string schema) : this(schema, SCHEMA_TREE) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonAssert"/> class for the
    /// specified <paramref name="expected"/> string which can be either a Schema or a Json
    /// representation.
    /// </summary>
    /// <param name="expected">An expected Schema or Json string for validation or conformation.</param>
    /// <param name="type">The type of string provided by <paramref name="expected"/>, indicating
    /// whether it represents a Schema or Json. Use <see cref="TreeType.SCHEMA_TREE"/> for Schema
    /// and <see cref="TreeType.JSON_TREE"/> for Json.</param>
    public JsonAssert(string expected, TreeType type)
    {
        if(type == SCHEMA_TREE)
        {
            Runtime = new RuntimeContext(MessageFormatter.SchemaAssertion, true);
            DataTree = new SchemaTree(Runtime, expected);
        }
        else
        {
            Runtime = new RuntimeContext(MessageFormatter.JsonAssertion, true);
            DataTree = new JsonTree(Runtime, expected);
        }
    }

    /// <summary>
    /// Tests whether the input JSON string conforms to the Schema specified
    /// in the <see cref="JsonAssert"/> constructor.
    /// </summary>
    /// <param name="jsonActual">The actual JSON to conform or validate.</param>
    public void IsValid(string jsonActual)
    {
        Runtime.Clear();
        JsonTree jsonTree = new(Runtime, jsonActual);
        DebugUtilities.Print(DataTree, jsonTree);
        if(!DataTree.Match(jsonTree))
            throw new InvalidOperationException("Invalid runtime state");
    }

    /// <summary>
    /// Tests whether the specified JSON string conforms to the given Schema string
    /// and throws an exception if the JSON string does not conform to the Schema.
    /// </summary>
    /// <param name="schemaExpected">The expected Schema to conform or validate.</param>
    /// <param name="jsonActual">The actual JSON to conform or validate.</param>
    public static void IsValid(string schemaExpected, string jsonActual)
        => new JsonAssert(schemaExpected).IsValid(jsonActual);

    /// <summary>
    /// Tests if the provided JSON strings are logically equivalent, meaning their structural
    /// composition and internal data are identical. If the JSON strings are not equivalent,
    /// an exception is thrown.
    /// </summary>
    /// <param name="jsonExpected">The expected JSON to compare.</param>
    /// <param name="jsonActual">The actual JSON to compare.</param>
    public static void AreEqual(string jsonExpected, string jsonActual)
        => new JsonAssert(jsonExpected, JSON_TREE).IsValid(jsonActual);
}