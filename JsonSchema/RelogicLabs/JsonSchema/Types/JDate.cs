using RelogicLabs.JsonSchema.Time;

namespace RelogicLabs.JsonSchema.Types;

public class JDate : JDateTime
{
    internal JDate(JString baseNode, JsonDateTime dateTime) : base(baseNode, dateTime) { }
    public override JsonType Type => JsonType.DATE;
}