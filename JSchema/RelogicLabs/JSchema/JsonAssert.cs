using RelogicLabs.JSchema.Tree;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Message.MessageFormatter;
using static RelogicLabs.JSchema.Tree.TreeType;

namespace RelogicLabs.JSchema;

/// <summary>
/// Provides assertion functionalities to validate JSON document against a JSchema or JSON document.
/// </summary>
public class JsonAssert
{
    public RuntimeContext Runtime { get; }
    public IDataTree ExpectedTree { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonAssert"/> class for the
    /// specified JSchema string.
    /// </summary>
    /// <param name="schema">A JSchema string for validation or conformation.</param>
    public JsonAssert(string schema) : this(schema, SCHEMA_TREE) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonAssert"/> class for the specified
    /// <paramref name="expected"/> string which can be either a JSchema or a JSON representation.
    /// </summary>
    /// <param name="expected">An expected JSchema or JSON string for validation or conformation.</param>
    /// <param name="type">The type of string provided by <paramref name="expected"/>, indicating
    /// whether it represents a JSchema or JSON. Use <see cref="TreeType.SCHEMA_TREE"/> for JSchema
    /// and <see cref="TreeType.JSON_TREE"/> for JSON.</param>
    public JsonAssert(string expected, TreeType type)
    {
        if(type == SCHEMA_TREE)
        {
            Runtime = new RuntimeContext(SchemaAssertion, true);
            ExpectedTree = new SchemaTree(Runtime, expected);
        }
        else
        {
            Runtime = new RuntimeContext(JsonAssertion, true);
            ExpectedTree = new JsonTree(Runtime, expected);
        }
    }

    /// <summary>
    /// Tests whether the input JSON string conforms to the expected JSchema or JSON
    /// specified in the <see cref="JsonAssert"/> constructor.
    /// </summary>
    /// <param name="json">The actual JSON to conform or validate.</param>
    public void IsValid(string json)
    {
        Runtime.Clear();
        JsonTree jsonTree = new(Runtime, json);
        LogHelper.Debug(ExpectedTree, jsonTree);
        if(!ExpectedTree.Match(jsonTree))
            throw new InvalidOperationException("Invalid runtime state");
    }

    /// <summary>
    /// Tests whether the specified JSON string conforms to the given JSchema string
    /// and throws an exception if the JSON string does not conform to the JSchema.
    /// </summary>
    /// <param name="schema">The expected JSchema to conform or validate.</param>
    /// <param name="json">The actual JSON to conform or validate.</param>
    public static void IsValid(string schema, string json)
        => new JsonAssert(schema).IsValid(json);

    /// <summary>
    /// Tests if the provided JSON strings are logically equivalent, meaning their structural
    /// composition and internal data are identical. If the JSON strings are not equivalent,
    /// an exception is thrown.
    /// </summary>
    /// <param name="expected">The expected JSON to compare.</param>
    /// <param name="actual">The actual JSON to compare.</param>
    public static void AreEqual(string expected, string actual)
        => new JsonAssert(expected, JSON_TREE).IsValid(actual);
}