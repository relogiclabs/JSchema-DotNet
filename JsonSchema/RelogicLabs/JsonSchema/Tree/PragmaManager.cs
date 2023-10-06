using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Types;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Tree;

internal class PragmaManager
{
    public const string IGNORE_UNDEFINED_PROPERTIES = "IgnoreUndefinedProperties";
    public const string FLOATING_POINT_TOLERANCE = "FloatingPointTolerance";
    public const string IGNORE_OBJECT_PROPERTY_ORDER = "IgnoreObjectPropertyOrder";

    private readonly Dictionary<string, JPragma> _pragmas = new();

    public bool IgnoreUndefinedProperties { get; private set; }
        = PragmaDescriptor.IgnoreUndefinedProperties.DefaultValue;
    public double FloatingPointTolerance { get; private set; }
        = PragmaDescriptor.FloatingPointTolerance.DefaultValue;
    public bool IgnoreObjectPropertyOrder { get; private set; }
        = PragmaDescriptor.IgnoreObjectPropertyOrder.DefaultValue;

    public JPragma AddPragma(JPragma pragma) {
        if(_pragmas.ContainsKey(pragma.Name))
            throw new DuplicatePragmaException(MessageFormatter.FormatForSchema(
                PRAG03, $"Duplication found for {pragma.GetOutline()}", pragma.Context));
        _pragmas.Add(pragma.Name, pragma);
        SetPragmaValue(pragma.Name, pragma.Value);
        return pragma;
    }

    private void SetPragmaValue(string name, JPrimitive value) {
        switch(name) {
            case IGNORE_UNDEFINED_PROPERTIES:
                IgnoreUndefinedProperties = ((IPragmaValue<bool>) value).Value;
                break;
            case FLOATING_POINT_TOLERANCE:
                FloatingPointTolerance = ((IPragmaValue<double>) value).Value;
                break;
            case IGNORE_OBJECT_PROPERTY_ORDER:
                IgnoreObjectPropertyOrder = ((IPragmaValue<bool>) value).Value;
                break;
        }
    }

    public T GetPragmaValue<T>(string name)
    {
        var entry = PragmaDescriptor.From(name);
        _pragmas.TryGetValue(entry!.Name, out var pragma);
        return pragma == null
            ? ((PragmaProfile<T>) entry).DefaultValue
            : ((IPragmaValue<T>) pragma.Value).Value;
    }

    public JPragma? GetPragma(string name) {
        var entry = PragmaDescriptor.From(name);
        _pragmas.TryGetValue(entry!.Name, out var pragma);
        return pragma;
    }
}