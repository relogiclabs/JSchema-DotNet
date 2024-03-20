using RelogicLabs.JSchema.Exceptions;
using static RelogicLabs.JSchema.Message.ErrorCode;

namespace RelogicLabs.JSchema.Tests.Negative;

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
    public void When_NestedMaximumWrongFloatInObject_ExceptionThrown()
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
    public void When_NestedMinimumExclusiveWrongFloatInObject_ExceptionThrown()
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
    public void When_NestedMaximumExclusiveWrongFloatInObject_ExceptionThrown()
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

    [TestMethod]
    public void When_NestedPositiveWithWrongNumberInArray_ExceptionThrown()
    {
        var schema =
            """
            @positive* #number*
            """;
        var json =
            """
            [1, 100.5, -500]
            """;

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(POSI01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_NestedPositiveReferenceWithWrongNumberInArray_ExceptionThrown()
    {
        var schema =
            """
            @positive*(0) #number*
            """;
        var json =
            """
            [0, 100, 0.1, -1]
            """;

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(POSI02, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_NestedNegativeWithWrongNumberInArray_ExceptionThrown()
    {
        var schema =
            """
            @negative* #number*
            """;
        var json =
            """
            [-100, -500, -0.1, 0]
            """;

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(NEGI01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_NestedNegativeReferenceWithWrongNumberInArray_ExceptionThrown()
    {
        var schema =
            """
            @negative*(0) #number*
            """;
        var json =
            """
            [-100, -500, -0.01, 1]
            """;

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(NEGI02, exception.Code);
        Console.WriteLine(exception);
    }
}