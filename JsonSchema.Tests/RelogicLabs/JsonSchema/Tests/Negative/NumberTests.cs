using RelogicLabs.JsonSchema.Exceptions;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Tests.Negative;

[TestClass]
public class NumberTests
{
    [TestMethod]
    public void When_JsonLessThanMinimumFloat_ExceptionThrown()
    {
        var schema =
            """
            @minimum(10) #float
            """;
        var json =
            """
            9.999999
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(MINI01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonGreaterThanMaximumInteger_ExceptionThrown()
    {
        var schema =
            """
            @maximum(10) #integer
            """;
        var json =
            """
            1000
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(MAXI01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_WrongJsonWithNestedMinimumIntegerInArray_ExceptionThrown()
    {
        var schema =
            """
            @minimum*(10.5) #integer*
            """;
        var json =
            """
            [
                1111,
                100,
                10
            ]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(MINI01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_WrongJsonWithNestedMinimumFloatInObject_ExceptionThrown()
    {
        var schema =
            """
            @minimum*(100) #number*
            """;
        var json =
            """
            {
                "key1": 100.000,
                "key2": 99.99,
                "key3": 200.884
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(MINI01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_WrongJsonWithNestedMaximumNumberInArray_ExceptionThrown()
    {
        var schema =
            """
            @maximum*(1000.05) #number*
            """;
        var json =
            """
            [
                -1000.05,
                1000.05,
                1001
            ]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(MAXI01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_NestedMaximumFloatInObject_ValidTrue()
    {
        var schema =
            """
            @maximum*(100) #float*
            """;
        var json =
            """
            {
                "key1": 100.000,
                "key2": -150.407,
                "key3": 100.00001
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(MAXI01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_NestedMinimumExclusiveFloatInObject_ValidTrue()
    {
        var schema =
            """
            @minimum*(100, true) #float*
            """;
        var json =
            """
            {
                "key1": 500.999,
                "key2": 100.001,
                "key3": 100.000
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(MINI03, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_NestedMaximumExclusiveFloatInObject_ValidTrue()
    {
        var schema =
            """
            @maximum*(100, true) #float*
            """;
        var json =
            """
            {
                "key1": 99.999,
                "key2": 10.407,
                "key3": 100.000
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(MAXI03, exception.Code);
        Console.WriteLine(exception);
    }
}