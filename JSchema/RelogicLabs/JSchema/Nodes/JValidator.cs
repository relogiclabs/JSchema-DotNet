using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Message;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.ErrorDetail;
using static RelogicLabs.JSchema.Nodes.JDataType;
using static RelogicLabs.JSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JSchema.Nodes;

public sealed class JValidator : JBranch
{
    internal const string OptionalMarker = "?";
    private readonly List<Exception> _exceptions;
    private List<Exception> TryBuffer => Runtime.Exceptions.TryBuffer;

    public JNode? Value { get; }
    public IList<JFunction> Functions { get; }
    public IList<JDataType> DataTypes { get; }
    public IList<JReceiver> Receivers { get; }
    public bool Optional { get; }

    private JValidator(Builder builder) : base(builder)
    {
        _exceptions = new List<Exception>();
        Value = builder.Value;
        Functions = RequireNonNull(builder.Functions);
        DataTypes = RequireNonNull(builder.DataTypes);
        Receivers = RequireNonNull(builder.Receivers);
        Optional = RequireNonNull(builder.Optional);
        Runtime.Receivers.Register(Receivers);
        Children = new List<JNode>().AddToList(Value)
            .AddToList(Functions, DataTypes, Receivers)
            .AsReadOnly();
    }

    public override bool Match(JNode node)
    {
        bool rValue = true;
        var value = CastType<IJsonType>(node);
        if(value == null) return false;
        Runtime.Receivers.Receive(Receivers, node);
        if(node is JNull && DataTypes.Any(static d => d.IsMatchNull()))
            return true;
        if(Value != null) rValue &= Value.Match(value.Node);
        if(!rValue) return Fail(new JsonSchemaException(
            new ErrorDetail(VALD01, ValidationFailed),
            ExpectedDetail.AsGeneralValueMismatch(Value!),
            ActualDetail.AsGeneralValueMismatch(node)));
        var rDataType = MatchDataType(node);
        var fDataType = rDataType && DataTypes.Count != 0;
        bool rFunction = Functions.Where(f => f.IsApplicable(node) || !fDataType)
            .ForEachTrue(f => f.Match(node));
        return rValue & rDataType & rFunction;
    }

    private bool MatchDataType(JNode node)
    {
        if(Runtime.Exceptions.TryExecute(() => CheckDataType(node))) return true;
        SaveTryBuffer();
        foreach(var e in _exceptions) Fail(e);
        return false;
    }

    private static IEnumerable<Exception> ProcessTryBuffer(List<Exception> buffer)
    {
        var list = new List<Exception>(buffer.Count);
        foreach(var e in buffer)
        {
            var result = MergeException(list.TryGetLast(), e);
            if(result is not null) list[^1] = result;
            else list.Add(e);
        }
        return list;
    }

    private static JsonSchemaException? MergeException(Exception? ex1, Exception? ex2)
    {
        if(ex1 is not JsonSchemaException e1 || ex2 is not JsonSchemaException e2) return null;
        if(e1.Code != e2.Code) return null;
        var a1 = e1.GetAttribute(DataTypeName);
        var a2 = e2.GetAttribute(DataTypeName);
        if(a1 is null || a2 is null) return null;
        var result = new JsonSchemaException(e1.Error, MergeExpected(e1, e2), e2.Actual,
            Aggregate(e1.InnerException ?? e1, e2));
        result.SetAttribute(DataTypeName, a1 + a2);
        return result;
    }

    private static AggregateException Aggregate(Exception ex1, Exception ex2)
    {
        var list = new List<Exception>();
        if(ex1 is AggregateException age1) list.AddRange(age1.InnerExceptions);
        else list.Add(ex1);
        if(ex2 is AggregateException age2) list.AddRange(age2.InnerExceptions);
        else list.Add(ex2);
        return new AggregateException(list.ToArray());
    }

    private static ExpectedDetail MergeExpected(JsonSchemaException ex1,
                                                JsonSchemaException ex2)
    {
        var typeName2 = ex2.GetAttribute(DataTypeName);
        var expected1 = ex1.Expected;
        return new ExpectedDetail(expected1.Context, $"{expected1.Message} or {typeName2}");
    }

    private bool CheckDataType(JNode node)
    {
        var list1 = DataTypes.Where(static d => !d.Nested).ToList();
        var result1 = AnyMatch(list1, node);
        if(result1) TryBuffer.Clear();
        var list2 = DataTypes.Where(d => d.Nested && (d.IsApplicable(node) || !result1)).ToList();
        if(list2.IsEmpty()) return result1 || list1.IsEmpty();
        if(node is not JComposite composite) return Fail(
            new JsonSchemaException(
                new ErrorDetail(DTYP03, InvalidNonCompositeType),
                ExpectedDetail.AsInvalidNonCompositeType(list2.First()),
                ActualDetail.AsInvalidNonCompositeType(node)));
        SaveTryBuffer();
        var result2 = composite.Components.ForEachTrue(n => AnyMatch(list2, n));
        return (result1 || list1.IsEmpty()) && result2;
    }

    private bool AnyMatch(List<JDataType> list, JNode node)
    {
        TryBuffer.Clear();
        foreach(var d in list) if(d.Match(node)) return true;
        SaveTryBuffer();
        return false;
    }

    private void SaveTryBuffer()
    {
        if(TryBuffer.IsEmpty()) return;
        _exceptions.AddRange(ProcessTryBuffer(TryBuffer));
        TryBuffer.Clear();
    }

    public override string ToString() => (
        $"{Value?.ToString() ?? string.Empty}"
        + $"{Functions.Join(" ", " ")}"
        + $"{DataTypes.Join(" ", " ")}"
        + $"{(Optional? $" {OptionalMarker}" : string.Empty)}"
    ).Trim();

    internal new sealed class Builder : JNode.Builder
    {
        public JNode? Value { get; init; }
        public IList<JFunction>? Functions { get; init; }
        public IList<JDataType>? DataTypes { get; init; }
        public IList<JReceiver>? Receivers { get; init; }
        public bool? Optional { get; init; }
        public override JValidator Build() => Build(new JValidator(this));
    }
}