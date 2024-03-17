namespace RelogicLabs.JsonSchema.Tests.Positive;

[TestClass]
public class PragmaTests
{
    [TestMethod]
    public void When_UndefinedPropertyInObject_ValidTrue()
    {
        var schema =
            """
            %pragma IgnoreUndefinedProperties: true
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
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_IgnorePropertyOrderFalseOfObject_ValidTrue()
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
                "key2": "value1",
                "key3": 2.1
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_IgnorePropertyOrderTrueOfObject_ValidTrue()
    {
        var schema =
            """
            %pragma IgnoreObjectPropertyOrder: true
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
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_FloatingPointToleranceOfNumber_ValidTrue()
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
                "key1": 5.000001,
                "key2": 10.000001E+0
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_DateDataTypeFormatSpecified_ValidTrue()
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
                "key1": "05-11-2023",
                "key2": "01-06-2021"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_DateTypeFormatWithBeforeAndAfter_ValidTrue()
    {
        var schema =
            """
            %pragma DateDataTypeFormat: "DD-MM-YYYY"
            %schema: 
            @after*("31-12-2009") @before*("01-01-2011") #date* #array
            """;
        var json =
            """
            [
                "01-01-2010",
                "01-02-2010",
                "30-06-2010",
                "31-12-2010"
            ]
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_TimeDataTypeFormatSpecified_ValidTrue()
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
                "key1": "05-11-2023 00:00:00",
                "key2": "01-06-2021 23:59:59"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_TimeTypeFormatWithBeforeAndAfter_ValidTrue()
    {
        var schema =
            """
            %pragma TimeDataTypeFormat: "DD-MM-YYYY hh:mm:ss"
            %schema: 
            @after*("31-12-2009 23:59:59") 
            @before*("01-01-2011 00:00:00") 
            #time* #array
            """;
        var json =
            """
            [
                "01-01-2010 00:00:00",
                "01-02-2010 00:00:01",
                "30-06-2010 12:10:30",
                "31-12-2010 23:59:59"
            ]
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_TimeTypeFormatWithRange_ValidTrue()
    {
        var schema =
            """
            %pragma TimeDataTypeFormat: "DD-MM-YYYY hh:mm:ss"
            %schema:
            @range*("01-01-2010 00:00:00", "31-12-2010 23:59:59")
            #time* #array
            """;
        var json =
            """
            [
                "01-01-2010 00:00:00",
                "01-02-2010 00:00:01",
                "30-06-2010 12:10:30",
                "31-12-2010 23:59:59"
            ]
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_TimeDataTypeOtherFormatSpecified_ValidTrue()
    {
        var schema =
            """
            %pragma TimeDataTypeFormat: "DD-MM-YYYY 'Time' hh:mm:ss"
            %schema:
            {
                "key1": #time,
                "key2": #time
            }
            """;
        var json =
            """
            {
                "key1": "05-11-2023 Time 00:00:00",
                "key2": "01-06-2021 Time 23:59:59"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_DateTypeFormatWithFullRange_ValidTrue()
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
                "01-11-2010",
                "31-12-2010"
            ]
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_DateTypeFormatWithRangeStart_ValidTrue()
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
                "01-11-2030",
                "31-12-2050"
            ]
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_DateTypeFormatWithRangeEnd_ValidTrue()
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
                "01-11-2010",
                "31-12-2010"
            ]
            """;
        JsonAssert.IsValid(schema, json);
    }
}