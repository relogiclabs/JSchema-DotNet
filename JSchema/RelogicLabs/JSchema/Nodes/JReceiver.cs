using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Types;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.MessageFormatter;
using static RelogicLabs.JSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JSchema.Nodes;

public sealed class JReceiver : JLeaf, IEArray
{
    public string Name { get; }
    public IReadOnlyList<IEValue> Values => FetchValueNodes();

    private JReceiver(Builder builder) : base(builder)
        => Name = RequireNonNull(builder.Name);

    public override bool Match(JNode node)
        => throw new InvalidOperationException("Invalid runtime state");

    private List<JNode> FetchValueNodes()
    {
        var list = Runtime.Receivers.Fetch(this);
        if(list is null) throw new ReceiverNotFoundException(
            FormatForSchema(RECV01, $"Receiver '{Name}' not found", this));
        return list;
    }

    public int GetValueCount() => FetchValueNodes().Count;

    public T GetValueNode<T>() where T : JNode
    {
        var list = FetchValueNodes();
        if(list.IsEmpty()) throw new NoValueReceivedException(
            FormatForSchema(RECV02, $"No value received for '{Name}'", this));
        if(list.Count > 1) throw new NotSupportedException("Multiple values exist");
        return (T) list[0];
    }

    public IList<T> GetValueNodes<T>() where T : JNode
        => FetchValueNodes().Cast<T>().ToList().AsReadOnly();

    public IEValue Get(int index) => FetchValueNodes()[index];

    public override bool Equals(object? obj)
    {
        if(ReferenceEquals(null, obj)) return false;
        if(ReferenceEquals(this, obj)) return true;
        if(obj.GetType() != this.GetType()) return false;
        JReceiver other = (JReceiver) obj;
        return Name == other.Name;
    }

    public override int GetHashCode() => Name.GetHashCode();
    public override string ToString() => Name;

    internal new sealed class Builder : JNode.Builder
    {
        public string? Name { get; init; }
        public override JReceiver Build() => Build(new JReceiver(this));
    }
}