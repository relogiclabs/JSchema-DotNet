using RelogicLabs.JSchema.Nodes;

namespace RelogicLabs.JSchema.Tree;

internal abstract class PragmaDescriptor
{
    private static readonly Dictionary<string, PragmaDescriptor> _Pragmas = new();

    public const string ISO_8601_DATE = "YYYY-MM-DD";
    public const string ISO_8601_TIME = "YYYY-MM-DD'T'hh:mm:ss.FZZ";

    public static readonly PragmaProfile<bool> IgnoreUndefinedProperties
        = new(nameof(IgnoreUndefinedProperties), typeof(JBoolean), false);
    public static readonly PragmaProfile<double> FloatingPointTolerance
        = new(nameof(FloatingPointTolerance), typeof(JNumber), 1E-10);
    public static readonly PragmaProfile<bool> IgnoreObjectPropertyOrder
        = new(nameof(IgnoreObjectPropertyOrder), typeof(JBoolean), true);
    public static readonly PragmaProfile<string> DateDataTypeFormat
        = new(nameof(DateDataTypeFormat), typeof(JString), ISO_8601_DATE);
    public static readonly PragmaProfile<string> TimeDataTypeFormat
        = new(nameof(TimeDataTypeFormat), typeof(JString), ISO_8601_TIME);

    public string Name { get; }
    public Type Type { get; }

    public PragmaDescriptor(string name, Type type)
    {
        Name = name;
        Type = type;
        _Pragmas.Add(Name, this);
    }

    public static PragmaDescriptor? From(string name)
    {
        _Pragmas.TryGetValue(name, out var pragma);
        return pragma;
    }

    public bool MatchType(Type type) => Type.IsAssignableFrom(type);
}