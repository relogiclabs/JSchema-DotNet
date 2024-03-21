namespace RelogicLabs.JSchema.Collections;

public interface IKeyed<out TK>
{
    TK Key { get; }
}