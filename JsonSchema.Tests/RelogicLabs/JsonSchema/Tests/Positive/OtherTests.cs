namespace RelogicLabs.JsonSchema.Tests.Positive;

[TestClass]
public class OtherTests
{
    [TestMethod]
    public void When_DataTypeFloat_ValidTrue()
    {
        var schema = "#float";
        var json = "2.5";
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_DataTypeDouble_ValidTrue()
    {
        var schema = "#double";
        var json = "2.5E1";
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_DataTypeNull_ValidTrue()
    {
        var schema = "null #null";
        var json = "null";
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_DataTypeUndefined_ValidTrue()
    {
        var schema = "!";
        var json = "1000";
        JsonAssert.IsValid(schema, json);
    }
}