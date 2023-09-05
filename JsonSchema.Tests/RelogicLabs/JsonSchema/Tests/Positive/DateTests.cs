namespace RelogicLabs.JsonSchema.Tests.Positive;

[TestClass]
public class DateTests
{
    
    [TestMethod]
    public void When_DataTypeDate_ValidTrue()
    {
        var schema = "#date";
        var json = "\"2023-09-01T14:35:10.123+06:00\"";
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_DataTypeDateInObject_ValidTrue()
    {
        var schema =
            """
            {
                "key1": #date,
                "key2": #date,
                "key3": #date
            }
            """;
        var json =
            """
            {
                "key1": "1950-12-31T11:40:10.000+06:30",
                "key2": "0001-01-01T00:00:00.000+00:00",
                "key3": "1600-02-29T23:59:59.999Z"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_DataTypeDateInArray_ValidTrue()
    {
        var schema =
            """
            [#date, #date]
            """;
        var json =
            """
            ["4444-12-31T11:40:10.000+06:30", "1000-01-31T23:59:59.999Z"]
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_NestedDataTypeDateInArray_ValidTrue()
    {
        var schema =
            """
            #date*
            """;
        var json =
            """
            ["9999-12-31T11:40:10.000+06:30", "0011-06-30T23:59:59.999Z"]
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_NestedDataTypeDateInObject_ValidTrue()
    {
        var schema =
            """
            #date*
            """;
        var json =
            """
            {
                "key1": "9999-12-31T11:40:10.000+06:30",
                "key2": "0011-06-30T23:59:59.999Z"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_DateFunctionWithYearMonthDay1InObject_ValidTrue()
    {
        var schema =
            """
            {
                "key1": @date("DD-MM-YYYY"),
                "key2": @date("YYYY-MM-DD"),
                "key3": @date("YY-M-D"),
                "key4": @date("DD-MM-YYYY G"),
                "key5": @date("YY-DD-MM"),
                "key6": @date("YYYY") #string,
                "key7": @date("YY-MM"),
                "key8": @date("D-M-YY") #string,
                "key9": @date("D-M-YYYY")
            }
            """;
        var json =
            """
            {
                "key1": "31-12-2024",
                "key2": "2034-12-31",
                "key3": "21-12-31",
                "key4": "31-12-2024 AD",
                "key5": "60-31-12",
                "key6": "1900",
                "key7": "23-12",
                "key8": "29-2-24",
                "key9": "29-2-2024"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_DateFunctionWithYearMonthDay2InObject_ValidTrue()
    {
        var schema =
            """
            {
                "key1": @date("MMMM DD, YYYY") #string,
                "key2": @date("YYYY-MMM-DD"),
                "key3": @date("DDDD, MMMM D, YY") #string,
                "key4": @date("DD-MMM-YYYY G"),
                "key5": @date("DDD D MMM, YY G") #string,
                "key6": @date("DDDD MMMM DD, YYYY"),
                "key7": @date("MMMM, DD-MM-YYYY")
            }
            """;
        var json =
            """
            {
                "key1": "January 01, 1985",
                "key2": "2034-Dec-31",
                "key3": "Tuesday, July 11, 23",
                "key4": "10-apr-2020 AD",
                "key5": "tue 2 apr, 30 ad",
                "key6": "Tuesday January 01, 1985",
                "key7": "January, 01-01-1985"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_DateFunctionWithHourMinuteSecondInObject_ValidTrue()
    {
        var schema =
            """
            {
                "key1": @date("hh:mm:ss") #string,
                "key2": @date("h:m:s"),
                "key3": @date("hh:mm:ss t") #string,
                "key4": @date("h:m:s t"),
                "key5": @date("hh:mm:ss.fff"),
                "key6": @date("hh:mm:ss.F"),
                "key7": @date("hh") #string,
                "key8": @date("hh:mm")
            }
            """;
        var json =
            """
            {
                "key1": "11:11:11",
                "key2": "1:10:59",
                "key3": "12:59:59 AM",
                "key4": "1:2:30 pm",
                "key5": "23:59:59.999",
                "key6": "00:00:00.9999",
                "key7": "21",
                "key8": "21:00"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_DateFunctionWithUtcOffsetInObject_ValidTrue()
    {
        var schema =
            """
            {
                "key1": @date("hh:mm:ss Z") #string,
                "key2": @date("hh:mm:ss ZZ"),
                "key3": @date("hh:mm:ss ZZZ") #string,
                "key4": @date("h:m:s ZZ")
            }
            """;
        var json =
            """
            {
                "key1": "11:11:59 +06",
                "key2": "12:59:00 -06:30",
                "key3": "12:00:59 -0630",
                "key4": "23:59:59 Z"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_DateFunctionWithPartialDateTimeInObject_ValidTrue()
    {
        var schema =
            """
            {
                "key1": @date("DD-MM-YYYY hh:mm:ss.fff ZZ"),
                "key2": @date("YY-M-D h:m:s Z"),
                "key3": @date("D") #string,
                "key4": @date("MM"),
                "key5": @date("hh"),
                "key6": @date("m") #string,
                "key7": @date("ss") #string,
                "key8": @date("YY") #string
            }
            """;
        var json =
            """
            {
                "key1": "31-12-2020 23:59:59.999 +06:00",
                "key2": "34-1-3 5:8:10 +02",
                "key3": "31",
                "key4": "12",
                "key5": "23",
                "key6": "59",
                "key7": "00",
                "key8": "24"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_NestedDateFunctionInObject_ValidTrue()
    {
        var schema =
            """
            @date*("DD-MM-YYYY") #string*
            """;
        var json =
            """
            {
                "key1": "01-01-1900",
                "key2": "31-07-0001",
                "key3": "29-02-2024"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
    
    [TestMethod]
    public void When_NestedDateFunctionInArray_ValidTrue()
    {
        var schema =
            """
            @date*("DD-MM-YYYY") #string*
            """;
        var json =
            """
            [
                "01-01-1900",
                "31-07-0001",
                "29-02-2024"
            ]
            """;
        JsonAssert.IsValid(schema, json);
    }
}