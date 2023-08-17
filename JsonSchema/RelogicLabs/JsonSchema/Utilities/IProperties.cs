using System.Diagnostics.CodeAnalysis;

namespace RelogicLabs.JsonSchema.Utilities;

public interface IProperties<T, TK, TV> : IList<T>
    where T : IProperty<TK, TV> where TK : notnull
{
    T this[TK key] { get; }
    IEnumerable<TK> Keys { get; }
    IEnumerable<TV> Values { get; }
    IEnumerable<T> Pairs { get; }
    bool ContainsKey(TK key);
    bool ContainsValue(TV value);
    bool TryGetValue(TK key, [MaybeNullWhen(false)] out T value);
}