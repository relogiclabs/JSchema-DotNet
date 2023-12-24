namespace RelogicLabs.JsonSchema.Types;

public interface INestedMode
{
    internal const string NestedMarker = "*";
    public bool Nested { get; }
}