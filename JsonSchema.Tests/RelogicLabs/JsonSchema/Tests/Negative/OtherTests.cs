using RelogicLabs.JsonSchema.Exceptions;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Tests.Negative;

[TestClass]
public class OtherTests
{
    [TestMethod]
    public void When_DataTypeNotValid_ExceptionThrown()
    {
        var schema = "#abcd";
        var json = "0";

        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<InvalidDataTypeException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonNotFloat_ExceptionThrown()
    {
        var schema = "#float";
        var json = "2.5E10";

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
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
        Assert.AreEqual(DTYP04, exception.Code);
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
        Assert.AreEqual(DTYP04, exception.Code);
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
        Assert.AreEqual(FLOT01, exception.Code);
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
        Assert.AreEqual(DUBL01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_NonStaticValidMethodWithWrongJson_ExceptionThrown()
    {
        var schema =
            """
            {
                "key1": #array,
                "key2": #array
            }
            """;
        var json1 =
            """
            {
                "key1": [1, 10, 100],
                "key2": [100, 1000, [10, 10000]]
            }
            """;
        var json2 =
            """
            {
                "key1": [1, 10, 100],
                "key2": "string"
            }
            """;
        var jsonAssert = new JsonAssert(schema);
        jsonAssert.IsValid(json1);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => jsonAssert.IsValid(json2));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }
}