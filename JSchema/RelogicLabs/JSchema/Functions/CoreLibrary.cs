using RelogicLabs.JSchema.Tree;

namespace RelogicLabs.JSchema.Functions;

internal static class CoreLibrary
{
    private static readonly FunctionMap _Functions = FunctionLoader.Load(typeof(CoreFunctions));

    public static List<IEFunction>? GetFunctions(string key)
        => _Functions.TryGetValue(key);
}