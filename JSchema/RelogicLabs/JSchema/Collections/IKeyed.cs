namespace RelogicLabs.JsonSchema.Collections;

public interface IKeyed<out TK>
{
    TK Key { get; }
}