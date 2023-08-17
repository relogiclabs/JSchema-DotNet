namespace RelogicLabs.JsonSchema.Tests.Positive;

[TestClass]
public class StringTests
{
    [TestMethod]
    public void When_DataTypeString_ValidTrue()
    {
        var schema = "#string";
        var json = "\"value\"";
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_DataTypeWithUnicodeString_ValidTrue()
    {
        var schema = "#string";
        var json = @"""\u0985\u0986\r\n\t""";
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_DataTypeStringInObject_ValidTrue()
    {
        var schema =
            """
            { 
                "key1": #string,
                "key2": #string,
                "key3": #string
            }
            """;
        var json =
            """
            { 
                "key1": "",
                "key2": "val2",
                "key3": "value3"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_DataTypeStringInArray_ValidTrue()
    {
        var schema =
            """
            [#string, #string, #string]
            """;
        var json =
            """
            ["value1", "Lorem ipsum dolor sit amet", 
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit"]
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_NestedDataTypeStringInArray_ValidTrue()
    {
        var schema =
            """
            #string*
            """;
        var json =
            """
            ["value1", "value2", "value3"]
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_NestedDataTypeStringInObject_ValidTrue()
    {
        var schema =
            """
            #string*
            """;
        var json =
            """
            { 
                "key1": "value1",
                "key2": "value2",
                "key3": "value3"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_NestedLengthWithStringInObject_ValidTrue()
    {
        var schema =
            """
            @length*(5, 80) #string*
            """;
        var json =
            """
            {
                "key1": "value1",
                "key2": "Lorem ipsum dolor sit amet",
                "key3": "Lorem ipsum dolor sit amet, consectetur adipiscing elit"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_NestedLengthWithUnknownStringInObject_ValidTrue()
    {
        var schema =
            """
            @length*(20, !) #string*
            """;
        var json =
            """
            {
                "key1": "Lorem ipsum dolor sit amet",
                "key2": "Lorem ipsum dolor sit amet, consectetur adipiscing elit",
                "key3": "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_NestedLengthWithUnknownStringInArray_ValidTrue()
    {
        var schema =
            """
            @length*(!, 10) #string*
            """;
        var json =
            """
            ["value1", "value11", "value111"]
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_RegexWithStringInObject_ValidTrue()
    {
        var schema =
            """
            { "key1": @regex("[A-Za-z0-9]+@gmail\\.com") }
            """;
        var json =
            """
            {
                "key1": "example@gmail.com"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_EnumWithStringInObject_ValidTrue()
    {
        var schema =
            """
            {
                "key1": #string,
                "key2": @enum("val1", "val2") #string
            }
            """;
        var json =
            """
            {
                "key1": "",
                "key2": "val2"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_EmailWithStringInObject_ValidTrue()
    {
        var schema =
            """
            {
                "key1": @email #string,
                "key2": @email #string
            }
            """;
        var json =
            """
            {
                "key1": "email.test@example.com",
                "key2": "Email_Test@example.org"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_UrlWithStringInObject_ValidTrue()
    {
        var schema =
            """
            {
                "key1": @url #string,
                "key2": @url #string,
                "key3": @url("ftps") #string
            }
            """;
        var json =
            """
            {
                "key1": "https://www.google.com/",
                "key2": "https://www.microsoft.com/en-us/",
                "key3": "ftps://subdomain.example.com/test#section?query=string"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_PhoneWithStringInObject_ValidTrue()
    {
        var schema =
            """
            {
                "key1": @phone #string,
                "key2": @phone #string,
                "key3": @phone #string
            }
            """;
        var json =
            """
            {
                "key1": "01737048177",
                "key2": "+8801737048177",
                "key3": "008801737048177"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
}