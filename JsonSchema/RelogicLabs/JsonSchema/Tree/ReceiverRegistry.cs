using System.Collections;
using RelogicLabs.JsonSchema.Types;

namespace RelogicLabs.JsonSchema.Tree;

public class ReceiverRegistry : IEnumerable<KeyValuePair<JReceiver, List<JNode>>>
{
    private readonly Dictionary<JReceiver, List<JNode>> _receivers = new();

    public void Register(IEnumerable<JReceiver> receivers)
    {
        foreach(var r in receivers) _receivers[r] = new List<JNode>();
    }

    public void Receive(IEnumerable<JReceiver> receivers, JNode node)
    {
        foreach(var r in receivers) _receivers[r].Add(node);
    }

    public List<JNode>? Fetch(JReceiver receiver)
    {
        _receivers.TryGetValue(receiver, out var list);
        return list;
    }

    public void Clear()
    {
        foreach(var pair in _receivers) pair.Value.Clear();
    }

    public IEnumerator<KeyValuePair<JReceiver, List<JNode>>> GetEnumerator()
        => _receivers.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}