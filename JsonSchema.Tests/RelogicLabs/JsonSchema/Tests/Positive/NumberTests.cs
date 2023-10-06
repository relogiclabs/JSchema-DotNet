namespace RelogicLabs.JsonSchema.Tests.Positive;

[TestClass]
public class NumberTests
{
    [TestMethod]
    public void When_MinimumFunctionWithInteger_ValidTrue()
    {
        var schema =
            """
            @minimum(10) #integer
            """;
        var json =
            """
            20
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_MaximumFunctionWithFloat_ValidTrue()
    {
        var schema =
            """
            @maximum(10.5) #float
            """;
        var json =
            """
            10.5
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_NestedMinimumIntegerInArray_ValidTrue()
    {
        var schema =
            """
            @minimum*(10.5) #integer*
            """;
        var json =
            """
            [
                11,
                100,
                500000
            ]
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_NestedMinimumFloatInObject_ValidTrue()
    {
        var schema =
            """
            @minimum*(100) #number*
            """;
        var json =
            """
            {
                "key1": 100.000,
                "key2": 150,
                "key3": 200.884
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_NestedMaximumNumberInArray_ValidTrue()
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
                10
            ]
            """;
        JsonAssert.IsValid(schema, json);
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
                "key3": 10.884
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_NestedMinimumMaximumFloatInObject_ValidTrue()
    {
        var schema =
            """
            @minimum*(-150.407) @maximum*(100.476) #float*
            """;
        var json =
            """
            {
                "key1": 100.476,
                "key2": -150.407,
                "key3": 10.884
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_NestedMinimumMaximumExclusiveFloatInObject_ValidTrue()
    {
        var schema =
            """
            @minimum*(-150, true) @maximum*(100, true) #float*
            """;
        var json =
            """
            {
                "key1": 99.999,
                "key2": -149.999,
                "key3": 10.884
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
}