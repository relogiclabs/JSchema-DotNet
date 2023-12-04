namespace RelogicLabs.JsonSchema.Tests.Positive;

[TestClass]
public class ReceiverTests
{
    [TestMethod]
    public void When_ReceiveSingleValueInObject_ValidTrue()
    {
        var schema =
            """
            %include: RelogicLabs.JsonSchema.Tests.Positive.ExternalFunctions,
                      RelogicLabs.JsonSchema.Tests
            %schema:
            {
                "key1": #integer &dependent,
                "key2": @condition(&dependent) #integer
            }
            """;
        var json1 =
            """
            {
                "key1": 5,
                "key2": 6
            }
            """;
        var json2 =
            """
            {
                "key1": 100,
                "key2": 102
            }
            """;
        var jsonAssert = new JsonAssert(schema);
        jsonAssert.IsValid(json1);
        jsonAssert.IsValid(json2);
    }

    [TestMethod]
    public void When_MultiReceiversValueInObject_ValidTrue()
    {
        var schema =
            """
            %include: RelogicLabs.JsonSchema.Tests.Positive.ExternalFunctions,
                      RelogicLabs.JsonSchema.Tests
            %schema:
            {
                "key1": #integer &receiver1 &receiver2 &receiver3,
                "key2": @condition(&receiver3) #integer
            }
            """;
        var json =
            """
            {
                "key1": 6,
                "key2": 8
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_ReceiveArrayValuesInObject_ValidTrue()
    {
        var schema =
            """
            %include: RelogicLabs.JsonSchema.Tests.Positive.ExternalFunctions,
                      RelogicLabs.JsonSchema.Tests
            
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
                "key2": 7
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_ReceiveMultipleValuesInObject_ValidTrue()
    {
        var schema =
            """
            %include: RelogicLabs.JsonSchema.Tests.Positive.ExternalFunctions,
                      RelogicLabs.JsonSchema.Tests

            %schema:
            {
                "key1": #integer &relatedData,
                "key10": @sumEqual(&relatedData) #integer,
                "key2": #integer &relatedData,
                "key3": #integer &relatedData,
                "key4": #integer &relatedData,
                "key5": #integer &relatedData
            }
            """;
        var json =
            """
            {
                "key1": 10,
                "key2": 5,
                "key3": 13,
                "key10": 100,
                "key4": 60,
                "key5": 12
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_MultiReceiversFunctionInObject_ValidTrue()
    {
        var schema =
            """
            %include: RelogicLabs.JsonSchema.Tests.Positive.ExternalFunctions,
                      RelogicLabs.JsonSchema.Tests

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
                "key2": 9,
                "key3": 10
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
}