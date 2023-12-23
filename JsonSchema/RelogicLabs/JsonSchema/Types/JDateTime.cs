using RelogicLabs.JsonSchema.Time;

namespace RelogicLabs.JsonSchema.Types;

public abstract class JDateTime : JString
{
    public JsonDateTime DateTime { get; }

    private protected JDateTime(JString node, JsonDateTime dateTime) : base(node)
        => DateTime = dateTime;

    internal DateTimeParser GetDateTimeParser()
    {
        if(Type == JsonType.DATE) return Runtime.Pragmas.DateTypeParser;
        if(Type == JsonType.TIME) return Runtime.Pragmas.TimeTypeParser;
        throw new InvalidOperationException("Invalid date time type");
    }
    public override JsonType Type => JsonType.DATETIME;
}