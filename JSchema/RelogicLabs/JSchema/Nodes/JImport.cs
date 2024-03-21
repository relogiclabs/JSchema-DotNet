using static RelogicLabs.JSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JSchema.Nodes;

public sealed class JImport : JDirective
{
    internal const string ImportMarker = "%import";
    public string ClassName { get; }

    private JImport(Builder builder) : base(builder)
        => ClassName = RequireNonNull(builder.ClassName);

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        if(obj.GetType() != this.GetType()) return false;
        JImport other = (JImport) obj;
        return ClassName == other.ClassName;
    }

    public override int GetHashCode() => ClassName.GetHashCode();
    public override string ToString() => $"{ImportMarker}: {ClassName}";

    internal new sealed class Builder : JNode.Builder
    {
        public string? ClassName { get; init; }
        public override JImport Build() => Build(new JImport(this));
    }
}