# Date and Time Patterns
Dates and times are fundamental data types used in a wide range of data exchange scenarios. This schema offers a comprehensive set of tools for managing dates and times, including pattern-based formatting to meet the unique requirements of various systems and applications. Simultaneously, it incorporates validation processes to ensure compliance with the schema.

Date and time formats are defined using date and time pattern strings. Within these pattern strings, unquoted letters ranging from 'A' to 'Z' and 'a' to 'z' serve as placeholders representing various components of a date or time string. To prevent their interpretation as patterns, text can be enclosed in single quotes ('). Additionally, two consecutive single quotes ('') are used to represent a literal single quote within the string. Any characters outside of these uppercase and lowercase letters, as well as the text enclosed in single quotes, are not interpreted and are matched against the input string during validation.

Below, you will find a list of pattern letters and strings that are defined for date and time patterns. Please note that any characters within the 'A' to 'Z' and 'a' to 'z' range are reserved and should not be directly included in the pattern string.

| Pattern | Description                           | Example |
|---------|---------------------------------------|---------|
| G       | Era period designator                 | AD      |
| YYYY    | Four digit year number                | 1970    |
| YY      | Two digit year number                 | 70      |
| MM      | Month number in year (2 digit form)   | 01      |
| MMMM    | Full name of month                    | January |
| MMM     | Short name of month                   | Jan     |
| M       | Month number in year (1-2 digit form) | 1, 01   |
| DDDD    | Full name of day in week              | Monday  |
| DDD     | Short name of day in week             | Mon     |
| DD      | Day in month (2 digit form)           | 01      |
| D       | Day in month (1-2 digit form)         | 1, 01   |
| t       | AM/PM designator                      | AM, PM  |
| hh      | Hour in day (2 digit form)            | 01, 12  |
| h       | Hour in day (1-2 digit form)          | 1, 01   |
| mm      | Minute in hour (2 digit form)         | 01, 20  |
| m       | Minute in hour (1-2 digit form)       | 1, 01   |
| ss      | Second in minute (2 digit form)       | 1, 01   |
| s       | Second in minute (1-2 digit form)     | 1, 01   |
| f       | Tenths of a second                    | 1       |
| ff      | Hundredths of a second                | 11      |
| fff     | Milliseconds of a second              | 111     |
| ffff    | Ten thousandths of a second           | 1111    |
| fffff   | Hundred thousandths of a second       | 11111   |
| ffffff  | Millionths of a second                | 111111  |
| F       | Fraction of a second upto 6 digits    | 1, 111  |

The pattern components listed above can be combined to create comprehensive and customized date and time patterns to accommodate all system and user requirements. The following table illustrates some examples of how different date-time pattern components can be combined.

| Combined Pattern            | Example                       |
|-----------------------------|-------------------------------|
| YYYY-MM-DD                  | 2023-09-01                    |
| YYYY-MM-DD'T'hh:mm:ss.fffZZ | 2023-09-01T14:35:10.123+06:00 |
| MMMM DD, YYYY G             | January 01, 1980 AD           |
| DDDD, D MMMM YYYY           | Tuesday, 11 July 2023         |
| YYYY.MM.DD hh.mm.ss t       | 1980.11.21 10.30.50 pm        |
| DDD, D MMM YY hh:mm:ss ZZ   | Sun, 4 Jul 99 12:08:56 -06:00 |
| hh:mm:ss t ZZ               | 03:11:30 AM +06:00            |

The first pattern in the table above adheres to the ISO 8601 date format and is used to validate the `#date` data type within the schema. The second pattern in the table follows the ISO 8601 format for date and time, validating the `#time` data type in the schema. Instead of explicitly specifying these patterns in the `@date` or `@time` functions, a more concise approach is to directly utilize the `#date` or `#time` type within the schema.

When the AM/PM designator is included in the date and time pattern, the presence of any hour format specifier indicates that the time is in the 12-hour clock format. Conversely, when the AM/PM designator is omitted, the presence of any hour format specifier indicates that the time is in the 24-hour clock format.



