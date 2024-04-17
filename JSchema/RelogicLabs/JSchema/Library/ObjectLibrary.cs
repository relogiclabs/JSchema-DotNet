using RelogicLabs.JSchema.Engine;
using RelogicLabs.JSchema.Script;
using RelogicLabs.JSchema.Types;

namespace RelogicLabs.JSchema.Library;

internal sealed class ObjectLibrary : CommonLibrary
{
    private const string Size_M0 = "size#0";
    private const string Length_M0 = "length#0";
    private const string Copy_M0 = "copy#0";

    public new static ObjectLibrary Instance { get; } = new();
    protected override EType Type => EType.OBJECT;

    private ObjectLibrary()
    {
        AddMethod(Size_M0, SizeMethod);
        AddMethod(Length_M0, SizeMethod);
        AddMethod(Copy_M0, CopyMethod);
    }

    private static GInteger SizeMethod(IEValue self, List<IEValue> arguments, ScriptScope scope)
        => GInteger.From(((IEObject) self).Count);

    private static GObject CopyMethod(IEValue self, List<IEValue> arguments, ScriptScope scope)
        => new((IEObject) self);
}