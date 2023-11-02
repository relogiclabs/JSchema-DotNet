using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Message.ErrorDetail;
using static RelogicLabs.JsonSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JsonSchema.Types;

public sealed class JValidator : JBranch
{
    public const string OptionalMarker = "?";
    public JNode? Value { get; }
    public IList<JFunction> Functions { get; }
    public IList<JDataType> DataTypes { get; }
    public bool Optional { get; }

    private JValidator(Builder builder) : base(builder)
    {
        Value = builder.Value;
        Functions = NonNull(builder.Functions);
        DataTypes = NonNull(builder.DataTypes);
        Optional = NonNull(builder.Optional);
        Children = new List<JNode>().AddToList(Value)
            .AddToList(Functions, DataTypes).AsReadOnly();
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
        var fDataType = rDataType && DataTypes.Count != 0;
        bool rFunction = Functions.Where(f => f.IsApplicable(node) || !fDataType)
            .Select(f => f.Match(node)).ForEachTrue() || Functions.Count == 0;
        return rValue & rDataType & rFunction;
    }

    private bool MatchDataType(JNode node)
    {
        if(Runtime.TryMatch(() => CheckDataType(node))) return true;
        DataTypes.Where(d => !d.Nested).ForEach(d => d.MatchForReport(node));
        DataTypes.Where(d => d.Nested).ForEach(d => d.MatchForReport(node));
        return false;
    }

    private bool CheckDataType(JNode node)
    {
        var list1 = DataTypes.Where(d => !d.Nested).Select(d => d.Match(node)).ToList();
        var result1 = list1.AnyTrue();
        var list2 = DataTypes.Where(d => d.Nested && (d.IsApplicable(node) || !result1))
            .Select(d => d.Match(node)).ToList();
        var result2 = list2.AnyTrue() || list2.Count == 0;
        return (result1 || list1.Count == 0) && result2;
    }

    public override string ToString() => (
        $"{Value?.ToString() ?? string.Empty}"
        + $"{Functions.Join(" ", " ")}"
        + $"{DataTypes.Join(" ", " ")}"
        + $"{(Optional? $" {OptionalMarker}" : string.Empty)}"
    ).Trim();

    internal new class Builder : JNode.Builder
    {
        public JNode? Value { get; init; }
        public IList<JFunction>? Functions { get; init; }
        public IList<JDataType>? DataTypes { get; init; }
        public bool? Optional { get; init; }
        public override JValidator Build() => Build(new JValidator(this));
    }
}