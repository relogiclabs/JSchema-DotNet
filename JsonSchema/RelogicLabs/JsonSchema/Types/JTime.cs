using RelogicLabs.JsonSchema.Time;

namespace RelogicLabs.JsonSchema.Types;

public sealed class JTime : JDateTime
{
    internal JTime(JString baseNode, JsonDateTime dateTime) : base(baseNode, dateTime) { }
    public override JsonType Type => JsonType.TIME;
}