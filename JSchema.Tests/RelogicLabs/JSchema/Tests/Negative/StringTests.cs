using RelogicLabs.JsonSchema.Exceptions;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Tests.Negative;

[TestClass]
public class StringTests
{
    [TestMethod]
    public void When_JsonNotString_ExceptionThrown()
    {
        var schema = "#string";
        var json = "10";

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidUnicodeStringInSchema_ExceptionThrown()
    {
        var schema = @"""\uX0485\uY486\r\n\t #string""";
        var json = @"""\u0485\u0486\r\n\t""";

        var exception = Assert.ThrowsException<SchemaLexerException>(
             () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(SLEX01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidUnicodeStringInJson_ExceptionThrown()
    {
        var schema = @"""\u0485\u0486\r\n\t #string""";
        var json = @"""\uX0485\uY486\r\n\t""";

        var exception = Assert.ThrowsException<JsonLexerException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(JLEX01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonNotStringInObject_ExceptionThrown()
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
                "key2": 10,
                "key3": null
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonNotStringInArray_ExceptionThrown()
    {
        var schema =
            """
            [#string, #string, #string]
            """;
        var json =
            """
            ["value1", null,
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit"]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_NestedJsonNotStringInArray_ExceptionThrown()
    {
        var schema =
            """
            #string*
            """;
        var json =
            """
            ["value1", 10, "value3"]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP06, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_NestedJsonNotStringInObject_ExceptionThrown()
    {
        var schema =
            """
            #string*
            """;
        var json =
            """
            {
                "key1": "value1",
                "key2": null,
                "key3": 100.5
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP06, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_WrongLengthWithStringInObject_ExceptionThrown()
    {
        var schema =
            """
            @length*(5) #string*
            """;
        var json =
            """
            {
                "key1": "12345",
                "key2": "1234",
                "key3": "123456"
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(SLEN01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_NestedWrongLengthWithStringInObject_ExceptionThrown()
    {
        var schema =
            """
            @length*(5, 15) #string*
            """;
        var json =
            """
            {
                "key1": "value1",
                "key2": "Lorem ipsum dolor sit amet",
                "key3": "Lorem ipsum dolor sit amet, consectetur adipiscing elit"
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(SLEN03, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_NestedWrongLengthWithUndefinedStringInObject_ExceptionThrown()
    {
        var schema =
            """
            @length*(100, !) #string*
            """;
        var json =
            """
            {
                "key1": "Lorem ipsum dolor sit amet",
                "key2": "Lorem ipsum dolor sit amet, consectetur adipiscing elit",
                "key3": "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua"
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(SLEN04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_NestedWrongLengthWithUndefinedStringInArray_ExceptionThrown()
    {
        var schema =
            """
            @length*(!, 10) #string*
            """;
        var json =
            """
            ["Lorem ipsum dolor sit amet", "value11", "value111"]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(SLEN05, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_RegexWithWrongStringInObject_ExceptionThrown()
    {
        var schema =
            """
            { "key1": @regex("[A-Za-z0-9]+@gmail\\.com") }
            """;
        var json =
            """
            {
                "key1": "new example@gmail.com"
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(REGX01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_EnumWithWrongStringInObject_ExceptionThrown()
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
                "key2": "val4"
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(ENUM01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_EmailWithWrongStringInObject_ExceptionThrown()
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
                "key1": "email@test@example.com",
                "key2": "Email_Test@example.org"
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(EMAL01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_UrlWithWrongStringAddressInObject_ExceptionThrown()
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
                "key1": "https:// www.example.com/",
                "key2": "https://www.<example>.com/test/",
                "key3": "ftps://subdomain.`example`.com/test#section?query=string"
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(URLA01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_UrlWithSchemeAndWrongStringAddressInObject_ExceptionThrown()
    {
        var schema =
            """
            {
                "key1": @url("ftps") #string,
                "key2": @url #string
            }
            """;
        var json =
            """
            {
                "key1": "ssh://www.example.com/test/",
                "key2": "ftp://www.example.com/"
            }
            """;

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(URLA04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_PhoneWithWrongStringInObject_ExceptionThrown()
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
                "key1": "Phone 01737048177",
                "key2": "+880:1737048177",
                "key3": "0088/01737048177"
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(PHON01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_EmptyStringInObject_ExceptionThrown()
    {
        var schema =
            """
            {
                "key1": @nonempty #string,
                "key2": @nonempty #string #null
            }
            """;
        var json =
            """
            {
                "key1": "",
                "key2": null
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(NEMT01, exception.Code);
        Console.WriteLine(exception);
    }
}