using RelogicLabs.JSchema.Nodes;
using RelogicLabs.JSchema.Types;

namespace RelogicLabs.JSchema.Tree;

internal interface IEFunction
{
    string Name { get; }
    int Arity { get; }
    Type TargetType { get; }
    IList<object>? Prebind<T>(IList<T> arguments) where T : IEValue;
    object Invoke(JFunction caller, object[] arguments);
}