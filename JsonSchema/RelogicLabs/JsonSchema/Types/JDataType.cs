using System.Text;
using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Message.ErrorDetail;
using static RelogicLabs.JsonSchema.Message.MatchReport;
using static RelogicLabs.JsonSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JsonSchema.Types;

public sealed class JDataType : JBranch, INestedMode
{
    public JsonType JsonType { get; }
    public bool Nested { get; }
    public JAlias? Alias { get; }

    private JDataType(Builder builder) : base(builder)
    {
        JsonType = NonNull(builder.JsonType);
        Nested = NonNull(builder.Nested);
        Alias = builder.Alias;
        Children = AsList(Alias);
    }

    public override bool Match(JNode node)
    {
        if(!Nested) return IsMatchCurrent(node);
        if(node is not JComposite composite) return false;
        IList<JNode> components = composite.GetComponents();
        return components.Select(IsMatchCurrent).AllTrue();
    }

    private bool IsMatchCurrent(JNode node)
        => MatchCurrent(node, out _) == Success;

    private MatchReport MatchCurrent(JNode node, out string error)
    {
        var result = JsonType.Match(node, out error) ? Success : TypeError;
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
        var result = MatchCurrent(node, out var error);
        if(ReferenceEquals(result, TypeError)) return FailWith(
            new JsonSchemaException(
                new ErrorDetail(TypeError.GetCode(nested),
                    FormatMessage(DataTypeMismatch, error)),
                ExpectedDetail.AsDataTypeMismatch(this),
                ActualDetail.AsDataTypeMismatch(node)));
        if(ReferenceEquals(result, AliasError)) return FailWith(
            new DefinitionNotFoundException(
                MessageFormatter.FormatForSchema(AliasError.GetCode(nested),
                    $"No definition found for '{Alias}'", Context)));
        if(ReferenceEquals(result, ArgumentError)) return FailWith(
            new JsonSchemaException(
                new ErrorDetail(ArgumentError.GetCode(nested),
                    DataTypeArgumentFailed),
                ExpectedDetail.AsDataTypeArgumentFailed(this),
                ActualDetail.AsDataTypeArgumentFailed(node)));
        return true;
    }

    private static string FormatMessage(string main, string optional)
        => string.IsNullOrEmpty(optional) ? main : $"{main} ({optional.Uncapitalize()})";

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

    internal new class Builder : JNode.Builder
    {
        public JsonType? JsonType { get; init; }
        public bool? Nested { get; init; }
        public JAlias? Alias { get; init; }
        public override JDataType Build() => Build(new JDataType(this));
    }
}