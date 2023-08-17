namespace RelogicLabs.JsonSchema.Utilities;

public interface IProperty<out TKey, out TValue>
{
    public TKey GetPropertyKey();
    public TValue GetPropertyValue();
}