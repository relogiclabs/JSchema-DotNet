namespace RelogicLabs.JsonSchema.Collections;

public interface IKeyer<out TK>
{
    TK GetKey();
}