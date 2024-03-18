using RelogicLabs.JSchema.Time;
using RelogicLabs.JSchema.Types;

namespace RelogicLabs.JSchema.Nodes;

public sealed class JDate : JDateTime
{
    internal JDate(JString node, JsonDateTime dateTime) : base(node, dateTime) { }
    public override EType Type => EType.DATE;
}