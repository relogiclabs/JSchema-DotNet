using System.Collections;
using RelogicLabs.JSchema.Nodes;
using static RelogicLabs.JSchema.Tree.FunctionRegistry;

namespace RelogicLabs.JSchema.Tree;

internal sealed class FunctionCache : IEnumerable<FunctionCache.Entry>
{
    public sealed record Entry(IEFunction Function, object[] Arguments)
    {
        public bool IsTargetMatch(JNode target)
            => IsMatch(Function.TargetType, target);

        public object Invoke(JFunction caller, JNode target)
        {
            Arguments[0] = GetDerived(target);
            return Function.Invoke(caller, Arguments);
        }
    }

    public static int SizeLimit { get; set; } = 10;
    private readonly List<Entry> _cache = new(SizeLimit);

    public void Add(IEFunction function, object[] arguments)
    {
        if(_cache.Count > SizeLimit) _cache.RemoveAt(0);
        arguments[0] = null!;
        _cache.Add(new Entry(function, arguments));
    }

    public IEnumerator<Entry> GetEnumerator() => _cache.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}