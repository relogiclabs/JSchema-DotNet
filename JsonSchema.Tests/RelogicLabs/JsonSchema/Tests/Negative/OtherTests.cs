using RelogicLabs.JsonSchema.Exceptions;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Tests.Negative;

[TestClass]
public class OtherTests
{
    [TestMethod]
    public void When_JsonNotFloat_ExceptionThrown()
    {
        var schema = "#float";
        var json = "2.5E10";
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP02, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_JsonNotDouble_ExceptionThrown()
    {
        var schema = "#double";
        var json = "\"string\"";
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP02, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_JsonNotNull_ExceptionThrown()
    {
        var schema = "#null";
        var json = "0";
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP02, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_JsonValueNotEqualForFloat_ExceptionThrown()
    {
        var schema = "3.5 #float";
        var json = "2.5";
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(FLOT01, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_JsonValueNotEqualForDouble_ExceptionThrown()
    {
        var schema = "2.5E0 #double";
        var json = "2.5E1";
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DUBL01, exception.ErrorCode);
        Console.WriteLine(exception);
    }
}