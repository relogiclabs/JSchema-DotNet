using RelogicLabs.JsonSchema.Time;

namespace RelogicLabs.JsonSchema.Types;

public sealed class JDate : JDateTime
{
    internal JDate(JString node, JsonDateTime dateTime) : base(node, dateTime) { }
    public override JsonType Type => JsonType.DATE;
}