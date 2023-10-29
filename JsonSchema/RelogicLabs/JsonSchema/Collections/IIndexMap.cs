namespace RelogicLabs.JsonSchema.Collections;

public interface IIndexMap<TK, TV> : IList<TV>
    where TV : IKeyer<TK> where TK : notnull
{
    TV this[TK key] { get; }
    IEnumerable<TK> Keys { get; }
    IEnumerable<TV> Values { get; }
    bool TryGetValue(TK key, out TV? value);
    IIndexMap<TK, TV> AsReadOnly();
}