using System.Text;
using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Message;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.ErrorDetail;
using static RelogicLabs.JSchema.Message.MessageFormatter;
using static RelogicLabs.JSchema.Nodes.INestedMode;
using static RelogicLabs.JSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JSchema.Nodes;

public sealed class JDataType : JBranch, INestedMode
{
    internal const string DataTypeName = "DataTypeName";
    public JsonType JsonType { get; }
    public bool Nested { get; }
    public JAlias? Alias { get; }

    private JDataType(Builder builder) : base(builder)
    {
        JsonType = RequireNonNull(builder.JsonType);
        Nested = RequireNonNull(builder.Nested);
        Alias = builder.Alias;
        Children = AsList(Alias);
    }

    public override bool Match(JNode node)
    {
        if(!JsonType.Match(node, out var error)) return FailTypeWith(
            new JsonSchemaException(new ErrorDetail(Nested ? DTYP06 : DTYP04,
                    FormatMessage(DataTypeMismatch, error)),
                ExpectedDetail.AsDataTypeMismatch(this),
                ActualDetail.AsDataTypeMismatch(node)));
        if(Alias is null) return true;
        var validator = Runtime.Definitions.TryGetValue(Alias);
        if(validator is null) return Fail(new DefinitionNotFoundException(
            FormatForSchema(Nested ? DEFI04 : DEFI03, $"No definition found for '{Alias}'", this)));
        if(!validator.Match(node)) return Fail(new JsonSchemaException(
            new ErrorDetail(Nested ? DTYP07 : DTYP05, DataTypeArgumentFailed),
            ExpectedDetail.AsDataTypeArgumentFailed(this),
            ActualDetail.AsDataTypeArgumentFailed(node)));
        return true;
    }

    private bool FailTypeWith(JsonSchemaException exception) {
        exception.SetAttribute(DataTypeName, ToString(true));
        return Fail(exception);
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

    internal bool IsMatchNull() => !Nested && JsonType.IsNullType();
    internal bool IsApplicable(JNode node) => !Nested || node is JComposite;
    public override int GetHashCode() => JsonType.GetHashCode();
    public override string ToString() => ToString(false);
    public string ToString(bool baseForm)
    {
        StringBuilder builder = new(JsonType.ToString());
        if(Nested && !baseForm) builder.Append(NestedMarker);
        if(Alias != null  && !baseForm) builder.Append($"({Alias})");
        return builder.ToString();
    }

    internal new sealed class Builder : JNode.Builder
    {
        public JsonType? JsonType { get; init; }
        public bool? Nested { get; init; }
        public JAlias? Alias { get; init; }
        public override JDataType Build() => Build(new JDataType(this));
    }
}