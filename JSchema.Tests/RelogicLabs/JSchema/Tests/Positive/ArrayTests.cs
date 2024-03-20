namespace RelogicLabs.JSchema.Tests.Positive;

[TestClass]
public class ArrayTests
{
    [TestMethod]
    public void When_DataTypeArray_ValidTrue()
    {
        var schema = "#array";
        var json = "[10, 20, 30]";
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_DataTypeArrayInObject_ValidTrue()
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
                "key1": [1, 10, 100],
                "key2": [100, 1000, [10, 10000]],
                "key3": [900000, 9000000, 9000000]
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_DataTypeArrayInArray_ValidTrue()
    {
        var schema =
            """
            [#array, #array, #array]
            """;
        var json =
            """
            [[], ["value1"], [0, 100, "value3", ["value4"]]]
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_NestedDataTypeArrayInArray_ValidTrue()
    {
        var schema =
            """
            #array*
            """;
        var json =
            """
            [[], [10, 100], [20000, 40000, 6000000]]
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_NestedDataTypeArrayInObject_ValidTrue()
    {
        var schema =
            """
            #array*
            """;
        var json =
            """
            { 
                "key1": [1, 10, 100],
                "key2": [100, 1000, [10, 10000]],
                "key3": [900000, 9000000, 9000000]
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_ElementsWithArray_ValidTrue()
    {
        var schema = "@elements(10, 20, 30) #array";
        var json = "[5, 10, 15, 20, 25, 30]";
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_NestedElementsWithArrayInArray_ValidTrue()
    {
        var schema = "@elements*(5, 10) #array";
        var json = "[[5, 10], [5, 10, 15], [5, 10, 15, 20]]";
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_EnumInArray_ValidTrue()
    {
        var schema =
            """
            [
                @enum(5, 10, 15), 
                @enum(100, 150, 200),
                @enum("abc", "pqr", "xyz")
            ] #array
            """;
        var json = """[10, 100, "xyz"]""";
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_FixedLengthWithArray_ValidTrue()
    {
        var schema =
            """
            @length(3) #array
            """;
        var json =
            """
            [
                "value1",
                "value2",
                "value3"
            ]
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_RangeLengthWithArray_ValidTrue()
    {
        var schema =
            """
            @length(2, 5) #array
            """;
        var json =
            """
            [
                "value1",
                "value2",
                "value3"
            ]
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_NestedLengthWithUndefinedArrayInObject_ValidTrue()
    {
        var schema =
            """
            @length*(3, !) #array*
            """;
        var json =
            """
            {
                "key1": [1, 2, 3],
                "key2": [10, 20, 30, 40],
                "key3": [100, 200, 300, 400, 500]
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_NestedLengthWithUndefinedArrayInArray_ValidTrue()
    {
        var schema =
            """
            @length*(!, 5) #array*
            """;
        var json =
            """
            [[1, 2, 3], [10, 20, 30, 40], [100, 200, 300, 400, 500]]
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_NonEmptyArrayInObject_ValidTrue()
    {
        var schema =
            """
            {
                "key1": @nonempty #array,
                "key2": @nonempty #array
            }
            """;
        var json =
            """
            {
                "key1": [10, 20, 30],
                "key2": ["val1", "val2"]
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
}