using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Message.ErrorDetail;

namespace RelogicLabs.JsonSchema.Types;

public class JValidator : JBranch
{
    public const string OptionalMarker = "?";
    private IEnumerable<JNode> _children = Enumerable.Empty<JNode>()!;
    
    public JNode? Value { get; init; }
    public required IList<JFunction> Functions { get; init; }
    public required IList<JDataType> DataTypes { get; init; }
    public bool Optional { get; init; }
    public override IEnumerable<JNode> Children => _children;
    
    internal JValidator(IDictionary<JNode, JNode> relations) : base(relations) { }

    internal override JValidator Initialize()
    {
        if(Value != null) _children = _children.Concat(new[] { Value });
        _children = _children.Concat(Functions).Concat(DataTypes);
        foreach(var c in Children) _relations[c] = this;
        return this;
    }

    public override bool Match(JNode node)
    {
        bool rValue = true;
        var value = CastType<IJsonType>(node);
        if(value == null) return false;
        if(node is JNull && DataTypes.Select(d => d.IsMatchNull()).AnyTrue())
            return true;
        if(Value != null) rValue &= Value.Match(value.Node);
        if(!rValue) return FailWith(new JsonSchemaException(
            new ErrorDetail(VALD01, ValidationFailed),
            ExpectedDetail.AsValueMismatch(Value!),
            ActualDetail.AsValueMismatch(node)));
        var rDataType = MatchDataType(node);
        if(!rDataType) DataTypes.ForEach(d => d.MatchForReport(node));
        var fDataType = rDataType && DataTypes.Count != 0;
        bool rFunction = Functions.Where(f => f.IsApplicable(node) || !fDataType)
            .Select(f => f.Match(node)).ForEachTrue() || Functions.Count == 0;
        return rValue & rDataType & rFunction;
    }

    private bool MatchDataType(JNode node)
    {
        var list1 = DataTypes.Where(d => !d.Nested).Select(d => d.Match(node)).ToList();
        var result1 = list1.AnyTrue() || list1.Count == 0;
        var list2 = DataTypes.Where(d => d.Nested && (d.IsApplicable(node) || !result1))
            .Select(d => d.Match(node)).ToList();
        var result2 = list2.AnyTrue() || list2.Count == 0;
        return result1 && result2;
    }

    public override string ToString() => (
        $"{Value?.ToString() ?? string.Empty}"
        + $"{Functions.ToString(" ", " ")}"
        + $"{DataTypes.ToString(" ", " ")}"
        + $"{(Optional? $" {OptionalMarker}" : string.Empty)}"
    ).Trim();
}