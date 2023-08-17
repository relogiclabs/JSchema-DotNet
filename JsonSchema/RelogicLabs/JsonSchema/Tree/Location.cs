using System.Diagnostics.CodeAnalysis;

namespace RelogicLabs.JsonSchema.Tree;

public class Location
{
    public required int Line { get; init; }
    public required int Column { get; init; }

    [SetsRequiredMembers]
    public Location(int line, int column)
    {
        Line = line;
        Column = column;
    }

    public override string ToString() => $"{Line}:{Column}";
}