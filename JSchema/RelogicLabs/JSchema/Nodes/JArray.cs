using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Message;
using RelogicLabs.JSchema.Types;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.ErrorDetail;
using static RelogicLabs.JSchema.Message.MessageFormatter;
using static RelogicLabs.JSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JSchema.Nodes;

public sealed class JArray : JComposite, IEArray
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
            if((restOptional |= optional) != optional) return Fail(
                new MisplacedOptionalException(FormatForSchema(ARRY02,
                    "Mandatory array element cannot appear after optional element",
                    Elements[i])));
            if(i >= other.Elements.Count && !optional)
                return Fail(new JsonSchemaException(
                    new ErrorDetail(ARRY01, ArrayElementNotFound),
                    ExpectedDetail.AsArrayElementNotFound(Elements[i], i),
                    ActualDetail.AsArrayElementNotFound(other, i)));
            if(i >= other.Elements.Count) continue;
            result &= Elements[i].Match(other.Elements[i]);
        }
        return result;
    }

    public override IList<JNode> Components => Elements;
    public IReadOnlyList<IEValue> Values => (IReadOnlyList<IEValue>) Elements;
    public IEValue Get(int index) => Elements[index];
    public void Set(int index, IEValue value)
        => throw new UpdateNotSupportedException(AUPD01, "Readonly array cannot be updated");
    private static bool IsOptional(JNode node) => node is JValidator { Optional: true };
    public override string ToString() => Elements.JoinWith(", ", "[", "]");

    internal new sealed class Builder : JNode.Builder
    {
        public IList<JNode>? Elements { get; init; }
        public override JArray Build() => Build(new JArray(this));
    }
}