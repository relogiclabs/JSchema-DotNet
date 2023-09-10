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
        if(node is not IJsonComposite composite) return FailWith(
            new JsonSchemaException(
                new ErrorDetail(DTYP01, InvalidNestedDataType),
                ExpectedDetail.AsInvalidDataType(this),
                ActualDetail.AsInvalidDataType(node)));
        IList<JNode> components = composite.ExtractComponents();
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
            new JsonSchemaException(new ErrorDetail(DTYP02, DataTypeMismatch),
            ExpectedDetail.AsDataTypeMismatch(this),
            ActualDetail.AsDataTypeMismatch(node)));
        if(node is not IJsonComposite composite) return FailWith(
            new JsonSchemaException(
                new ErrorDetail(DTYP01, InvalidNestedDataType),
                ExpectedDetail.AsInvalidDataType(this),
                ActualDetail.AsInvalidDataType(node)));
        IList<JNode> components = composite.ExtractComponents();
        bool result = true;
        foreach(var c in components)
        {
            if(!MatchCurrent(c)) result &= FailWith(new JsonSchemaException(
                new ErrorDetail(DTYP02, DataTypeMismatch),
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
    public bool IsApplicable(JNode node) => !Nested || node is IJsonComposite;
    public override int GetHashCode() => JsonType.GetHashCode();
    public override string ToJson()
    {
        var builder = new StringBuilder(JsonType.ToString());
        if(Nested) builder.Append(INestedMode.NestedMarker);
        if(Alias != null) builder.Append($"({Alias.Name})");
        return builder.ToString();
    }

    public override string ToString() => ToJson();
}