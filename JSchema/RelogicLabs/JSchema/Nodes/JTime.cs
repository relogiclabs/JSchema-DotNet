using RelogicLabs.JSchema.Time;
using RelogicLabs.JSchema.Types;

namespace RelogicLabs.JSchema.Nodes;

public sealed class JTime : JDateTime
{
    internal JTime(JString node, JsonDateTime dateTime) : base(node, dateTime) { }
    public override EType Type => EType.TIME;
}