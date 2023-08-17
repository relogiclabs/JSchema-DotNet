namespace RelogicLabs.JsonSchema.Types;

public interface IJsonComposite
{
    IList<JNode> ExtractComponents();
}