using RelogicLabs.JSchema.Nodes;

namespace RelogicLabs.JSchema.Tree;

public interface IDataTree
{
    public bool Match(IDataTree dataTree);
    public RuntimeContext Runtime { get; }
    public JRoot Root { get; }
    public TreeType Type { get; }
}