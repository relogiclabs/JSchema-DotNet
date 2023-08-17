namespace RelogicLabs.JsonSchema.Tests.Positive;

[TestClass]
public class FunctionTests
{
    
    [TestMethod]
    public void When_ExternalFunctionExecute_ValidTrue()
    {
        var schema =
            """
            %include: RelogicLabs.JsonSchema.Tests.Positive.ExternalFunctions,
                    RelogicLabs.JsonSchema.Tests
            %schema: @even #integer
            """;
        var json = "10";
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_ExternalFunctionWithoutDataType_ValidTrue()
    {
        var schema =
            """
            %include: RelogicLabs.JsonSchema.Tests.Positive.ExternalFunctions,
                    RelogicLabs.JsonSchema.Tests
            %schema: @even
            """;
        var json = "10";
        
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_ExternalFunctionInObject_ValidTrue()
    {
        var schema =
            """
            %include: RelogicLabs.JsonSchema.Tests.Positive.ExternalFunctions, 
                    RelogicLabs.JsonSchema.Tests
            %schema:
            {
                "key1": @even #integer,
                "key2": @even #integer
            }
            """;
        var json =
            """
            {
                "key1": 10,
                "key2": 12
            }
            """;
        
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_ExternalFunctionInArray_ValidTrue()
    {
        var schema =
            """
            %include: RelogicLabs.JsonSchema.Tests.Positive.ExternalFunctions,
                    RelogicLabs.JsonSchema.Tests
            %schema: [
                @even #integer,
                @even #integer
            ]
            """;
        var json = "[100, 200]";
        
        JsonAssert.IsValid(schema, json);
    }
}