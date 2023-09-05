namespace RelogicLabs.JsonSchema.Tests.Positive;

[TestClass]
public class ObjectTests
{
    [TestMethod]
    public void When_DataTypeObject_ValidTrue()
    {
        var schema = "#object";
        var json = """{"key": "value"}""";
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_DataTypeObjectInObject_ValidTrue()
    {
        var schema =
            """
            { 
                "key1": #object,
                "key2": #object,
                "key3": #object
            }
            """;
        var json =
            """
            { 
                "key1": {},
                "key2": { "key21": 1 },
                "key3": { "key31": 2, "key32": 2.5, "key33": "value31" }
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_DataTypeObjectInArray_ValidTrue()
    {
        var schema =
            """
            [#object, #object, #object]
            """;
        var json =
            """
            [{}, 
            { "key1": "value1" }, 
            {
                "key1": "value1",
                "key2": [10, 20],
                "key3": { "key": 10 }
            }]
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_NestedDataTypeObjectInArray_ValidTrue()
    {
        var schema =
            """
            #object*
            """;
        var json =
            """
            [ {"key1": "value1"}, {"key2": [10, 20]}, {"key3": { "key": 10 }} ]
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_NestedDataTypeObjectInObject_ValidTrue()
    {
        var schema =
            """
            #object*
            """;
        var json =
            """
            { 
                "key1": {"key11": "value11"},
                "key2": {"key21": [10, 20]},
                "key3": {"key3": { "key": 10 }}
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_KeysWithObject_ValidTrue()
    {
        var schema =
            """
            @keys("key1", "key3") #integer*
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
    public void When_ValuesWithObject_ValidTrue()
    {
        var schema =
            """
            @values(100, 200) #integer*
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
    public void When_NestedKeysWithObjectInObject_ValidTrue()
    {
        var schema =
            """
            @keys*("key") #object*
            """;
        var json =
            """
            {
                "key1": {"key": 10},
                "key2": {"key": 150},
                "key3": {"key": 1000}
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_NestedKeysAndValuesWithObjectInArray_ValidTrue()
    {
        var schema =
            """
            @keys*("key") @values*(100) #object*
            """;
        var json =
            """
            [{"key": 100}, {"key": 100}, {"key": 100}]
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_EnumWithObject_ValidTrue()
    {
        var schema = 
            """
            {
            "key1": @enum(5, 10, 15),
            "key2": @enum(100, 150, 200),
            "key3": @enum("abc", "pqr", "xyz")
            } #object
            """;
        var json = 
            """
            {
            "key1": 10, 
            "key2": 100, 
            "key3": "xyz"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_NonEmptyObjectInArray_ValidTrue()
    {
        var schema =
            """
            [
                @nonempty #object,
                @nonempty #object
            ]
            """;
        var json =
            """
            [
                { "key1": 10 },
                { "key2": ["val1", "val2"] }
            ]
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_LengthOfObjectInArray_ValidTrue()
    {
        var schema =
            """
            [
                @length(1) #object,
                @length(1, 5) #object,
                @length(1, 5) #object,
                @length(!, 5) #object,
                @length(2, !) #object
            ]
            """;
        var json =
            """
            [
                { "key1": 10 },
                { "key1": 10 },
                { "key1": 10, "key2": 20, "key3": 30, "key4": 40, "key5": 50 },
                { },
                { "key1": 10, "key2": 20, "key3": 30 },
                { "key1": 10, "key2": 20, "key3": 30 }
            ]
            """;
        JsonAssert.IsValid(schema, json);
    }
}