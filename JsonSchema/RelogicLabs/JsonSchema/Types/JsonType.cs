using RelogicLabs.JsonSchema.Date;

namespace RelogicLabs.JsonSchema.Types;

public class JsonType
{
    private static readonly Dictionary<string, JsonType> _DataTypes = new();
    private static readonly DateValidator _DateValidator = new(DateValidator.ISO_8601);
    
    public static readonly JsonType Boolean = new("#boolean", typeof(JBoolean));
    public static readonly JsonType String = new("#string", typeof(JString));
    public static readonly JsonType Integer = new("#integer", typeof(JInteger));
    public static readonly JsonType Float = new("#float", typeof(JFloat));
    public static readonly JsonType Decimal = new("#double", typeof(JDouble));
    public static readonly JsonType Object = new("#object", typeof(JObject));
    public static readonly JsonType Array = new("#array", typeof(JArray));
    public static readonly JsonType Null = new("#null", typeof(JNull));
    public static readonly JsonType Number = new("#number", typeof(JNumber));
    public static readonly JsonType Date = new("#date", typeof(JString));
    public static readonly JsonType Any = new("#any", typeof(IJsonType<>));
    
    public string Name { get; }
    public Type Type { get; }

    public static JsonType From(string name) => _DataTypes[name];
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
        return this != Date || _DateValidator.IsDate((JString) node);
    }
}