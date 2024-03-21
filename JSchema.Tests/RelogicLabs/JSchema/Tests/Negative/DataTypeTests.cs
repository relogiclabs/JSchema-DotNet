using RelogicLabs.JSchema.Exceptions;
using static RelogicLabs.JSchema.Message.ErrorCode;

namespace RelogicLabs.JSchema.Tests.Negative;

[TestClass]
public class DataTypeTests
{
    [TestMethod]
    public void When_WrongJsonWithDirectDataType_ExceptionThrown()
    {
        var schema =
            """
            #string* #array
            """;
        var json =
            """
            10
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_WrongJsonWithNestedDataType_ExceptionThrown()
    {
        var schema =
            """
            #string* #array
            """;
        var json =
            """
            [10, 20]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP06, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_NestedTypeWithNonCompositeJson_ExceptionThrown()
    {
        var schema =
            """
            #string*
            """;
        var json =
            """
            10
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP03, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_UndefinedDataTypeArgument_ExceptionThrown()
    {
        var schema =
            """
            #array($undefined)
            """;
        var json =
            """
            [10, 20]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<DefinitionNotFoundException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DEFI03, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_UndefinedNestedDataTypeArgument_ExceptionThrown()
    {
        var schema =
            """
            #integer*($undefined) #array
            """;
        var json =
            """
            [10, 20]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<DefinitionNotFoundException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DEFI04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_DataTypeArgumentWithValidationFailed_ExceptionThrown()
    {
        var schema =
            """
            %define $test: {"k1": #string}
            %schema: #object($test)
            """;
        var json =
            """
            {"k1": 10}
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_NestedDataTypeArgumentWithValidationFailed_ExceptionThrown()
    {
        var schema =
            """
            %define $test: {"k1": #string}
            %schema: #object*($test) #array
            """;
        var json =
            """
            [{"k1": 10}]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_MultipleNestedDataTypeWithWrongValueInObject_ExceptionThrown()
    {
        var schema =
            """
            {
                "key1": #date* #time* #null* #array,
                "key2": #string* #null* #array,
                "key3": #integer* #float* #array
            }
            """;
        var json =
            """
            {
                "key1": ["2021-08-01", "2021-08-01T15:50:30.300Z", "test"],
                "key2": ["test", null, "text", 10],
                "key3": [false, true, null, "text"]
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP06, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_DataTypeExceptionCountInObject_ExceptionThrown()
    {
        var schema =
            """
            {
                "key1": #boolean* #integer #string #array,
                "key2": #boolean* #integer #string,
                "key3": #boolean* #integer #string #array
            }
            """;
        var json =
            """
            {
                "key1": [10, "test", "2021-08-01"],
                "key2": [10, "test", "2021-08-01"],
                "key3": []
            }
            """;
        var jsonSchema = new JsonSchema(schema);
        if(!jsonSchema.IsValid(json)) jsonSchema.WriteError();
        Assert.AreEqual(8, jsonSchema.Exceptions.Count);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP06, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_MultipleDataTypeWithWrongValueInObject_ExceptionThrown()
    {
        var schema =
            """
            {
                "key1": #date #time #null,
                "key2": #array #object #null,
                "key3": #integer #float,
                "key4": #integer #date #null,
                "key5": #number #string #null
            }
            """;
        var json =
            """
            {
                "key1": "2021-08-01",
                "key2": null,
                "key3": 100,
                "key4": "test",
                "key5": false
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }
}