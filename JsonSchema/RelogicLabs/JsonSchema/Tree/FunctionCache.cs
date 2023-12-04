using System.Collections;
using RelogicLabs.JsonSchema.Types;

namespace RelogicLabs.JsonSchema.Tree;

internal class FunctionCache : IEnumerable<FunctionCache.Entry>
{
    public record Entry(MethodPointer MethodPointer, object?[] Arguments)
    {
        public bool IsTargetMatch(JNode target) => MethodPointer.Parameters[0]
            .ParameterType.IsInstanceOfType(target.Derived);

        public object Invoke(JFunction function, JNode target)
        {
            Arguments[0] = target.Derived;
            return MethodPointer.Invoke(function, Arguments);
        }
    }

    public static int SizeLimit { get; set; } = 10;
    private List<Entry> _cache;

    public FunctionCache() => _cache = new List<Entry>(SizeLimit);

    public void Add(MethodPointer methodPointer, object?[] arguments)
    {
        if(_cache.Count > SizeLimit) _cache.RemoveAt(0);
        arguments[0] = null;
        _cache.Add(new Entry(methodPointer, arguments));
    }

    public IEnumerator<Entry> GetEnumerator() => _cache.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}