using RelogicLabs.JSchema.Tree;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Message.MessageFormatter;

namespace RelogicLabs.JSchema;

/// <summary>
/// Provides JSchema validation functionalities for JSON document.
/// </summary>
public class JsonSchema
{
    public RuntimeContext Runtime { get; }
    public SchemaTree SchemaTree { get; }
    public ExceptionRegistry Exceptions { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonSchema"/> class for the
    /// specified JSchema string.
    /// </summary>
    /// <param name="schema">A JSchema string for validation or conformation.</param>
    public JsonSchema(string schema)
    {
        Runtime = new(SchemaValidation, false);
        Exceptions = Runtime.Exceptions;
        SchemaTree = new(Runtime, schema);
    }

    /// <summary>
    /// Indicates whether the input JSON string conforms to the JSchema specified
    /// in the <see cref="JsonSchema"/> constructor.
    /// </summary>
    /// <param name="json">The JSON string to conform or validate with JSchema.</param>
    /// <returns><c>true</c> if the JSON string conforms to the JSchema; otherwise,
    /// <c>false</c>.</returns>
    public bool IsValid(string json)
    {
        Runtime.Clear();
        JsonTree jsonTree = new(Runtime, json);
        LogHelper.Debug(SchemaTree, jsonTree);
        var result = SchemaTree.Match(jsonTree);
        return result;
    }

    /// <summary>
    /// Writes error messages that occur during JSchema validation process, to the
    /// standard error stream.
    /// </summary>
    public void WriteError()
    {
        if(Exceptions.Count == 0)
        {
            Console.WriteLine("No error has occurred");
            return;
        }
        foreach(var exception in Exceptions)
            Console.Error.WriteLine(exception.Message);
    }

    /// <summary>
    /// Indicates whether the input JSON string conforms to the given JSchema string.
    /// </summary>
    /// <param name="schema">The JSchema string to conform or validate.</param>
    /// <param name="json">The JSON string to conform or validate.</param>
    /// <returns><c>true</c> if the JSON string conforms to the JSchema; otherwise,
    /// <c>false</c>.</returns>
    public static bool IsValid(string schema, string json)
    {
        JsonSchema jsonSchema = new(schema);
        var result = jsonSchema.IsValid(json);
        if(!result) jsonSchema.WriteError();
        return result;
    }
}