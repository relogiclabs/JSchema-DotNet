namespace RelogicLabs.JSchema.Tree;

public sealed class TreeType
{
    public static readonly TreeType SCHEMA_TREE = new("Schema");
    public static readonly TreeType JSON_TREE = new("Json");

    public string Name { get; }

    private TreeType(string name) => Name = name;
    public override string ToString() => Name;
}