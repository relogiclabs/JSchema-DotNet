namespace RelogicLabs.JSchema.Types;

public sealed class EType
{
    public static readonly EType NUMBER = new("#number");
    public static readonly EType INTEGER = new("#integer");
    public static readonly EType FLOAT = new("#float");
    public static readonly EType DOUBLE = new("#double");
    public static readonly EType STRING = new("#string");
    public static readonly EType ARRAY = new("#array");
    public static readonly EType RANGE = new("#range");
    public static readonly EType OBJECT = new("#object");
    public static readonly EType BOOLEAN = new("#boolean");
    public static readonly EType DATETIME = new("#datetime");
    public static readonly EType DATE = new("#date");
    public static readonly EType TIME = new("#time");
    public static readonly EType PRIMITIVE = new("#primitive");
    public static readonly EType COMPOSITE = new("#composite");
    public static readonly EType ANY = new("#any");
    public static readonly EType NULL = new("#null");
    public static readonly EType UNDEFINED = new("#undefined");
    public static readonly EType VOID = new("#void");

    public string Name { get; }

    private EType(string name) => Name = name;
    public override string ToString() => Name;
}