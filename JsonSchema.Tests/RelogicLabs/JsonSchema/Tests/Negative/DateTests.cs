using RelogicLabs.JsonSchema.Exceptions;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Tests.Negative;

[TestClass]
public class DateTests
{
    [TestMethod]
    public void When_JsonNotDate_ExceptionThrown()
    {
        var schema = "#date";
        var json = "\"This is a string\"";
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP02, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_DateFormatIncorrect_ExceptionThrown()
    {
        var schema = """ @date("DD-MM-YY") """;
        var json = """ "99-09-01" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DDAY04, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_DayOutOfRange_ExceptionThrown()
    {
        var schema = """ @date("DD-MM-YYYY") """;
        var json = """ "29-02-1939" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DDAY03, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_DayOutOfRange2_ExceptionThrown()
    {
        var schema = """ @date("DD-MM-YYYY") """;
        var json = """ "32-12-1939" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DDAY04, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidMonthFullNameFormat_ExceptionThrown()
    {
        var schema = """ @date("MMMM DD, YYYY G") """;
        var json = """ "Septembar 01, 1939 AD" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DMON01, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidMonthShortNameFormat_ExceptionThrown()
    {
        var schema = """ @date("MMM DD, YYYY G") """;
        var json = """ "Sap 01, 1939 AD" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DMON02, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidMonthNumberFormat_ExceptionThrown()
    {
        var schema = """ @date("MM-DD, YYYY G") """;
        var json = """ "Sep-01, 1939 AD" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DMON03, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidMonthNumberRange_ExceptionThrown()
    {
        var schema = """ @date("MM-DD, YYYY G") """;
        var json = """ "13-01, 1939 AD" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DMON05, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidWeekdayFormat_ExceptionThrown()
    {
        var schema = """ @date("DDD, MMM DD, YYYY G") """;
        var json = """ "Fry, Sep 01, 1939 AD" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DWKD02, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_ConflictingInfoInInput_ExceptionThrown()
    {
        var schema = """ @date("MMMM, DD-MM-YYYY") """;
        var json = """ "January, 01-12-1939" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DCNF01, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidWeekday_ExceptionThrown()
    {
        var schema = """ @date("DDD, MMM DD, YYYY G") """;
        var json = """ "Sat, Sep 01, 1939 AD" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DWKD03, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidYearFormat_ExceptionThrown()
    {
        var schema = """ @date("DD-MM-YY") """;
        var json = """ "01-09-Twenty" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DYAR02, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidYearFormat2_ExceptionThrown()
    {
        var schema = """ @date("DD-MM-YYYY") """;
        var json = """ "01-09-0000" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DYAR03, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidYearFormat3_ExceptionThrown()
    {
        var schema = """ @date("DD-MM-YY") """;
        var json = """ "01-09-1939" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DINV02, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidEraFormat_ExceptionThrown()
    {
        var schema = """ @date("DD-MM-YYYY G") """;
        var json = """ "02-12-1939 AA" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DERA01, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidDateTextMissing_ExceptionThrown()
    {
        var schema = """ @date("DD-MM-YYYY 'Time' hh:mm:ss") """;
        var json = """ "01-11-1939 10:00:00" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTXT01, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidHourFormat_ExceptionThrown()
    {
        var schema = """ @date("hh:mm:ss") """;
        var json = """ "Twelve:00:00" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DHUR01, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidHourRange_ExceptionThrown()
    {
        var schema = """ @date("hh:mm:ss") """;
        var json = """ "24:00:00" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DHUR06, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidMinuteFormat_ExceptionThrown()
    {
        var schema = """ @date("hh:mm:ss") """;
        var json = """ "23:one:00" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DMIN01, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidMinuteRange_ExceptionThrown()
    {
        var schema = """ @date("hh:mm:ss") """;
        var json = """ "23:60:00" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DMIN03, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidSecondFormat_ExceptionThrown()
    {
        var schema = """ @date("hh:mm:ss") """;
        var json = """ "23:59:Three" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DSEC01, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidSecondRange_ExceptionThrown()
    {
        var schema = """ @date("hh:mm:ss") """;
        var json = """ "23:59:60" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DSEC03, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidSecondFractionFormat_ExceptionThrown()
    {
        var schema = """ @date("hh:mm:ss.fff") """;
        var json = """ "23:59:00.11" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DFRC04, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidHourFormatWithExtra0_ExceptionThrown()
    {
        var schema = """ @date("h:m:s") """;
        var json = """ "01:3:8" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DSYM01, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidTimeFormat_ExceptionThrown()
    {
        var schema = """ @date("hh mm ss") """;
        var json = """ "01:10:08" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DWTS01, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidAmPmFormat_ExceptionThrown()
    {
        var schema = """ @date("hh:mm:ss t") """;
        var json = """ "12:00:00 AD" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTAP01, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_Invalid12HourFormat_ExceptionThrown()
    {
        var schema = """ @date("hh:mm:ss t") """;
        var json = """ "13:00:00 AM" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DHUR03, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidUTCOffsetFormat_ExceptionThrown()
    {
        var schema = """ @date("hh:mm:ss Z") """;
        var json = """ "11:00:00 Six" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DUTC01, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidUTCOffsetHourRange_ExceptionThrown()
    {
        var schema = """ @date("hh:mm:ss Z") """;
        var json = """ "11:00:00 +14" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DUTC04, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidUTCOffsetMinuteRange_ExceptionThrown()
    {
        var schema = """ @date("hh:mm:ss ZZ") """;
        var json = """ "11:00:00 +10:60" """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DUTC05, exception.ErrorCode);
        Console.WriteLine(exception);
    }
    
    [TestMethod]
    public void When_InvalidPatternCauseLexerError_ExceptionThrown()
    {
        var schema = """ @date("ABCD") """;
        var json = "\"23-09-01T14:35:10.555\"";
        
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DLEX01, exception.ErrorCode);
        Console.WriteLine(exception);
    }
}