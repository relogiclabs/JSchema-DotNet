using RelogicLabs.JSchema.Exceptions;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Tests.External.ExternalFunctions;

namespace RelogicLabs.JSchema.Tests.Negative;

[TestClass]
public class ReceiverTests
{
    [TestMethod]
    public void When_WrongReceiverNameInObject_ExceptionThrown()
    {
        var schema =
            """
            %import: RelogicLabs.JSchema.Tests.External.ExternalFunctions,
                     RelogicLabs.JSchema.Tests
            %schema:
            {
                "key1": #integer &someName,
                "key2": @condition(&notExist) #integer
            }
            """;
        var json =
            """
            {
                "key1": 5,
                "key2": 6
            }
            """;
        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<ReceiverNotFoundException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(RECV01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_NoValueReceiveInObject_ExceptionThrown()
    {
        var schema =
            """
            %import: RelogicLabs.JSchema.Tests.External.ExternalFunctions,
                     RelogicLabs.JSchema.Tests

            %schema:
            {
                "key1": #integer &relatedValue ?,
                "key2": @condition(&relatedValue) #integer ?
            }
            """;
        var json =
            """
            {
                "key2": 1
            }
            """;
        //JsonSchema.IsValid(schema, json);
        //This exception is avoidable inside @condition when needed
        var exception = Assert.ThrowsException<NoValueReceivedException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(RECV02, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_ConditionFailedInObject_ExceptionThrown()
    {
        var schema =
            """
            %import: RelogicLabs.JSchema.Tests.External.ExternalFunctions,
                     RelogicLabs.JSchema.Tests

            %schema:
            {
                "key1": #integer &relatedValue ?,
                "key2": @condition(&relatedValue) #integer ?
            }
            """;
        var json =
            """
            {
                "key1": 11,
                "key2": 10
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(CONDFUNC01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_ConditionAllFailedInObject_ExceptionThrown()
    {
        var schema =
            """
            %import: RelogicLabs.JSchema.Tests.External.ExternalFunctions,
                     RelogicLabs.JSchema.Tests

            %define $numbers: @range(1, 10) #integer &relatedValues
            %schema:
            {
                "key1": #integer*($numbers) #array,
                "key2": @conditionAll(&relatedValues) #integer
            }
            """;
        var json =
            """
            {
                "key1": [1, 2, 3, 4, 5, 6],
                "key2": 6
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(CONDFUNC02, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_ReceiveWrongValuesInObject_ExceptionThrown()
    {
        var schema =
            """
            %import: RelogicLabs.JSchema.Tests.External.ExternalFunctions,
            RelogicLabs.JSchema.Tests

            %schema:
            {
                "key10": @sumEqual(&relatedData) #integer,
                "key1": #integer &relatedData,
                "key2": #integer &relatedData,
                "key3": #integer &relatedData,
                "key4": #integer &relatedData,
                "key5": #integer &relatedData
            }
            """;
        var json =
            """
            {
                "key1": 9,
                "key2": 5,
                "key3": 13,
                "key4": 60,
                "key5": 12,
                "key10": 100
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(SUMEQUAL01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_MultiReceiverFunctionWrongValuesInObject_ExceptionThrown()
    {
        var schema =
            """
            %import: RelogicLabs.JSchema.Tests.External.ExternalFunctions, RelogicLabs.JSchema.Tests

            %schema:
            {
                "key1": #integer &minData,
                "key2": @minmax(&minData, &maxData) #integer,
                "key3": #integer &maxData
            }
            """;
        var json =
            """
            {
                "key1": 1,
                "key2": 11,
                "key3": 10
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(MINMAX01, exception.Code);
        Console.WriteLine(exception);
    }
}