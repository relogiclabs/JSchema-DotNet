namespace RelogicLabs.JsonSchema.Tests.Positive;

[TestClass]
public class ComponentTests
{
    [TestMethod]
    public void When_ComponentObjectInObject_ValidTrue()
    {
        var schema =
            """
            %define $cmp1: { "key1": #integer, "key2": #string }
            %schema: {
                "key1": $cmp1,
                "key2": $cmp1,
                "key3": $cmp1
            }
            """;
        var json =
            """
            {
                "key1": { "key1": 10, "key2": "value11" },
                "key2": { "key1": 20, "key2": "value22" },
                "key3": { "key1": 30, "key2": "value33" }
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_ComponentObjectInArray_ValidTrue()
    {
        var schema =
            """
            %define $cmp1: { "key1": #integer, "key2": #float }
            %schema: [$cmp1, $cmp1, $cmp1]
            """;
        var json =
            """
            [
                { "key1": 10, "key2": 2.5 },
                { "key1": 20, "key2": 3.5 },
                { "key1": 30, "key2": 4.5 }
            ]
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_ComponentArrayInArray_ValidTrue()
    {
        var schema =
            """
            %define $cmp1: [#number, #number]
            %schema: [$cmp1, $cmp1, $cmp1]
            """;
        var json =
            """
            [
                [10, 2.5],
                [20, 3.5],
                [30, 4.5]
            ]
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_ComponentArrayInObject_ValidTrue()
    {
        var schema =
            """
            %define $cmp1: [#number, #number]
            %schema: {
                "key1": $cmp1,
                "key2": $cmp1,
                "key3": $cmp1
            }
            """;
        var json =
            """
            {
                "key1": [10, 2.5],
                "key2": [20, 3.5],
                "key3": [30, 4.5]
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_RangeWithComponentObjectInArray_ValidTrue()
    {
        var schema =
            """
            %define $cmp1: { "key1": @range(10, 30) #integer, "key2": @range(2.5, 4.5) #float }
            %schema: [$cmp1, $cmp1, $cmp1]
            """;
        var json =
            """
            [
                { "key1": 10, "key2": 2.5 },
                { "key1": 20, "key2": 3.5 },
                { "key1": 30, "key2": 4.5 }
            ]
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_NestedComponentObjectWithObjectInArray_ValidTrue()
    {
        var schema =
            """
            %define $cmp1: 
            { 
                "key1": @range(10, 30) #integer, 
                "key2": @enum("val1", "val2", "val3") #string 
            }
            %schema: #object*($cmp1) #array
            """;
        var json =
            """
            [
                { "key1": 10, "key2": "val1" },
                { "key1": 20, "key2": "val2" },
                { "key1": 30, "key2": "val3" }
            ]
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_ComponentInComponentWithArrayInObject_ValidTrue()
    {
        var schema =
            """
            %define $cmp1: {
                "key": @range(10, 100) #number
            }
            %define $cmp2: [ $cmp1, $cmp1 ]
            %schema: {
                "key1": $cmp2,
                "key2": $cmp2,
                "key3": $cmp2
            }
            """;
        var json =
            """
            {
                "key1": [{"key": 10}, {"key": 11}],
                "key2": [{"key": 20}, {"key": 21}],
                "key3": [{"key": 30}, {"key": 31}]
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_CompositeAndPrimitiveDataTypeWithPrimitiveInput_ValidTrue()
    {
        var schema = "@range*(5, 10) #array #string";
        var json = 
            """
            "value1"
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_CompositeAndPrimitiveDataTypeWithCompositeInput_ValidTrue()
    {
        var schema = "@range*(5, 10) #array #string";
        var json = 
            """
            [5, 6, 7]
            """;
        JsonAssert.IsValid(schema, json);
    }
}