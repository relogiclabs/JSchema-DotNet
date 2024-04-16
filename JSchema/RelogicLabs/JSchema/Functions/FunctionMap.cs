using RelogicLabs.JSchema.Tree;
using RelogicLabs.JSchema.Utilities;

namespace RelogicLabs.JSchema.Functions;

internal sealed class FunctionMap
{
    private readonly Dictionary<string, List<IEFunction>> _functions = new();

    public void MergeWith(FunctionMap other)
    {
        foreach(var pair in other._functions)
        {
            _functions.TryGetValue(pair.Key, out var list);
            if(list == default) _functions.Add(pair.Key, pair.Value);
            else list.AddRange(pair.Value);
        }
    }

    public void Add(IEFunction function)
    {
        var key = FunctionId.Generate(function);
        var list = _functions.TryGetValue(key) ?? new List<IEFunction>();
        list.Add(function);
        _functions[key] = list;
    }

    public List<IEFunction>? TryGetValue(string key) => _functions.TryGetValue(key);
}