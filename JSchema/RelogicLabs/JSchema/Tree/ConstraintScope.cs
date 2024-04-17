using RelogicLabs.JSchema.Engine;
using RelogicLabs.JSchema.Nodes;
using RelogicLabs.JSchema.Script;
using RelogicLabs.JSchema.Types;

namespace RelogicLabs.JSchema.Tree;

internal sealed class ConstraintScope : ScriptScope
{
    private readonly List<GReference> _receivers = new(4);

    public ConstraintScope(ScriptScope? parent) : base(parent) { }

    public override GReference AddVariable(string name, IEValue value)
    {
        var reference = base.AddVariable(name, value);
        if(value is JReceiver) _receivers.Add(reference);
        return reference;
    }

    public void UnpackReceivers()
    {
        foreach(var reference in _receivers)
        {
            var list = ((JReceiver) reference.Value).Values;
            if(list.Count == 1) reference.Value = list[0];
        }
    }
}