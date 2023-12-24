namespace RelogicLabs.JsonSchema.Tests.Positive;

[TestClass]
public class OtherTests
{
    [TestMethod]
    public void When_DataTypeFloat_ValidTrue()
    {
        var schema = "#float";
        var json = "2.5";
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_DataTypeDouble_ValidTrue()
    {
        var schema = "#double";
        var json = "2.5E1";
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_DataTypeNull_ValidTrue()
    {
        var schema = "null #null";
        var json = "null";
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_DataTypeUndefined_ValidTrue()
    {
        var schema = "!";
        var json = "1000";
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_NonStaticValidMethod_ValidTrue() {
        var schema =
            """
            %version: 2023.09.11
            %include: RelogicLabs.JsonSchema.Tests.External.ExternalFunctions,
                      RelogicLabs.JsonSchema.Tests
            %pragma IgnoreUndefinedProperties: true
            %define $element: @range(1, 100) #integer
            %schema: {
                "key1": #integer*($element) #array,
                "key2": #array,
                "key3": @even #integer
            }
            """;
        var json1 =
            """
            {
                "key1": [1, 10, 100],
                "key2": [100, 1000, [10, 10000]],
                "key3": 10
            }
            """;
        var json2 =
            """
            {
                "key1": [10, 20, 50, 90, 100],
                "key2": ["test", 1000, [10.7, 10000]],
                "key3": 102
            }
            """;
        var jsonAssert = new JsonAssert(schema);
        jsonAssert.IsValid(json1);
        jsonAssert.IsValid(json2);
    }
}