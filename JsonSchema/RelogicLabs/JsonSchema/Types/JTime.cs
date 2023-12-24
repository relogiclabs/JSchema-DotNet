using RelogicLabs.JsonSchema.Time;

namespace RelogicLabs.JsonSchema.Types;

public sealed class JTime : JDateTime
{
    internal JTime(JString node, JsonDateTime dateTime) : base(node, dateTime) { }
    public override JsonType Type => JsonType.TIME;
}