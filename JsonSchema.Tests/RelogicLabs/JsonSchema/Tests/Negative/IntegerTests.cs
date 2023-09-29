using RelogicLabs.JsonSchema.Exceptions;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Tests.Negative;

[TestClass]
public class IntegerTests
{
    [TestMethod]
    public void When_JsonNotInteger_ExceptionThrown()
    {
        var schema = "#integer";
        var json = "10.5";
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_JsonValueNotEqualForInteger_ExceptionThrown()
    {
        var schema = "10 #integer";
        var json = "9";
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(INTE01, exception.Code);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_JsonNotIntegerInObject_ExceptionThrown()
    {
        var schema =
            """
            { 
                "key1": #integer,
                "key2": #integer,
                "key3": #integer
            }
            """;
        var json =
            """
            { 
                "key1": null,
                "key2": "value1",
                "key3": 4000.45
            }
            """;
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_JsonNotIntegerInArray_ExceptionThrown()
    {
        var schema =
            """
            [#integer, #integer, #integer]
            """;
        var json =
            """
            [true, -4568.57, 100]
            """;
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_NestedJsonNotIntegerInArray_ExceptionThrown()
    {
        var schema =
            """
            #integer*
            """;
        var json =
            """
            [null, 2.2, "40000000"]
            """;
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP06, exception.Code);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_NestedJsonNotIntegerInObject_ExceptionThrown()
    {
        var schema =
            """
            #integer*
            """;
        var json =
            """
            { 
                "key1": "value1",
                "key2": false,
                "key3": "-50000"
            }
            """;
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP06, exception.Code);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_NestedRangeWithJsonNotIntegerInObject_ExceptionThrown()
    {
        var schema =
            """
            @range*(-100, 200) #integer*
            """;
        var json =
            """
            {
                "key1": "value1",
                "key2": false,
                "key3": "-50000"
            }
            """;
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP06, exception.Code);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_NestedRangeWithNonCompositeJsonInObject_ExceptionThrown()
    {
        var schema =
            """
            @range*(100, !)
            """;
        var json =
            """
            "value1"
            """;
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(FUNC06, exception.Code);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_NestedRangeWithJsonWrongIntegerInObject_ExceptionThrown()
    {
        var schema =
            """
            @range*(-100, 100) #integer*
            """;
        var json =
            """
            {
                "key1": -100,
                "key2": 100,
                "key3": -500
            }
            """;
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(RANG01, exception.Code);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_NestedRangeWithUndefinedAndWrongIntegerInArray_ExceptionThrown()
    {
        var schema =
            """
            @range*(!, 400) #integer*
            """;
        var json =
            """
            [100, 500, 900]
            """;
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(RANG04, exception.Code);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_NestedPositiveWithWrongIntegerInArray_ExceptionThrown()
    {
        var schema =
            """
            @positive* #integer*
            """;
        var json =
            """
            [100, -500, 900]
            """;
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(POSI01, exception.Code);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_NestedNegativeWithWrongIntegerInArray_ExceptionThrown()
    {
        var schema =
            """
            @negative* #integer*
            """;
        var json =
            """
            [-100, -500, 900]
            """;
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(NEGI01, exception.Code);
        Console.WriteLine(exception);
    }
}