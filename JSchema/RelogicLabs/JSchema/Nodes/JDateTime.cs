using RelogicLabs.JSchema.Time;
using RelogicLabs.JSchema.Types;

namespace RelogicLabs.JSchema.Nodes;

public abstract class JDateTime : JString
{
    public JsonDateTime DateTime { get; }

    private protected JDateTime(JString node, JsonDateTime dateTime) : base(node)
        => DateTime = dateTime;

    internal DateTimeParser GetDateTimeParser()
    {
        if(Type == EType.DATE) return Runtime.Pragmas.DateTypeParser;
        if(Type == EType.TIME) return Runtime.Pragmas.TimeTypeParser;
        throw new InvalidOperationException("Invalid date time type");
    }
}