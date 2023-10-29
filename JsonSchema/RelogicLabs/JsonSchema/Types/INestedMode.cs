namespace RelogicLabs.JsonSchema.Types;

public interface INestedMode
{
    public const string NestedMarker = "*";
    public bool Nested { get; }
}