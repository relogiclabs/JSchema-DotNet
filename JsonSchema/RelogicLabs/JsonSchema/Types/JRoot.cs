using System.Text;
using RelogicLabs.JsonSchema.Utilities;
using static System.Environment;

namespace RelogicLabs.JsonSchema.Types;

public class JRoot : JNode
{
    private IEnumerable<JNode> _children = Enumerable.Empty<JNode>();

    public JTitle? Title { get; init; }
    public JVersion? Version { get; init; }
    public IList<JInclude>? Includes { get; init; }
    public IList<JPragma>? Pragmas { get; init; }
    public IList<JDefinition>? Definitions { get; init; }
    public required JNode Value { get; init; }
    
    public override JNode Parent => null!;
    public override IEnumerable<JNode> Children => _children;

    internal JRoot(IDictionary<JNode, JNode> relations) : base(relations) { }
    
    // Avoid enumeration with yield for performance
    internal override JRoot Initialize()
    {
        _relations[this] = null!;
        List<JNode> list = new();
        AddNodeToList(Title);
        AddNodeToList(Version);
        AddNodesToList(Includes);
        AddNodesToList(Pragmas);
        AddNodesToList(Definitions);
        AddNodeToList(Value);
        _children = list.AsReadOnly();
        return (JRoot) base.Initialize();

        void AddNodeToList(JNode? node) { if(node != null) list.Add(node); }
        void AddNodesToList(IEnumerable<JNode>? nodes) { if(nodes != null) 
            list.AddRange(nodes); }
    }

    public override bool Match(JNode node)
    {
        var other = CastType<JRoot>(node);
        if(other == null) return false;
        return Value.Match(other.Value);
    }

    public override string ToString()
    {
        StringBuilder builder = new();
        AppendTo(builder, Title?.ToString());
        AppendTo(builder, Version?.ToString());
        AppendTo(builder, Includes?.ToString(NewLine));
        AppendTo(builder, Pragmas?.ToString(NewLine));
        AppendTo(builder, Definitions?.ToString(NewLine));
        AppendTo(builder, Value.ToString());
        return builder.ToString().Trim();
    }
    
    private void AppendTo(StringBuilder builder, string? text)
    {
        if(text is null || text.Length == 0) return;
        builder.Append(text).Append(NewLine);
    }
}