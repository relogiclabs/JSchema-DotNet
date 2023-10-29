using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Utilities;

namespace RelogicLabs.JsonSchema;

/// <summary>
/// Provides Schema validation functionalities for JSON document.
/// </summary>
public class JsonSchema
{
    public RuntimeContext Runtime { get; }
    public SchemaTree SchemaTree { get; }
    public Queue<Exception> Exceptions { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonSchema"/> class for the
    /// specified Schema string.
    /// </summary>
    /// <param name="schema">A Schema string for validation or conformation.</param>
    public JsonSchema(string schema)
    {
        Runtime = new(MessageFormatter.SchemaValidation, false);
        Exceptions = Runtime.Exceptions;
        SchemaTree = new(Runtime, schema);
    }

    /// <summary>
    /// Indicates whether the input JSON string conforms to the Schema specified
    /// in the <see cref="JsonSchema"/> constructor.
    /// </summary>
    /// <param name="json">The JSON string to conform or validate with Schema.</param>
    /// <returns><c>true</c> if the JSON string conforms to the Schema; otherwise,
    /// <c>false</c>.</returns>
    public bool IsValid(string json)
    {
        Exceptions.Clear();
        JsonTree jsonTree = new(Runtime, json);
        DebugUtilities.Print(SchemaTree, jsonTree);
        var result = SchemaTree.Root.Match(jsonTree.Root);
        return result;
    }

    /// <summary>
    /// Writes error messages that occur during Schema validation process, to the
    /// standard error stream.
    /// </summary>
    public void WriteError()
    {
        foreach(var exception in Exceptions)
            Console.Error.WriteLine(exception.Message);
    }
    
    /// <summary>
    /// Indicates whether the input JSON string conforms to the given Schema string.
    /// </summary>
    /// <param name="schema">The Schema string to conform or validate.</param>
    /// <param name="json">The JSON string to conform or validate.</param>
    /// <returns><c>true</c> if the JSON string conforms to the Schema; otherwise,
    /// <c>false</c>.</returns>
    public static bool IsValid(string schema, string json)
    {
        JsonSchema jsonSchema = new(schema);
        var result = jsonSchema.IsValid(json);
        if(!result) jsonSchema.WriteError();
        return result;
    }
}