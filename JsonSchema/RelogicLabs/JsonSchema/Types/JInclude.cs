using static RelogicLabs.JsonSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JsonSchema.Types;

public sealed class JInclude : JDirective
{
    internal const string IncludeMarker = "%include";
    public string ClassName { get; }

    private JInclude(Builder builder) : base(builder)
        => ClassName = RequireNonNull(builder.ClassName);

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        if(obj.GetType() != this.GetType()) return false;
        JInclude other = (JInclude) obj;
        return ClassName == other.ClassName;
    }

    public override int GetHashCode() => ClassName.GetHashCode();
    public override string ToString() => $"{IncludeMarker}: {ClassName}";

    internal new class Builder : JNode.Builder
    {
        public string? ClassName { get; init; }
        public override JInclude Build() => Build(new JInclude(this));
    }
}