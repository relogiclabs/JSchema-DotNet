using RelogicLabs.JsonSchema.Exceptions;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Tests.Negative;

[TestClass]
public class PragmaTests
{
    [TestMethod]
    public void When_UndefinedPropertyOfObject_ExceptionThrown()
    {
        var schema =
            """
            %pragma IgnoreUndefinedProperties: false
            %schema:
            {
                "key1": #integer
            }
            """;
        var json =
            """
            {
                "key1": 10,
                "key2": "value1"
            }
            """;

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(PROP06, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidUndefinedPropertyValueMissing_ExceptionThrown()
    {
        var schema =
            """
            %pragma IgnoreUndefinedProperties:
            %schema:
            {
                "key1": #integer
            }
            """;
        var json =
            """
            {
                "key1": 10,
                "key2": "value1"
            }
            """;

        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<SchemaParserException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(SPRS01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_IgnoreUndefinedPropertiesMalformed_ExceptionThrown()
    {
        var schema =
            """
            %pragma IgnoreUndefinedProperty: true
            %schema:
            {
                "key1": #integer
            }
            """;
        var json =
            """
            {
                "key1": 10,
                "key2": "value1"
            }
            """;

        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<PragmaNotFoundException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(PRAG01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidUndefinedPropertyValue_ExceptionThrown()
    {
        var schema =
            """
            %pragma IgnoreUndefinedProperties: 1
            %schema:
            {
                "key1": #integer
            }
            """;
        var json =
            """
            {
                "key1": 10,
                "key2": "value1"
            }
            """;

        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<InvalidPragmaValueException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(PRAG02, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_IgnorePropertyOrderOfObject_ExceptionThrown()
    {
        var schema =
            """
            %pragma IgnoreObjectPropertyOrder: false
            %schema:
            {
                "key1": #integer,
                "key2": #string,
                "key3": #float
            }
            """;
        var json =
            """
            {
                "key1": 10,
                "key3": 2.1,
                "key2": "value1"
            }
            """;

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(PROP07, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_FloatingPointToleranceOfNumber_ExceptionThrown()
    {
        var schema =
            """
            %pragma FloatingPointTolerance: 0.00001
            %schema:
            {
                "key1": 5.00 #float,
                "key2": 10.00E+0 #double
            }
            """;
        var json =
            """
            {
                "key1": 5.00002,
                "key2": 10.0002E+0
            }
            """;

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(FLOT01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_DuplicatePragmaAssign_ExceptionThrown()
    {
        var schema =
            """
            %pragma IgnoreUndefinedProperties: false
            %pragma IgnoreUndefinedProperties: false
            
            %schema:
            {
                "key1": #integer
            }
            """;
        var json =
            """
            {
                "key1": 10,
                "key2": "value1"
            }
            """;

        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<DuplicatePragmaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(PRAG03, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidDateTypeFormat_ExceptionThrown()
    {
        var schema =
            """
            %pragma DateDataTypeFormat: "ABCD"
            %schema:
            {
                "key1": #date,
                "key2": #date
            }
            """;
        var json =
            """
            {
                "key1": "2023-11-05",
                "key2": "2021-06-01"
            }
            """;

        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<DateTimeLexerException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DLEX01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonMismatchWithDateFormat_ExceptionThrown()
    {
        var schema =
            """
            %pragma DateDataTypeFormat: "DD-MM-YYYY"
            %schema:
            {
                "key1": #date,
                "key2": #date
            }
            """;
        var json =
            """
            {
                "key1": "2023-11-05",
                "key2": "2021-06-01"
            }
            """;

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonMismatchWithTimeFormat_ExceptionThrown()
    {
        var schema =
            """
            %pragma TimeDataTypeFormat: "DD-MM-YYYY hh:mm:ss"
            %schema:
            {
                "key1": #time,
                "key2": #time
            }
            """;
        var json =
            """
            {
                "key1": "05-11-2023 12:10:30",
                "key2": "05-11-2023 23.59.59"
            }
            """;

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonDateNotValidWithBefore_ExceptionThrown()
    {
        var schema =
            """
            %pragma DateDataTypeFormat: "DD-MM-YYYY"
            %schema:
            @before*("01-01-2011") #date* #array
            """;
        var json =
            """
            [
                "01-01-2010",
                "01-02-2010",
                "31-12-2010",
                "01-01-2011"
            ]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(BFOR01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonDateNotValidWithAfter_ExceptionThrown()
    {
        var schema =
            """
            %pragma DateDataTypeFormat: "DD-MM-YYYY"
            %schema:
            @after*("31-12-2009") #date* #array
            """;
        var json =
            """
            [
                "01-01-2010",
                "01-02-2010",
                "31-12-2010",
                "31-12-2009"
            ]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(AFTR01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonTimeNotValidWithBefore_ExceptionThrown()
    {
        var schema =
            """
            %pragma TimeDataTypeFormat: "DD-MM-YYYY hh:mm:ss"
            %schema:
            @before*("01-01-2011 00:00:00") #time* #array
            """;
        var json =
            """
            [
                "01-01-1970 10:30:49",
                "01-02-2010 12:59:49",
                "31-12-2010 23:59:59",
                "01-01-2011 00:00:00"
            ]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(BFOR02, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonTimeNotValidWithAfter_ExceptionThrown()
    {
        var schema =
            """
            %pragma TimeDataTypeFormat: "DD-MM-YYYY hh:mm:ss"
            %schema:
            @after*("01-09-1939 00:00:00") #time* #array
            """;
        var json =
            """
            [
                "01-09-1939 00:00:01",
                "01-02-2010 12:59:49",
                "31-12-2030 23:59:59",
                "01-09-1939 00:00:00"
            ]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(AFTR02, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonDateNotValidWithBothRange_ExceptionThrown()
    {
        var schema =
            """
            %pragma DateDataTypeFormat: "DD-MM-YYYY"
            %schema:
            @range*("01-01-2010", "31-12-2010") #date* #array
            """;
        var json =
            """
            [
                "01-01-2010",
                "01-02-2010",
                "30-06-2010",
                "31-12-2009",
                "01-01-2011"
            ]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DRNG01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonDateNotValidWithStartRange_ExceptionThrown()
    {
        var schema =
            """
            %pragma DateDataTypeFormat: "DD-MM-YYYY"
            %schema:
            @range*("01-01-2010", !) #date* #array
            """;
        var json =
            """
            [
                "01-01-2010",
                "01-02-2010",
                "30-06-2011",
                "01-11-2050",
                "31-12-2009"
            ]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DRNG07, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonDateNotValidWithEndRange_ExceptionThrown()
    {
        var schema =
            """
            %pragma DateDataTypeFormat: "DD-MM-YYYY"
            %schema:
            @range*(!, "31-12-2010") #date* #array
            """;
        var json =
            """
            [
                "01-01-1930",
                "01-02-2000",
                "30-06-2005",
                "31-12-2010",
                "01-01-2011"
            ]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DRNG05, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonTimeNotValidWithBothRange_ExceptionThrown()
    {
        var schema =
            """
            %pragma TimeDataTypeFormat: "DD-MM-YYYY hh:mm:ss"
            %schema:
            @range*("01-01-2010 00:00:00", "31-12-2010 23:59:59") #time* #array
            """;
        var json =
            """
            [
                "01-01-2010 00:00:00",
                "01-02-2010 01:30:45",
                "30-06-2010 12:01:07",
                "31-12-2009 23:59:59",
                "01-01-2011 00:00:00"
            ]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DRNG02, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonTimeNotValidWithStartRange_ExceptionThrown()
    {
        var schema =
            """
            %pragma TimeDataTypeFormat: "DD-MM-YYYY hh:mm:ss"
            %schema:
            @range*("01-01-2010 00:00:00", !) #time* #array
            """;
        var json =
            """
            [
                "01-01-2010 00:00:00",
                "01-02-2021 01:30:45",
                "30-06-2030 12:01:07",
                "31-12-2009 23:59:59"
            ]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DRNG08, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonTimeNotValidWithEndRange_ExceptionThrown()
    {
        var schema =
            """
            %pragma TimeDataTypeFormat: "DD-MM-YYYY hh:mm:ss"
            %schema:
            @range*(!, "31-12-2010 23:59:59") #time* #array
            """;
        var json =
            """
            [
                "01-01-1930 00:00:00",
                "01-02-1990 01:30:45",
                "30-06-2000 12:01:07",
                "31-12-2010 23:59:59",
                "01-01-2011 00:00:00"
            ]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DRNG06, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_SchemaDateNotValidWithStartRange_ExceptionThrown()
    {
        var schema =
            """
            %pragma DateDataTypeFormat: "DD-MM-YYYY"
            %schema:
            @range*("2010-01-01", !) #date* #array
            """;
        var json =
            """
            [
                "01-01-2010",
                "30-06-2011",
                "01-11-2050"
            ]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DSYM01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_SchemaTimeNotValidWithEndRange_ExceptionThrown()
    {
        var schema =
            """
            %pragma TimeDataTypeFormat: "DD-MM-YYYY hh:mm:ss"
            %schema:
            @range*(!, "31-12-2010 23.59.59") #time* #array
            """;
        var json =
            """
            [
                "01-01-1930 00:00:00",
                "30-06-2000 12:01:07",
                "31-12-2010 23:59:59"
            ]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DSYM01, exception.Code);
        Console.WriteLine(exception);
    }
}