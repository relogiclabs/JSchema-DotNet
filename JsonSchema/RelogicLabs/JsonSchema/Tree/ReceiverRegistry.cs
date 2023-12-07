using RelogicLabs.JsonSchema.Types;

namespace RelogicLabs.JsonSchema.Tree;

public class ReceiverRegistry
{
    public Dictionary<JReceiver, List<JNode>> Receivers { get; } = new();

    public void Register(IEnumerable<JReceiver> receivers)
    {
        foreach(var r in receivers) Receivers[r] = new List<JNode>();
    }

    public void Receive(IEnumerable<JReceiver> receivers, JNode node)
    {
        foreach(var r in receivers) Receivers[r].Add(node);
    }

    public List<JNode>? Fetch(JReceiver receiver)
    {
        Receivers.TryGetValue(receiver, out var list);
        return list;
    }

    public void Clear()
    {
        foreach(var pair in Receivers) pair.Value.Clear();
    }
}