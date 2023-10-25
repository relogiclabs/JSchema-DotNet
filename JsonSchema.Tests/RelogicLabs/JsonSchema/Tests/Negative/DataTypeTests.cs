using RelogicLabs.JsonSchema.Exceptions;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Tests.Negative;

[TestClass]
public class DataTypeTests
{
    [TestMethod]
    public void When_JsonWithWrongMainDataType_ExceptionThrown()
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
    public void When_JsonWithWrongNestedDataType_ExceptionThrown()
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
}