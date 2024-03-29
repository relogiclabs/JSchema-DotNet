using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Message;
using RelogicLabs.JSchema.Tree;
using RelogicLabs.JSchema.Types;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.ErrorDetail;
using static RelogicLabs.JSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JSchema.Nodes;

public abstract class JNode : IEValue
{
    // To make complete tree read only and immutable
    private readonly IDictionary<JNode, JNode> _relations;
    public Context Context { get; }
    public virtual JNode? Parent => _relations.TryGetValue(this);
    public virtual IEnumerable<JNode> Children
    { get; private protected init; } = Enumerable.Empty<JNode>();
    public RuntimeContext Runtime => Context.Runtime;

    private protected JNode(Builder builder)
    {
        _relations = RequireNonNull(builder.Relations);
        Context = RequireNonNull(builder.Context);
    }

    private protected JNode(JNode node)
    {
        _relations = RequireNonNull(node._relations);
        Context = RequireNonNull(node.Context);
        Children = RequireNonNull(node.Children);
    }

    private T Initialize<T>() where T : JNode
    {
        foreach(var c in Children) _relations[c] = this;
        return (T) this;
    }

    /// <summary>
    /// Determines whether the specified node matches with the current node.
    /// </summary>
    /// <param name="node">The node to compare with the current node.</param>
    /// <returns>
    /// <c>true</c> if the specified node matches with the current node;
    /// otherwise, <c>false</c>.
    /// </returns>
    public abstract bool Match(JNode node);

    /// <summary>
    /// Returns a JSON string that represents the current object.
    /// </summary>
    /// <returns>A JSON string that represents the current object.</returns>
    public override string ToString() => GetType().ToString();

    /// <summary>
    /// Returns an abbreviated outline version of the string obtained from the
    /// <see cref="ToString"/> method of the specified length from
    /// <see cref="OutlineFormatter.OutlineLength"/> and replaces a portion of the
    /// string with ellipses to match the specified length; otherwise, returns the
    /// string unmodified.
    /// </summary>
    /// <returns>
    /// An abbreviated outline version of the <see cref="ToString"/> string.
    /// </returns>
    public virtual string GetOutline() => OutlineFormatter.CreateOutline(ToString());

    private protected T? CastType<T>(JNode node)
    {
        if(node is T other) return other;
        Fail(new JsonSchemaException(
            new ErrorDetail(DTYP02, DataTypeMismatch),
            ExpectedDetail.AsDataTypeMismatch(this),
            ActualDetail.AsDataTypeMismatch(node)));
        return default;
    }

    private protected bool CheckType<T>(JNode node)
        => CastType<T>(node) != null;

    private protected bool Fail(Exception exception)
        => Runtime.Exceptions.Fail(exception);

    internal abstract class Builder
    {
        public IDictionary<JNode, JNode>? Relations { get; init; }
        public Context? Context { get; init; }
        public abstract JNode Build();
        protected static T Build<T>(T node) where T : JNode => node.Initialize<T>();
    }
}