using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JsonSchema.Types;

public sealed class JReceiver : JLeaf
{
    public string Name { get; }

    private JReceiver(Builder builder) : base(builder)
        => Name = RequireNonNull(builder.Name);

    public override bool Match(JNode node)
        => throw new InvalidOperationException("Invalid runtime state");

    public int GetValueCount()
    {
        var list = Runtime.Receivers.Fetch(this);
        if(ReferenceEquals(list, null)) throw new ReceiverNotFoundException(MessageFormatter
            .FormatForSchema(RECV01, $"Receiver '{Name}' not found", Context));
        return list.Count;
    }

    public T GetValueNode<T>() where T : JNode
    {
        var list = Runtime.Receivers.Fetch(this);
        if(ReferenceEquals(list, null)) throw new ReceiverNotFoundException(MessageFormatter
            .FormatForSchema(RECV02, $"Receiver '{Name}' not found", Context));
        if(list.Count == 0) throw new NoValueReceivedException(MessageFormatter
            .FormatForSchema(RECV03, $"No value received for '{Name}'", Context));
        if(list.Count > 1) throw new InvalidOperationException("Multiple values found");
        return (T) list[0];
    }

    public IList<T> GetValueNodes<T>() where T : JNode
    {
        var list = Runtime.Receivers.Fetch(this);
        if(ReferenceEquals(list, null)) throw new ReceiverNotFoundException(MessageFormatter
            .FormatForSchema(RECV04, $"Receiver '{Name}' not found", Context));
        return list.Select(i => (T) i).ToList().AsReadOnly();
    }

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

    internal new class Builder : JNode.Builder
    {
        public string? Name { get; init; }
        public override JReceiver Build() => Build(new JReceiver(this));
    }
}