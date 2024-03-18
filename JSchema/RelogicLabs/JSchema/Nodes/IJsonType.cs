using RelogicLabs.JSchema.Types;

namespace RelogicLabs.JSchema.Nodes;

public interface IJsonType : IEValue
{
    public JNode Node { get; }
}