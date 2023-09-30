using Antlr4.Runtime.Tree;
using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Time;
using RelogicLabs.JsonSchema.Tree;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Types;

public class JsonType
{
    private static readonly Dictionary<string, JsonType> _DataTypes = new();
    private static readonly DateTimeValidator _Iso8601Date = new(DateTimeValidator.ISO_8601_DATE);
    private static readonly DateTimeValidator _Iso8601Time = new(DateTimeValidator.ISO_8601_TIME);
    
    public static readonly JsonType BOOLEAN = new("#boolean", typeof(JBoolean));
    public static readonly JsonType STRING = new("#string", typeof(JString));
    public static readonly JsonType INTEGER = new("#integer", typeof(JInteger));
    public static readonly JsonType FLOAT = new("#float", typeof(JFloat));
    public static readonly JsonType DOUBLE = new("#double", typeof(JDouble));
    public static readonly JsonType OBJECT = new("#object", typeof(JObject));
    public static readonly JsonType ARRAY = new("#array", typeof(JArray));
    public static readonly JsonType NULL = new("#null", typeof(JNull));
    public static readonly JsonType NUMBER = new("#number", typeof(JNumber));
    public static readonly JsonType DATE = new("#date", typeof(JString));
    public static readonly JsonType TIME = new("#time", typeof(JString));
    public static readonly JsonType ANY = new("#any", typeof(IJsonType));
    
    public string Name { get; }
    public Type Type { get; }


    internal static JsonType From(ITerminalNode node) 
        => From(node.GetText(), Location.From(node.Symbol));
    
    internal static JsonType From(string name, Location location)
    {
        var result = _DataTypes.TryGetValue(name, out var type);
        if(!result) throw new InvalidDataTypeException(MessageFormatter.FormatForSchema(
                DTYP01, $"Invalid data type {name}", location));
        return type!;
    }

    private JsonType(string name, Type type)
    {
        Name = name;
        Type = type;
        _DataTypes[name] = this;
    }

    public override string ToString() => Name;
    public bool Match(JNode node)
    {
        if(!Type.IsInstanceOfType(node)) return false;
        if(this == DATE) return _Iso8601Date.IsValidDate((JString) node);
        if(this == TIME) return _Iso8601Time.IsValidTime((JString) node);
        return true;
    }
}