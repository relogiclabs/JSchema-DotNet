using RelogicLabs.JsonSchema.Exceptions;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Tests.Negative;

[TestClass]
public class ObjectTests
{
    [TestMethod]
    public void When_JsonNotObject_ExceptionThrown()
    {
        var schema = "#object";
        var json = "100";
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP02, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_JsonNotObjectInObject_ExceptionThrown()
    {
        var schema =
            """
            { 
                "key1": #object,
                "key2": #object,
                "key3": #object
            }
            """;
        var json =
            """
            { 
                "key1": [],
                "key2": "value1",
                "key3": [10, 20, 30]
            }
            """;
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP02, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_JsonNotObjectInArray_ExceptionThrown()
    {
        var schema =
            """
            [#object, #object, #object]
            """;
        var json =
            """
            [null, "value1", true]
            """;
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP02, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_NestedJsonNotObjectInArray_ExceptionThrown()
    {
        var schema =
            """
            #object*
            """;
        var json =
            """
            [ 100, true, false ]
            """;
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP02, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_NestedJsonNotObjectInObject_ExceptionThrown()
    {
        var schema =
            """
            #object*
            """;
        var json =
            """
            { 
                "key1": 15.4,
                "key2": 0,
                "key3": [10]
            }
            """;
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP02, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_KeysWithWrongObject_ExceptionThrown()
    {
        var schema =
            """
            @keys("key1", "key2") #integer*
            """;
        var json =
            """
            {
                "key4": 100,
                "key5": 150,
                "key6": 200
            }
            """;
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(KEYS01, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_ValuesWithWrongObject_ExceptionThrown()
    {
        var schema =
            """
            @values(100, 200) #integer*
            """;
        var json =
            """
            {
                "key1": 1,
                "key2": 2,
                "key3": 3
            }
            """;
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(VALU01, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_NestedKeysWithWrongObjectInObject_ExceptionThrown()
    {
        var schema =
            """
            @keys*("key") #object*
            """;
        var json =
            """
            {
                "key1": {"value": 10},
                "key2": {"value": 150},
                "key3": {"value": 1000}
            }
            """;
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(KEYS01, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_NestedKeysAndValuesWithWrongObjectInArray_ExceptionThrown()
    {
        var schema =
            """
            @keys*("key1", "key2") @values*(100, 200) #object*
            """;
        var json =
            """
            [{"value": 10}, {"value": 20}, {"value": 30}]
            """;
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(KEYS01, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_EnumWithWrongObject_ExceptionThrown()
    {
        var schema = 
            """
            {
            "key1": @enum(5, 10, 15),
            "key2": @enum(100, 150, 200),
            "key3": @enum("abc", "pqr", "xyz")
            } #object
            """;
        var json = 
            """
            {
            "key1": 1, 
            "key2": 10, 
            "key3": "efg"
            }
            """;
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(ENUM02, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_DuplicateJsonPropertyInObject_ExceptionThrown()
    {
        var schema =
            """
            {
                "key1": #object,
                "key2": #object,
                "key3": #object
            }
            """;
        var json =
            """
            {
                "key1": [],
                "key2": "value1",
                "key2": [10, 20, 30]
            }
            """;
        
        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<DuplicatePropertyKeyException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(PROP03, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_DuplicateSchemaPropertyInObject_ExceptionThrown()
    {
        var schema =
            """
            {
                "key1": #object,
                "key2": #object,
                "key2": #object
            }
            """;
        var json =
            """
            {
                "key1": [],
                "key2": "value1",
                "key3": [10, 20, 30]
            }
            """;
        
        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<DuplicatePropertyKeyException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(PROP04, exception.ErrorCode);
        Console.WriteLine(exception);
    }
}