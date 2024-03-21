using System.Collections;
using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Types;
using static RelogicLabs.JSchema.Engine.ScriptTreeHelper;
using static RelogicLabs.JSchema.Message.ErrorCode;

namespace RelogicLabs.JSchema.Script;

internal sealed class GIterator : IEnumerable<IEValue>
{
    private readonly IEValue _iterable;

    public GIterator(IEValue iterable) => _iterable = Dereference(iterable);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<IEValue> GetEnumerator()
    {
        if(_iterable is IEArray a) return GetEnumerator(a);
        if(_iterable is IEObject o) return GetEnumerator(o);
        if(_iterable is IEString s) return GetEnumerator(s);
        throw new ScriptCommonException(ITER01,
            $"Invalid type {_iterable.Type} for iteration");
    }

    private static IEnumerator<IEValue> GetEnumerator(IEObject source)
        => source.Keys.Select(GString.Of).GetEnumerator();

    private static IEnumerator<IEValue> GetEnumerator(IEString source)
        => source.Value.Select(GString.Of).GetEnumerator();

    private static IEnumerator<IEValue> GetEnumerator(IEArray source)
    {
        while(source.Count == 1)
        {
            if(source.Get(0) is IEArray) source = (IEArray) source.Get(0);
            else break;
        }
        return source.Values.GetEnumerator();
    }
}