using System.Text;
using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Message.ErrorDetail;
using static RelogicLabs.JsonSchema.Message.MatchReport;

namespace RelogicLabs.JsonSchema.Types;

public class JDataType : JBranch, INestedMode
{
    private IList<JNode> _children = null!;
    public required JsonType JsonType { get; init; }
    public required JAlias? Alias { get; init; }
    public required bool Nested { get; init; }
    public override IEnumerable<JNode> Children => _children;

    internal JDataType(IDictionary<JNode, JNode> relations) : base(relations) { }

    internal override JNode Initialize()
    {
        List<JNode> children = new();
        if(Alias != null) children.Add(Alias);
        _children = children.AsReadOnly();
        return base.Initialize();
    }

    public override bool Match(JNode node)
    {
        if(!Nested) return _MatchCurrent(node);
        if(node is not JComposite composite) return false;
        IList<JNode> components = composite.GetComponents();
        return components.Select(_MatchCurrent).AllTrue();
    }

    private bool _MatchCurrent(JNode node)
        => MatchCurrent(node) == Success;

    private MatchReport MatchCurrent(JNode node)
    {
        var result = JsonType.Match(node) ? Success : TypeError;
        if(Alias == null || result != Success) return result;
        Runtime.Definitions.TryGetValue(Alias, out var validator);
        if(validator == null) return AliasError;
        result = validator.Match(node) ? Success : ArgumentError;
        return result;
    }

    internal bool MatchForReport(JNode node)
    {
        if(!Nested) return MatchForReport(node, false);
        if(node is not JComposite composite) return FailWith(
            new JsonSchemaException(
                new ErrorDetail(DTYP03, InvalidNestedDataType),
                ExpectedDetail.AsInvalidNestedDataType(this),
                ActualDetail.AsInvalidNestedDataType(node)));
        IList<JNode> components = composite.GetComponents();
        bool result = true;
        foreach(var c in components) result &= MatchForReport(c, true);
        return result;
    }

    private bool MatchForReport(JNode node, bool nested)
    {
        var result = MatchCurrent(node);
        if(ReferenceEquals(result, TypeError)) return FailWith(
            new JsonSchemaException(
                new ErrorDetail(TypeError.GetCode(nested), DataTypeMismatch),
                ExpectedDetail.AsDataTypeMismatch(this),
                ActualDetail.AsDataTypeMismatch(node)));
        if(ReferenceEquals(result, AliasError)) return FailWith(
            new DefinitionNotFoundException(
                MessageFormatter.FormatForSchema(AliasError.GetCode(nested),
                    $"No definition found for {Alias!.Name}", Context)));
        if(ReferenceEquals(result, ArgumentError)) return FailWith(
            new JsonSchemaException(
                new ErrorDetail(ArgumentError.GetCode(nested),
                    DataTypeArgumentFailed),
                ExpectedDetail.AsDataTypeArgumentFailed(this),
                ActualDetail.AsDataTypeArgumentFailed(node)));
        return true;
    }

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        if(obj.GetType() != this.GetType()) return false;
        JDataType other = (JDataType) obj;
        return JsonType == other.JsonType;
    }

    internal bool IsMatchNull() => !Nested && JsonType == JsonType.NULL;
    public bool IsApplicable(JNode node) => !Nested || node is JComposite;
    public override int GetHashCode() => JsonType.GetHashCode();
    public override string ToString() => ToString(false);
    public string ToString(bool baseForm)
    {
        StringBuilder builder = new(JsonType.ToString());
        if(Nested && !baseForm) builder.Append(INestedMode.NestedMarker);
        if(Alias != null  && !baseForm) builder.Append($"({Alias})");
        return builder.ToString();
    }
}