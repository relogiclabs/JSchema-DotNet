using Antlr4.Runtime.Tree;
using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Types;

public class JsonType
{
    private static readonly Dictionary<string, JsonType> _StringTypeMap = new();
    private static readonly Dictionary<Type, JsonType> _ClassTypeMap = new();

    public static readonly JsonType BOOLEAN = new("#boolean", typeof(JBoolean));
    public static readonly JsonType STRING = new("#string", typeof(JString));
    public static readonly JsonType INTEGER = new("#integer", typeof(JInteger));
    public static readonly JsonType FLOAT = new("#float", typeof(JFloat));
    public static readonly JsonType DOUBLE = new("#double", typeof(JDouble));
    public static readonly JsonType OBJECT = new("#object", typeof(JObject));
    public static readonly JsonType ARRAY = new("#array", typeof(JArray));
    public static readonly JsonType NULL = new("#null", typeof(JNull));
    public static readonly JsonType NUMBER = new("#number", typeof(JNumber));
    public static readonly JsonType DATETIME = new("#datetime", typeof(JDateTime));
    public static readonly JsonType DATE = new("#date", typeof(JString));
    public static readonly JsonType TIME = new("#time", typeof(JString));
    public static readonly JsonType PRIMITIVE = new("#primitive", typeof(JPrimitive));
    public static readonly JsonType COMPOSITE = new("#composite", typeof(JComposite));
    public static readonly JsonType ANY = new("#any", typeof(IJsonType));

    public string Name { get; }
    public Type Type { get; }


    internal static JsonType? From(Type type) => _ClassTypeMap.GetValue(type);
    internal static JsonType From(ITerminalNode node)
        => From(node.GetText(), Location.From(node.Symbol));

    internal static JsonType From(string name, Location location)
    {
        var result = _StringTypeMap.TryGetValue(name, out var type);
        if(!result) throw new InvalidDataTypeException(MessageFormatter.FormatForSchema(
                DTYP01, $"Invalid data type {name}", location));
        return type!;
    }

    private JsonType(string name, Type type)
    {
        Name = name;
        Type = type;
        _StringTypeMap[name] = this;
        _ClassTypeMap.TryAdd(type, this);
    }

    public override string ToString() => Name;
    public bool Match(JNode node, out string error)
    {
        error = string.Empty;
        if(!Type.IsInstanceOfType(node)) return false;

        if(this == DATE)
        {
            var _node = (JString) node;
            var dateTime = node.Runtime.Pragmas.DateTypeParser
                .TryParse(_node, out error);
            if(ReferenceEquals(dateTime, null)) return false;
            node.Derived = new JDate(_node, dateTime);
        }
        else if(this == TIME) {
            var _node = (JString) node;
            var dateTime = node.Runtime.Pragmas.TimeTypeParser
                .TryParse((JString) node, out error);
            if(ReferenceEquals(dateTime, null)) return false;
            node.Derived = new JTime(_node, dateTime);
        }
        return true;
    }
}