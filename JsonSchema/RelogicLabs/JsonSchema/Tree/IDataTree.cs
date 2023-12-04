using RelogicLabs.JsonSchema.Types;

namespace RelogicLabs.JsonSchema.Tree;

public interface IDataTree
{
    public bool Match(IDataTree dataTree);
    public RuntimeContext Runtime { get; }
    public JRoot Root { get; }
    public TreeType Type { get; }
}