using RelogicLabs.JsonSchema.Types;

namespace RelogicLabs.JsonSchema.Tree;

internal abstract class PragmaPreset
{
    private static readonly Dictionary<string, PragmaPreset> _Pragmas = new();
    
    public static readonly PragmaDescriptor<bool> IgnoreUnknownProperties 
        = new(nameof(IgnoreUnknownProperties), typeof(JBoolean), false);
    public static readonly PragmaDescriptor<double> FloatingPointTolerance 
        = new(nameof(FloatingPointTolerance), typeof(IJsonFloat), 1E-7);
    public static readonly PragmaDescriptor<bool> IgnoreObjectPropertyOrder 
        = new(nameof(IgnoreObjectPropertyOrder), typeof(JBoolean), true);
    
    public string Name { get; }
    public Type Type { get; }

    public PragmaPreset(string name, Type type)
    {
        Name = name;
        Type = type;
        _Pragmas.Add(Name, this);
    }

    public static PragmaPreset? From(string name)
    {
        _Pragmas.TryGetValue(name, out var pragma);
        return pragma;
    }

    public bool MatchType(Type type) => Type.IsAssignableFrom(type);
}