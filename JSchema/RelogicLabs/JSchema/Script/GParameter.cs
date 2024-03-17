namespace RelogicLabs.JSchema.Script;

internal sealed class GParameter
{
    private const string VariadicSuffix = "...";
    public string Name { get; }
    public bool Variadic { get; }

    public GParameter(string name)
    {
        if(name.EndsWith(VariadicSuffix))
        {
            Name = name[..^3];
            Variadic = true;
        }
        else
        {
            Name = name;
            Variadic = false;
        }
    }

    public override string ToString() => Variadic ? Name + VariadicSuffix : Name;
}