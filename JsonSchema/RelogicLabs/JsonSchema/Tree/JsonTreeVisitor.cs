using RelogicLabs.JsonSchema.Antlr;
using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Types;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Tree;

internal class JsonTreeVisitor : JsonParserBaseVisitor<JNode>
{
    private readonly Dictionary<JNode, JNode> _relations = new();
    private readonly RuntimeContext _context;

    public JsonTreeVisitor(RuntimeContext context)
        => _context = context;

    public override JNode VisitJson(JsonParser.JsonContext context)
        => new JRoot(_relations)
        {
            Context = new Context(context, _context),
            Value = Visit(context.value()),
        }.Initialize();

    public override JNode VisitValue(JsonParser.ValueContext context)
    {
        if(context.primitive() != null) return Visit(context.primitive());
        if(context.array() != null) return Visit(context.array());
        if(context.@object() != null) return Visit(context.@object());
        throw new InvalidOperationException("Invalid Parser State");
    }

    public override JNode VisitObject(JsonParser.ObjectContext context)
        => new JObject(_relations)
        {
            Context = new Context(context, _context),
            Properties = CheckProperties(context.property()).ToProperties()
        }.Initialize();

    private IEnumerable<JProperty> CheckProperties(JsonParser.PropertyContext[] contexts)
    {
        List<JProperty> properties = contexts.Select(p => (JProperty) Visit(p)).ToList();
        var groups = properties.GroupBy(p => p.Key).Where(g => g.Count() > 1).ToList();
        if(groups.Count == 0) return properties;
        JProperty property = groups.First().First();
        throw new DuplicatePropertyKeyException(MessageFormatter.FormatForJson(
            PROP03, $"Multiple key with name {groups.First().Key.DoubleQuote()} found", 
            property.Context));

    }

    public override JNode VisitProperty(JsonParser.PropertyContext context)
        => new JProperty(_relations)
        {
            Context = new Context(context, _context),
            Key = context.STRING().GetText()[1..^1],
            Value = Visit(context.value())
        }.Initialize();

    public override JNode VisitArray(JsonParser.ArrayContext context)
        => new JArray(_relations)
        {
            Context = new Context(context, _context),
            Elements = context.value().Select(Visit).ToList().AsReadOnly()
        }.Initialize();

    public override JNode VisitPrimitiveTrue(JsonParser.PrimitiveTrueContext context)
        => new JBoolean(_relations)
        {
            Context = new Context(context, _context),
            Value = true
        }.Initialize();

    public override JNode VisitPrimitiveFalse(JsonParser.PrimitiveFalseContext context)
        => new JBoolean(_relations)
        {
            Context = new Context(context, _context),
            Value = false
        }.Initialize();

    public override JNode VisitPrimitiveString(JsonParser.PrimitiveStringContext context)
        => new JString(_relations)
        {
            Context = new Context(context, _context),
            Value = context.STRING().GetText().ToEncoded()
        }.Initialize();

    public override JNode VisitPrimitiveInteger(JsonParser.PrimitiveIntegerContext context)
        => new JInteger(_relations)
        {
            Context = new Context(context, _context),
            Value = Convert.ToInt64(context.INTEGER().GetText())
        }.Initialize();

    public override JNode VisitPrimitiveFloat(JsonParser.PrimitiveFloatContext context)
        => new JFloat(_relations)
        {
            Context = new Context(context, _context),
            Value = Convert.ToDouble(context.FLOAT().GetText())
        }.Initialize();

    public override JNode VisitPrimitiveDouble(JsonParser.PrimitiveDoubleContext context)
        => new JDouble(_relations)
        {
            Context = new Context(context, _context),
            Value = Convert.ToDouble(context.DOUBLE().GetText())
        }.Initialize();

    public override JNode VisitPrimitiveNull(JsonParser.PrimitiveNullContext context)
        => new JNull(_relations) { Context = new Context(context, _context) }
            .Initialize();
}