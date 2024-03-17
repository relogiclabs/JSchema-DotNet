using RelogicLabs.JsonSchema.Exceptions;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Tests.Negative;

[TestClass]
public class ArrayTests
{
    [TestMethod]
    public void When_JsonNotArray_ExceptionThrown()
    {
        var schema = "#array";
        var json = "10";

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonNotArrayInObject_ExceptionThrown()
    {
        var schema =
            """
            { 
                "key1": #array,
                "key2": #array,
                "key3": #array
            }
            """;
        var json =
            """
            { 
                "key1": "value1",
                "key2": {"key": "value"},
                "key3": 100000
            }
            """;

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonNotArrayInArray_ExceptionThrown()
    {
        var schema =
            """
            [#array, #array, #array]
            """;
        var json =
            """
            [{}, "value1", 10.5]
            """;

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_NestedJsonNotArrayInArray_ExceptionThrown()
    {
        var schema =
            """
            #array*
            """;
        var json =
            """
            [true, "value1", false]
            """;

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP06, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_NestedJsonNotArrayInObject_ExceptionThrown()
    {
        var schema =
            """
            #array*
            """;
        var json =
            """
            { 
                "key1": 10.11,
                "key2": true,
                "key3": "value1"
            }
            """;

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP06, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_ElementsWithWrongArray_ExceptionThrown()
    {
        var schema = "@elements(10, 20, 30, 40) #array";
        var json = "[5, 10, 15, 20, 25]";

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(ELEM01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_NestedElementsWithWrongArrayInArray_ExceptionThrown()
    {
        var schema = "@elements*(5, 10) #array";
        var json = "[[5, 10], [], [5, 10, 15, 20]]";

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(ELEM01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_EnumWithWrongValueInArray_ExceptionThrown()
    {
        var schema =
            """
            [
            @enum(5, 10, 15), 
            @enum(100, 150, 200),
            @enum("abc", "pqr", "xyz")
            ] #array
            """;
        var json = """[11, 102, "efg"]""";

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(ENUM02, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidJsonInArray_ExceptionThrown()
    {
        var schema = "#array";
        var json = "[,,]";

        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonParserException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(JPRS01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_EmptyArrayInObject_ExceptionThrown()
    {
        var schema =
            """
            {
                "key1": @nonempty #array,
                "key2": @nonempty
            }
            """;
        var json =
            """
            {
                "key1": [],
                "key2": []
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(NEMT02, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonWrongLengthInObject_ExceptionThrown()
    {
        var schema =
            """
            @length*(2) #array* #object
            """;
        var json =
            """
            {
                "key1": [10, 20],
                "key2": [10]
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(ALEN01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonWrongMinimumLengthInObject_ExceptionThrown()
    {
        var schema =
            """
            @length*(2, 4) #array* #object
            """;
        var json =
            """
            {
                "key1": [10, 20],
                "key2": [10]
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(ALEN02, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonWrongMaximumLengthInObject_ExceptionThrown()
    {
        var schema =
            """
            @length*(2, 4) #array* #object
            """;
        var json =
            """
            {
                "key1": [10, 20],
                "key2": [10, 20, 30, 40, 50]
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(ALEN03, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonWrongMinimumLengthWithUndefinedInObject_ExceptionThrown()
    {
        var schema =
            """
            @length*(2, !) #array* #object
            """;
        var json =
            """
            {
                "key1": [10, 20],
                "key2": [10]
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(ALEN04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonWrongMaximumLengthWithUndefinedInObject_ExceptionThrown()
    {
        var schema =
            """
            @length*(!, 4) #array* #object
            """;
        var json =
            """
            {
                "key1": [10, 20],
                "key2": [10, 20, 30, 40, 50]
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(ALEN05, exception.Code);
        Console.WriteLine(exception);
    }
}