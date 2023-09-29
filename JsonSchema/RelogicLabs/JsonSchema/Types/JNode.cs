using Antlr4.Runtime;
using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Tree;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Message.ErrorDetail;

namespace RelogicLabs.JsonSchema.Types;

public abstract class JNode
{
    // To make complete tree read only and immutable
    internal readonly IDictionary<JNode, JNode> _relations;
    public required Context Context { get; init; }
    public virtual JNode? Parent => _relations[this];
    public abstract IEnumerable<JNode> Children { get; }
    public ParserRuleContext Parser => Context.Parser;
    public RuntimeContext Runtime => Context.Runtime;

    internal JNode(IDictionary<JNode, JNode> relations)
        => _relations = relations;

    internal virtual JNode Initialize()
    {
        foreach(var c in Children) _relations[c] = this;
        return this;
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
    /// <see cref="MessageFormatter.OutlineLength"/> and replaces a portion of the
    /// string with ellipses to match the specified length; otherwise, returns the
    /// string unmodified.
    /// </summary>
    /// <returns>
    /// An abbreviated outline version of the <see cref="ToString"/> string.
    /// </returns>
    public virtual string GetOutline()
        => Context.MessageFormatter.CreateOutline(ToString());

    protected T? CastType<T>(JNode node)
    {
        if(node is T other) return other;
        FailWith(new JsonSchemaException(
            new ErrorDetail(DTYP03, DataTypeMismatch),
            ExpectedDetail.AsDataTypeMismatch(this),
            ActualDetail.AsDataTypeMismatch(node)));
        return default;
    }

    protected bool CheckType<T>(JNode node)
        => CastType<T>(node) != null;

    internal bool FailWith(Exception exception)
        => Runtime.FailWith(exception);
}