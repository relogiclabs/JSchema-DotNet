using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Nodes;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Tree.TreeType;
using static RelogicLabs.JSchema.Message.MessageFormatter;

namespace RelogicLabs.JSchema.Tree;

internal static class TreeHelper
{
    internal static IEnumerable<JProperty> RequireUniqueness(List<JProperty> list, TreeType treeType)
    {
        var group = list.GroupBy(static p => p.Key).FirstOrDefault(static g => g.Count() > 1);
        if(group == default) return list;
        var property = group.Last();

        if(treeType == JSON_TREE) throw new DuplicatePropertyKeyException(
            FormatForJson(PROP03, GetMessage(property), property));
        if(treeType == SCHEMA_TREE) throw new DuplicatePropertyKeyException(
            FormatForSchema(PROP04, GetMessage(property), property));

        throw new InvalidOperationException("Invalid parser state");
    }

    private static string GetMessage(JProperty property)
        => $"Multiple key with name '{property.Key}' found";
}