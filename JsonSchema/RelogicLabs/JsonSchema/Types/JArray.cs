using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Message.ErrorDetail;

namespace RelogicLabs.JsonSchema.Types;

public class JArray : JComposite
{
    public required IList<JNode> Elements { get; init; }
    public override IEnumerable<JNode> Children => Elements;

    internal JArray(IDictionary<JNode, JNode> relations) : base(relations) { }

    public override bool Match(JNode node)
    {
        var other = CastType<JArray>(node);
        if(other == null) return false;
        var result = true;
        for(var i = 0; i < Elements.Count; i++)
        {
            if(i >= other.Elements.Count && !((JValidator) Elements[i]).Optional)
                return FailWith(new JsonSchemaException(
                    new ErrorDetail(ARRY01, ArrayElementNotFound),
                    ExpectedDetail.AsArrayElementNotFound(Elements[i], i),
                    ActualDetail.AsArrayElementNotFound(other, i)));
            if(i >= other.Elements.Count) continue;
            result &= Elements[i].Match(other.Elements[i]);
        }
        return result;
    }

    public override JsonType Type => JsonType.ARRAY;
    public override IList<JNode> GetComponents() => Elements;
    public override string ToString() => Elements.ToString(", ", "[", "]");
}