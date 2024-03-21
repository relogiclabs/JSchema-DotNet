using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Types;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.MessageFormatter;
using static RelogicLabs.JSchema.Types.EType;

namespace RelogicLabs.JSchema.Nodes;

public sealed class JsonType
{
    private static readonly Dictionary<string, EType> _StringTypeMap = new(16);
    private static readonly Dictionary<EType, Type> _TypeClassMap = new(16);
    private static readonly Dictionary<Type, EType> _ClassTypeMap = new(16);

    public EType Type { get; }

    static JsonType()
    {
        MapType(BOOLEAN, typeof(JBoolean));
        MapType(INTEGER, typeof(JInteger));
        MapType(FLOAT, typeof(JFloat));
        MapType(DOUBLE, typeof(JDouble));
        MapType(NUMBER, typeof(JNumber));
        MapType(STRING, typeof(JString));
        MapType(ARRAY, typeof(JArray));
        MapType(OBJECT, typeof(JObject));
        MapType(NULL, typeof(JNull));
        MapType(DATE, typeof(JString));
        MapType(TIME, typeof(JString));
        MapType(PRIMITIVE, typeof(JPrimitive));
        MapType(COMPOSITE, typeof(JComposite));
        MapType(ANY, typeof(IJsonType));
    }

    private JsonType(EType type) => Type = type;
    internal bool IsNullType() => Type == NULL;
    internal static EType? GetType(Type typeClass) => _ClassTypeMap.TryGetValue(typeClass);

    private static void MapType(EType type, Type typeClass)
    {
        _StringTypeMap[type.Name] = type;
        _ClassTypeMap.TryAdd(typeClass, type);
        _TypeClassMap[type] = typeClass;
    }

    internal static JsonType From(ITerminalNode node)
        => From(node.GetText(), node.Symbol);

    internal static JsonType From(string name, IToken token)
    {
        _StringTypeMap.TryGetValue(name, out var type);
        if(type is null) throw new InvalidDataTypeException(FormatForSchema(DTYP01,
            $"Invalid data type '{name}'", token));
        return new JsonType(type);
    }

    public bool Match(JNode node, out string error)
    {
        error = string.Empty;
        if(!_TypeClassMap[Type].IsInstanceOfType(node)) return false;
        if(Type == DATE)
        {
            var date = (JString) node;
            var dateTime = node.Runtime.Pragmas.DateTypeParser
                .TryParse(date, out error);
            if(dateTime is null) return false;
            date.Derived = new JDate(date, dateTime);
        }
        else if(Type == TIME)
        {
            var time = (JString) node;
            var dateTime = node.Runtime.Pragmas.TimeTypeParser
                .TryParse(time, out error);
            if(dateTime is null) return false;
            time.Derived = new JTime(time, dateTime);
        }
        return true;
    }

    public override string ToString() => Type.Name;
}