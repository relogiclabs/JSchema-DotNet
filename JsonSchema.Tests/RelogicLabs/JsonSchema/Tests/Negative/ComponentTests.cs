using RelogicLabs.JsonSchema.Exceptions;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Tests.Negative;

[TestClass]
public class ComponentTests
{
    [TestMethod]
    public void When_ComponentNotDefined_ExceptionThrown()
    {
        var schema =
            """
            %schema: {
                "key1": $cmp1,
                "key2": $cmp1
            }
            """;
        var json =
            """
            {
                "key1": { "key1": 10, "key2": "value11" },
                "key2": { "key1": 20, "key2": "value22" }
            }
            """;
        
        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<DefinitionNotFoundException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DEFI02, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_ComponentWithDuplicateDefinition_ExceptionThrown()
    {
        var schema =
            """
            %define $cmp1: { "key1": #integer, "key2": #string }
            %schema: {
                "key1": $cmp1,
                "key2": $cmp1
            }
            %define $cmp1: [#string, #string]
            """;
        var json =
            """
            {
                "key1": { "key1": 10, "key2": "value11" },
                "key2": { "key1": 20, "key2": "value22" }
            }
            """;
        
        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<DuplicateDefinitionException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DEFI01, exception.ErrorCode);
        Console.WriteLine(exception);
    }
}