using RelogicLabs.JsonSchema.Exceptions;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Tests.Negative;

[TestClass]
public class BooleanTests
{
    [TestMethod]
    public void When_JsonNotBoolean_ExceptionThrown()
    {
        var schema = "#boolean";
        var json = "5";
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_JsonValueNotEqualForBoolean_ExceptionThrown()
    {
        var schema = "true #boolean";
        var json = "false";
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(BOOL01, exception.Code);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_JsonNotBooleanInObject_ExceptionThrown()
    {
        var schema =
            """
            {
                "key1": #boolean,
                "key2": #boolean,
                "key3": #boolean
            }
            """;
        var json =
            """
            {
                "key1": null,
                "key2": 10.5,
                "key3": true
            }
            """;
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_JsonNotBooleanInArray_ExceptionThrown()
    {
        var schema =
            """
            [#boolean, #boolean, #boolean]
            """;
        var json =
            """
            [[], 11.5, "false"]
            """;
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_NestedJsonNotBooleanInArray_ExceptionThrown()
    {
        var schema =
            """
            #boolean*
            """;
        var json =
            """
            ["true", {}, [true]]
            """;
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP06, exception.Code);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_NestedJsonNotBooleanInObject_ExceptionThrown()
    {
        var schema =
            """
            #boolean*
            """;
        var json =
            """
            {
                "key1": {"key": true},
                "key2": null,
                "key3": false
            }
            """;
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP06, exception.Code);
        Console.WriteLine(exception);
    }
}