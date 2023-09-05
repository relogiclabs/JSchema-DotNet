namespace RelogicLabs.JsonSchema.Tree;

internal class PragmaProfile<T> : PragmaDescriptor
{
    public T DefaultValue { get; }
    
    public PragmaProfile(string name, Type type, T defaultValue) 
        : base(name, type)
    {
        DefaultValue = defaultValue;
    }
}