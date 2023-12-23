using System.Collections;
using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Time;
using RelogicLabs.JsonSchema.Types;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Message.MessageFormatter;
using static RelogicLabs.JsonSchema.Time.DateTimeType;

namespace RelogicLabs.JsonSchema.Tree;

public sealed class PragmaRegistry : IEnumerable<KeyValuePair<string, JPragma>>
{
    private const string IGNORE_UNDEFINED_PROPERTIES = "IgnoreUndefinedProperties";
    private const string FLOATING_POINT_TOLERANCE = "FloatingPointTolerance";
    private const string IGNORE_OBJECT_PROPERTY_ORDER = "IgnoreObjectPropertyOrder";
    private const string DATE_DATA_TYPE_FORMAT = "DateDataTypeFormat";
    private const string TIME_DATA_TYPE_FORMAT = "TimeDataTypeFormat";

    private readonly Dictionary<string, JPragma> _pragmas = new();

    public bool IgnoreUndefinedProperties { get; private set; }
        = PragmaDescriptor.IgnoreUndefinedProperties.DefaultValue;
    public double FloatingPointTolerance { get; private set; }
        = PragmaDescriptor.FloatingPointTolerance.DefaultValue;
    public bool IgnoreObjectPropertyOrder { get; private set; }
        = PragmaDescriptor.IgnoreObjectPropertyOrder.DefaultValue;
    public string DateDataTypeFormat { get; private set; }
        = PragmaDescriptor.DateDataTypeFormat.DefaultValue;
    public string TimeDataTypeFormat { get; private set; }
        = PragmaDescriptor.TimeDataTypeFormat.DefaultValue;

    internal DateTimeParser DateTypeParser { get; private set; }
    internal DateTimeParser TimeTypeParser { get; private set; }

    internal PragmaRegistry()
    {
        DateTypeParser = new DateTimeParser(DateDataTypeFormat, DATE_TYPE);
        TimeTypeParser = new DateTimeParser(TimeDataTypeFormat, TIME_TYPE);
    }

    public JPragma AddPragma(JPragma pragma) {
        if(_pragmas.ContainsKey(pragma.Name))
            throw new DuplicatePragmaException(FormatForSchema(PRAG03,
                $"Duplication found for {pragma.GetOutline()}", pragma));
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
            case DATE_DATA_TYPE_FORMAT:
                DateDataTypeFormat = ((IPragmaValue<string>) value).Value;
                DateTypeParser = new DateTimeParser(DateDataTypeFormat, DATE_TYPE);
                break;
            case TIME_DATA_TYPE_FORMAT:
                TimeDataTypeFormat = ((IPragmaValue<string>) value).Value;
                TimeTypeParser = new DateTimeParser(TimeDataTypeFormat, TIME_TYPE);
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

    public IEnumerator<KeyValuePair<string, JPragma>> GetEnumerator()
        => _pragmas.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}