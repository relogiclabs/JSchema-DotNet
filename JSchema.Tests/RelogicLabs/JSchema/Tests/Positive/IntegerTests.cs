namespace RelogicLabs.JsonSchema.Tests.Positive;

[TestClass]
public class IntegerTests
{
    [TestMethod]
    public void When_DataTypeInteger_ValidTrue()
    {
        var schema = "#integer";
        var json = "10";
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_DataTypeIntegerInObject_ValidTrue()
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
                "key1": 0,
                "key2": 5000,
                "key3": 500000
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_DataTypeIntegerInArray_ValidTrue()
    {
        var schema =
            """
            [#integer, #integer, #integer]
            """;
        var json =
            """
            [10, -20000, 40000000]
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_NestedDataTypeIntegerInArray_ValidTrue()
    {
        var schema =
            """
            #integer*
            """;
        var json =
            """
            [0, -20000, 40000000]
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_NestedDataTypeIntegerInObject_ValidTrue()
    {
        var schema =
            """
            @range*(-50000, 15000) #integer*
            """;
        var json =
            """
            { 
                "key1": -500,
                "key2": 15000,
                "key3": -50000
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_NestedRangeIntegerInObject_ValidTrue()
    {
        var schema =
            """
            @range*(100, 200) #integer*
            """;
        var json =
            """
            {
                "key1": 100,
                "key2": 150,
                "key3": 200
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_NestedRangeWithUndefinedIntegerInObject_ValidTrue()
    {
        var schema =
            """
            @range*(100, !) #integer*
            """;
        var json =
            """
            {
                "key1": 100,
                "key2": 1500,
                "key3": 200000
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_NestedRangeWithUndefinedIntegerInArray_ValidTrue()
    {
        var schema =
            """
            @range*(!, 40000) #integer*
            """;
        var json =
            """
            [10, 2000, 40000]
            """;
        JsonAssert.IsValid(schema, json);
    }
}