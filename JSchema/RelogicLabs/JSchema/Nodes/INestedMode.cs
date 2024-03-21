namespace RelogicLabs.JSchema.Nodes;

public interface INestedMode
{
    internal const string NestedMarker = "*";
    public bool Nested { get; }
}