using RelogicLabs.JsonSchema.Exceptions;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Tests.Negative;

[TestClass]
public class DateTimeTests
{
    [TestMethod]
    public void When_JsonNotDate_ExceptionThrown()
    {
        var schema = "#date";
        var json = "\"This is not a date\"";

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonNotValidDate_ExceptionThrown()
    {
        var schema = "#date";
        var json = "\"1939-02-29\"";

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonNotTime_ExceptionThrown()
    {
        var schema = "#time";
        var json = "\"This is not a time\"";

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonNotValidTime_ExceptionThrown()
    {
        var schema = "#time";
        var json = "\"1939-09-02T2:12:12.000Z\"";

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_DateInputWrong_ExceptionThrown()
    {
        var schema = """ @date("DD-MM-YY") """;
        var json = """ "99-09-01" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DDAY04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_TimeInputWrong_ExceptionThrown()
    {
        var schema = """ @time("hh:mm:ss t") """;
        var json = """ "13:10:10 PM" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DHUR03, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_DateDayOutOfRange_ExceptionThrown()
    {
        var schema = """ @date("DD-MM-YYYY") """;
        var json = """ "29-02-1939" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DDAY03, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_DateDayOutOfRange2_ExceptionThrown()
    {
        var schema = """ @date("DD-MM-YYYY") """;
        var json = """ "32-12-1939" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DDAY04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidDateMonthFullName_ExceptionThrown()
    {
        var schema = """ @date("MMMM DD, YYYY G") """;
        var json = """ "Septembar 01, 1939 AD" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DMON01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidDateMonthShortName_ExceptionThrown()
    {
        var schema = """ @date("MMM DD, YYYY G") """;
        var json = """ "Sap 01, 1939 AD" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DMON02, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidDateMonthNumber_ExceptionThrown()
    {
        var schema = """ @date("MM-DD, YYYY G") """;
        var json = """ "Sep-01, 1939 AD" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DMON03, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidDateMonthNumberRange_ExceptionThrown()
    {
        var schema = """ @date("MM-DD, YYYY G") """;
        var json = """ "13-01, 1939 AD" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DMON05, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidDateWeekdayInput_ExceptionThrown()
    {
        var schema = """ @date("DDD, MMM DD, YYYY G") """;
        var json = """ "Fry, Sep 01, 1939 AD" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DWKD02, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_ConflictingDateInfoInInput_ExceptionThrown()
    {
        var schema = """ @date("MMMM, DD-MM-YYYY") """;
        var json = """ "January, 01-12-1939" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DCNF01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_ConflictingTimeInfoInInput_ExceptionThrown()
    {
        var schema = """ @time("hh, hh:mm:ss") """;
        var json = """ "12, 11:10:12" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DCNF01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidDateWeekday_ExceptionThrown()
    {
        var schema = """ @date("DDD, MMM DD, YYYY G") """;
        var json = """ "Sat, Sep 01, 1939 AD" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DWKD03, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidDateYearInput_ExceptionThrown()
    {
        var schema = """ @date("DD-MM-YY") """;
        var json = """ "01-09-Twenty" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DYAR02, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidDateYearInput2_ExceptionThrown()
    {
        var schema = """ @date("DD-MM-YYYY") """;
        var json = """ "01-09-0000" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DYAR03, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidDateYearInput3_ExceptionThrown()
    {
        var schema = """ @date("DD-MM-YY") """;
        var json = """ "01-09-1939" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DINV02, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidDateEraInput_ExceptionThrown()
    {
        var schema = """ @date("DD-MM-YYYY G") """;
        var json = """ "02-12-1939 AA" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DERA01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidTimeTextMissing_ExceptionThrown()
    {
        var schema = """ @time("DD-MM-YYYY 'Time' hh:mm:ss") """;
        var json = """ "01-11-1939 10:00:00" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTXT01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidTimeHourInput_ExceptionThrown()
    {
        var schema = """ @time("hh:mm:ss") """;
        var json = """ "Twelve:00:00" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DHUR01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidTimeHourRange_ExceptionThrown()
    {
        var schema = """ @time("hh:mm:ss") """;
        var json = """ "24:00:00" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DHUR06, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidTimeMinuteInput_ExceptionThrown()
    {
        var schema = """ @time("hh:mm:ss") """;
        var json = """ "23:one:00" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DMIN01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidTimeMinuteRange_ExceptionThrown()
    {
        var schema = """ @time("hh:mm:ss") """;
        var json = """ "23:60:00" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DMIN03, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidTimeSecondInput_ExceptionThrown()
    {
        var schema = """ @time("hh:mm:ss") """;
        var json = """ "23:59:Three" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DSEC01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidTimeSecondRange_ExceptionThrown()
    {
        var schema = """ @time("hh:mm:ss") """;
        var json = """ "23:59:60" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DSEC03, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidTimeSecondFraction_ExceptionThrown()
    {
        var schema = """ @time("hh:mm:ss.fff") """;
        var json = """ "23:59:00.11" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DFRC04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidTimeNoHourInput_ExceptionThrown()
    {
        var schema = """ @time("h:m:s") """;
        var json = """ ":3:8" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DHUR02, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidTimeInput_ExceptionThrown()
    {
        var schema = """ @date("hh mm ss") """;
        var json = """ "01:10:08" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DWTS01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidTimeAmPmInput_ExceptionThrown()
    {
        var schema = """ @time("hh:mm:ss t") """;
        var json = """ "12:00:00 AD" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTAP01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidTime12HourInput_ExceptionThrown()
    {
        var schema = """ @time("hh:mm:ss t") """;
        var json = """ "13:00:00 AM" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DHUR03, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidTimeAmPmMissing_ExceptionThrown()
    {
        var schema = """ @time("hh:mm:sst") """;
        var json = """ "11:11:11" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTAP01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidTimeUTCOffsetInput_ExceptionThrown()
    {
        var schema = """ @time("hh:mm:ss Z") """;
        var json = """ "11:00:00 Six" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DUTC01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidTimeUTCOffsetHourRange_ExceptionThrown()
    {
        var schema = """ @time("hh:mm:ss Z") """;
        var json = """ "11:00:00 +14" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DUTC04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidTimeUTCOffsetMinuteRange_ExceptionThrown()
    {
        var schema = """ @time("hh:mm:ss ZZ") """;
        var json = """ "11:00:00 +10:60" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DUTC05, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidDatePatternCauseLexerError_ExceptionThrown()
    {
        var schema = """ @date("ABCD") """;
        var json = "\"23-09-01T14:35:10.555\"";

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DLEX01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidTimePatternCauseLexerError_ExceptionThrown()
    {
        var schema = """ @time("ABCD") """;
        var json = "\"23-09-01T14:35:10.555\"";

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DLEX01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonDateNotValidWithBothRange_ExceptionThrown()
    {
        var schema =
            """
            @range*("2010-01-01", "2010-12-31") #date* #array
            """;
        var json =
            """
            [
                "2010-01-01",
                "2010-02-01",
                "2010-06-30",
                "2009-12-31",
                "2011-01-01"
            ]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DRNG01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonTimeNotValidWithBothRange_ExceptionThrown()
    {
        var schema =
            """
            @range*("2010-01-01T00:00:00.000Z", 
                    "2010-12-31T23:59:59.999Z") 
            #time* #array
            """;
        var json =
            """
            [
                "2010-01-01T00:00:00.000Z",
                "2010-02-01T01:30:45.1Z",
                "2010-06-30T12:01:07.999999Z",
                "2009-12-31T22:39:50.0-04:00",
                "2011-01-01T02:10:00.0+06:00",
                "2009-12-31T22:59:59.000Z",
                "2011-01-01T00:00:00.000Z"
            ]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DRNG02, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonDateNotValidWithStart_ExceptionThrown()
    {
        var schema =
            """
            @start*("2010-01-01") #date* #array
            """;
        var json =
            """
            [
                "2010-01-01",
                "2010-02-01",
                "2011-06-30",
                "2050-11-01",
                "2009-12-31"
            ]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(STRT01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonDateNotValidWithEnd_ExceptionThrown()
    {
        var schema =
            """
            @end*("2010-12-31") #date* #array
            """;
        var json =
            """
            [
                "1930-01-01",
                "2000-02-01",
                "2005-06-30",
                "2010-12-31",
                "2011-01-01"
            ]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(ENDE01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonDateNotValidWithBefore_ExceptionThrown()
    {
        var schema =
            """
            @before*("2011-01-01") #date* #array
            """;
        var json =
            """
            [
                "2010-01-01",
                "2010-06-30",
                "2010-12-31",
                "2011-01-01",
                "2023-11-15"
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
            @after*("2009-12-31") #date* #array
            """;
        var json =
            """
            [
                "2010-01-01",
                "2010-02-10",
                "2010-12-31",
                "2009-12-31",
                "1980-11-19"
            ]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(AFTR01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_JsonTimeNotValidWithEndInFraction_ExceptionThrown()
    {
        var schema =
            """
            @end*("1939-09-02T02:12:12.555Z") #time* #array
            """;
        var json =
            """
            [
                "1939-09-02T02:12:12.554Z",
                "1939-09-02T02:12:12.555Z",
                "1939-09-02T02:12:12.556Z"
            ]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(ENDE02, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_SchemaDateNotValidWithBefore_ExceptionThrown()
    {
        var schema =
            """
            @before*("01-01-2011") #date* #array
            """;
        var json =
            """
            [
                "1900-01-01",
                "2010-06-30",
                "2010-12-31"
            ]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DYAR01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_SchemaDateNotValidWithAfter_ExceptionThrown()
    {
        var schema =
            """
            @after*("12-31-2009") #date* #array
            """;
        var json =
            """
            [
                "2010-01-01",
                "2010-02-10",
                "2050-12-31"
            ]
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DYAR01, exception.Code);
        Console.WriteLine(exception);
    }
}