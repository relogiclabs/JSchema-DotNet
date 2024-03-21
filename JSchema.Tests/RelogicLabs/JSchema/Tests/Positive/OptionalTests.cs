namespace RelogicLabs.JSchema.Tests.Positive;

[TestClass]
public class OptionalTests
{
    [TestMethod]
    public void When_OptionalInArray_ValidTrue()
    {
        var schema = "[#integer?]";
        var json = "[]";
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_OptionalInObject_ValidTrue()
    {
        var schema = """{"key1": #string ?}""";
        var json = "{}";
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_OptionalWithMandatoryInArray_ValidTrue()
    {
        var schema = "[@range(1, 10) #number, @range(10, 100) #number?, #number?]";
        var json = "[8.5, 80]";
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_OptionalWithFunctionInArray_ValidTrue()
    {
        var schema =
            """
            [
                @range(10, 11) #number, 
                @negative #number?, 
                @range(100, 500) #number?
            ]
            """;
        var json = "[10.5, -200]";
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_OptionalWithFunctionsInObject_ValidTrue()
    {
        var schema =
            """
            {
                "key1": @positive #number,
                "key2": @length(6) #string?,
                "key3": @negative #number?
            }
            """;
        var json =
            """
            {
                "key1": 0.11,
                "key2": "value2"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
}