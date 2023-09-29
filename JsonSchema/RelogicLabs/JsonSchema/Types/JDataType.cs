using System.Text;
using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Message.ErrorDetail;

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
        if(!Nested) return JsonType.Match(node);
        if(node is not JComposite composite) return FailWith(
            new JsonSchemaException(
                new ErrorDetail(DTYP02, InvalidNestedDataType),
                ExpectedDetail.AsInvalidDataType(this),
                ActualDetail.AsInvalidDataType(node)));
        IList<JNode> components = composite.GetComponents();
        return components.Select(MatchCurrent).AllTrue();
    }

    private bool MatchCurrent(JNode node)
    {
        bool result = true;
        result &= JsonType.Match(node);
        if(Alias == null) return result;
        Runtime.Definitions.TryGetValue(Alias, out var validator);
        if(validator == null) return FailWith(new DefinitionNotFoundException(
            MessageFormatter.FormatForSchema(DEFI03, Alias.Name, Context)));
        result &= validator.Match(node);
        return result;
    }

    internal bool MatchForReport(JNode node)
    {
        if(!Nested && !JsonType.Match(node)) return FailWith(
            new JsonSchemaException(new ErrorDetail(DTYP04, DataTypeMismatch),
            ExpectedDetail.AsDataTypeMismatch(this),
            ActualDetail.AsDataTypeMismatch(node)));
        if(node is not JComposite composite) return FailWith(
            new JsonSchemaException(
                new ErrorDetail(DTYP05, InvalidNestedDataType),
                ExpectedDetail.AsInvalidDataType(this),
                ActualDetail.AsInvalidDataType(node)));
        IList<JNode> components = composite.GetComponents();
        bool result = true;
        foreach(var c in components)
        {
            if(!MatchCurrent(c)) result &= FailWith(new JsonSchemaException(
                new ErrorDetail(DTYP06, DataTypeMismatch),
                ExpectedDetail.AsDataTypeMismatch(this),
                ActualDetail.AsDataTypeMismatch(c)));
        }
        return result;
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