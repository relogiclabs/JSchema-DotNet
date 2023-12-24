using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Message.ErrorDetail;
using static RelogicLabs.JsonSchema.Message.MessageFormatter;
using static RelogicLabs.JsonSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JsonSchema.Types;

public sealed class JArray : JComposite
{
    public IList<JNode> Elements { get; }

    private JArray(Builder builder) : base(builder)
    {
        Elements = RequireNonNull(builder.Elements);
        Children = Elements;
    }

    public override bool Match(JNode node)
    {
        var other = CastType<JArray>(node);
        if(other == null) return false;
        bool result = true, restOptional = false;
        for(var i = 0; i < Elements.Count; i++)
        {
            var optional = IsOptional(Elements[i]);
            if((restOptional |= optional) != optional) return FailWith(
                new MisplacedOptionalException(FormatForSchema(ARRY02,
                    "Mandatory array element cannot appear after optional element",
                    Elements[i])));
            if(i >= other.Elements.Count && !optional)
                return FailWith(new JsonSchemaException(
                    new ErrorDetail(ARRY01, ArrayElementNotFound),
                    ExpectedDetail.AsArrayElementNotFound(Elements[i], i),
                    ActualDetail.AsArrayElementNotFound(other, i)));
            if(i >= other.Elements.Count) continue;
            result &= Elements[i].Match(other.Elements[i]);
        }
        return result;
    }

    private static bool IsOptional(JNode node) => node is JValidator { Optional: true };
    public override JsonType Type => JsonType.ARRAY;
    public override IList<JNode> Components => Elements;
    public override string ToString() => Elements.JoinWith(", ", "[", "]");

    internal new class Builder : JNode.Builder
    {
        public IList<JNode>? Elements { get; init; }
        public override JArray Build() => Build(new JArray(this));
    }
}