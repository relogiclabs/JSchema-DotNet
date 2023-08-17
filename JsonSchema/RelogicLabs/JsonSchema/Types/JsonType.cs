namespace RelogicLabs.JsonSchema.Types;

public class JsonType
{
    private static readonly Dictionary<string, JsonType> DataTypes = new();
    
    public static readonly JsonType Boolean = new("#boolean", typeof(JBoolean));
    public static readonly JsonType String = new("#string", typeof(JString));
    public static readonly JsonType Integer = new("#integer", typeof(JInteger));
    public static readonly JsonType Float = new("#float", typeof(JFloat));
    public static readonly JsonType Decimal = new("#double", typeof(JDouble));
    public static readonly JsonType Object = new("#object", typeof(JObject));
    public static readonly JsonType Array = new("#array", typeof(JArray));
    public static readonly JsonType Null = new("#null", typeof(JNull));
    public static readonly JsonType Number = new("#number", typeof(JNumber));
    public static readonly JsonType Any = new("#any", typeof(IJsonType<>));
    
    public string Name { get; }
    public Type Type { get; }

    public static JsonType From(string name) => DataTypes[name];
    private JsonType(string name, Type type)
    {
        Name = name;
        Type = type;
        DataTypes[name] = this;
    }

    public override string ToString() => Name;
    public bool Match(JNode node) => Type.IsInstanceOfType(node);
}