using RelogicLabs.JsonSchema.Types;

namespace RelogicLabs.JsonSchema.Tree;

internal abstract class PragmaDescriptor
{
    private static readonly Dictionary<string, PragmaDescriptor> _Pragmas = new();
    
    public static readonly PragmaProfile<bool> IgnoreUndefinedProperties 
        = new(nameof(IgnoreUndefinedProperties), typeof(JBoolean), false);
    public static readonly PragmaProfile<double> FloatingPointTolerance 
        = new(nameof(FloatingPointTolerance), typeof(IJsonFloat), 1E-7);
    public static readonly PragmaProfile<bool> IgnoreObjectPropertyOrder 
        = new(nameof(IgnoreObjectPropertyOrder), typeof(JBoolean), true);
    
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