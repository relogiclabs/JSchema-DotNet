namespace RelogicLabs.JsonSchema.Tree;

internal class PragmaDescriptor<T> : PragmaPreset
{
    public T DefaultValue { get; }
    
    public PragmaDescriptor(string name, Type type, T defaultValue) 
        : base(name, type)
    {
        DefaultValue = defaultValue;
    }
}