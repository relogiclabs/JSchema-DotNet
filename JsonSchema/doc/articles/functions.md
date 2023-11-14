<style>
pre code { font-size: 1.1em; }
table th:first-of-type { min-width: 140px; }
</style>

# Constraint Functions
This document serves as a brief overview, providing key insights into the built-in constraint functions that are part of the core schema. These functions significantly extend the scope of data and structural validation, going beyond the limits of basic data type restrictions. These functions are designed to enhance the effectiveness of schema validation, ensuring the accuracy, consistency, integrity, and compliance of the JSON data to the schema.

The notation below outlines the typical structure of constraint or validation functions applicable to JSON elements or values. In this notation, `Target` comprises two components: the target type, which specifies the JSON value type under consideration, and the target name, which identifies the specific JSON value to which the validation or constraint function applies. The `Constraint-Function` refers to the name of the function responsible for the validation.
```yaml
Target - Constraint-Function[(Parameter-Set)]
```
The `Parameter-Set` contains the parameters that control the validation process provided by the constraint function on the target JSON value. Please note that the `Parameter-Set` including the opening and closing parentheses are optional. The ellipsis or three dots `...` after a parameter type indicates  it can accept any number of arguments of that type. When using multiple validation functions, each function validates the target JSON value, and the overall validation succeeds only when every function independently deems the target JSON value as valid.

## Function Details
Below, you will find a detailed explanation of the syntax and useful applications of each function, allowing you to gain a clear understanding of their usage.

### String Length
```stylus
#string target - @length(#integer number)
```
Validates that the `target` string has the length specified by the `number`. If the length of the `target` string does not match the value specified by `number`, a validation error will generate.

```stylus
#string target - @length(#integer minimum, #integer maximum)
```
Validates that the length of the target string satisfies the range requirement specified by the parameters. It checks that the length of the target string is equal to or greater than the `minimum` length specified and simultaneously less than or equal to the `maximum` length specified. If not, a validation error will generate.

If either the parameter values for `minimum` or `maximum` are unspecified or undefined, the `undefined` symbol `!` can be used in place of either of these parameters. The following examples illustrate the various use cases of the `@length` function of the two variations described above, for the target data type string:

| Ues Cases       | Valid Values                   | Invalid Values           |
|-----------------|--------------------------------|--------------------------|
| `@length(4)`    | `"ABCD"`                       | `"AB"`; `"ABCDE"`        |
| `@length(2, 4)` | `"AB"`; `"ABC"`; `"ABCD"`      | `""`; `"A"`; `"ABCDE"`   |
| `@length(2, !)` | `"AB"`; `"ABCDEFGH"`           | `""`; `"A"`              |
| `@length(!, 4)` | `""`; `"A"`; `"ABC"`; `"ABCD"` | `"ABCDE"`; `"ABCDEFGHI"` |

### Array Length
```stylus
#array target - @length(#integer number)
```
Validates that the `target` array has the length specified by the `number`. If the length of the `target` array does not match the value specified by `number`, a validation error will generate.

```stylus
#array target - @length(#integer minimum, #integer maximum)
```
Validates that the length of the target array satisfies the range requirement specified by the parameters. It checks that the length of the target array is equal to or greater than the `minimum` length specified and simultaneously less than or equal to the `maximum` length specified. If not, a validation error will generate.

If either the parameter values for `minimum` or `maximum` are unspecified or undefined, the `undefined` symbol `!` can be used in place of either of these parameters. The following examples illustrate the various use cases of the `@length` function of the two variations described above, for the target data type array:

| Ues Cases       | Valid Values                          | Invalid Values                          |
|-----------------|---------------------------------------|-----------------------------------------|
| `@length(4)`    | `[1, 2, 3, 4]`                        | `[1, 2, 3]`; `[1, 2, 3, 4, 5]`          |
| `@length(2, 4)` | `[1, 2]`; `[1, 2, 3]`; `[1, 2, 3, 4]` | `[]`; `[1]`; `[1, 2, 3, 4, 5]`          |
| `@length(2, !)` | `[1, 2]`; `[1, 2, 3, 4, 5]`           | `[]`; `[1]`                             |
| `@length(!, 4)` | `[]`; `[1, 2]`; `[1, 2, 3, 4]`        | `[1, 2, 3, 4, 5]`; `[1, 2, 3, 4, 5, 6]` |

### Object Length / Size
```stylus
#object target - @length(#integer number)
```
Validates that the `target` object has the length or size specified by the `number`. If the length of the `target` object does not match the value specified by `number`, a validation error will generate.

```stylus
#object target - @length(#integer minimum, #integer maximum)
```
Validates that the length or size of the target object satisfies the range requirement specified by the parameters. It checks that the length of the target object is equal to or greater than the `minimum` length specified and simultaneously less than or equal to the `maximum` length specified. If not, a validation error will generate.

If either the parameter values for `minimum` or `maximum` are unspecified or undefined, the `undefined` symbol `!` can be used in place of either of these parameters. The following examples illustrate the various use cases of the `@length` function of the two variations described above, for the target data type object:

| Ues Cases       | Valid Values                                                   | Invalid Values                                                         |
|-----------------|----------------------------------------------------------------|------------------------------------------------------------------------|
| `@length(4)`    | `{"k1":1, "k2":2, "k3":3, "k4":4}`                             | `{"k1":1, "k2":2, "k3":3}`; `{"k1":1, "k2":2, "k3":3, "k4":4, "k5":5}` |
| `@length(2, 4)` | `{"k1":1, "k2":2}`; `{"k1":1, "k2":2, "k3":3, "k4":4}`         | `{}`; `{"k1":1}`; `{"k1":1, "k2":2, "k3":3, "k4":4, "k5":5}`           |
| `@length(2, !)` | `{"k1":1, "k2":2}`; `{"k1":1, "k2":2, "k3":3, "k4":4, "k5":5}` | `{}`; `{"k1":1}`                                                       |
| `@length(!, 4)` | `{}`; `{"k1":1, "k2":2}`; `{"k1":1, "k2":2, "k3":3, "k4":4}`   | `{"k1":1, "k2":2, "k3":3, "k4":4, "k5":5}`                             |

### Number Range
```stylus
#number target - @range(#number minimum, #number maximum)
```
Validates that the `target` number satisfies the range requirement specified by the parameters. It checks that the `target` number is greater than or equal to the `minimum` number specified and simultaneously less than or equal to the `maximum` number specified. If not, a validation error will generate. 

If either the parameter values for `minimum` or `maximum` are unspecified or undefined, the `undefined` symbol `!` can be used in place of either of these parameters. The following examples illustrate the various use cases of the `@range` function of the two variations described above, for the target data type number:

| Ues Cases      | Valid Values          | Invalid Values          |
|----------------|-----------------------|-------------------------|
| `@range(2, 4)` | `2`; `3`; `4`         | `0`; `1`; `-100`; `100` |
| `@range(2, !)` | `2`; `3`; `4`; `100`  | `0`; `1`; `-100`        |
| `@range(!, 4)` | `0`; `1`; `4`; `-100` | `5`; `10`; `100`        |

### Number Minimum
```stylus
#number target - @minimum(#number minimum)
#number target - @minimum(#number minimum, #boolean exclusive)
```
Validates that the `target` number is greater than or equal to the `minimum` number specified as a parameter, unless the `exclusive` parameter flag is explicitly set to `true`, in which case the `target` number must be exclusively greater than the `minimum` number.

| Ues Cases          | Valid Values             | Invalid Values           |
|--------------------|--------------------------|--------------------------|
| `minimum(0)`       | `0`; `1`; `1000`         | `-1`; `-10`; `-10000`    |
| `minimum(10.5)`    | `10.5`; `10.6`; `1000.1` | `10.49`; `1.0`; `-100.1` |
| `minimum(0, true)` | `0.001`; `1.01`; `100.1` | `0`; `-0.01`; `-100.1`   |

### Number Maximum
```stylus
#number target - @maximum(#number maximum)
#number target - @maximum(#number maximum, #boolean exclusive)
```
Validates that the `target` number is less than or equal to the `maximum` number specified as a parameter, unless the `exclusive` parameter flag is explicitly set to `true`, in which case the `target` number must be exclusively less than the `maximum` number.

| Ues Cases          | Valid Values                 | Invalid Values            |
|--------------------|------------------------------|---------------------------|
| `maximum(100)`     | `100`; `-100`; `0`           | `101`; `1000`; `10000`    |
| `maximum(10.5)`    | `10.50`; `10.49`; `-1000.1`  | `10.51`; `11.0`; `1000.1` |
| `maximum(0, true)` | `-0.001`; `-1.01`; `-1000.1` | `0`; `0.01`; `100.1`      |

### String Enum
```stylus
#string target - @enum(#string... items)
```
Validates that the `target` string is equal to one of the strings specified by the `items` parameter. If not, a validation error will generate.

### Number Enum
```stylus
#number target - @enum(#number... items)
```
Validates that the `target` number is equal to one of the numbers specified by the `items` parameter. If not, a validation error will generate.

### Array Elements
```stylus
#array target - @elements(#any... items)
```
Validates that the `target` array contains every JSON value specified by the `items` parameter. If not, it generates a validation error.

### Object Keys
```stylus
#object target - @keys(#string... items)
```
Validates that all the strings specified in the `items` parameter are present as keys in the `target` object. If any of them is missing, a validation error is generated.

### Object Values
```stylus
#object target - @values(#any... items)
```
Validates that all the JSON values specified in the `items` parameter are present as values in the `target` object. If any of them is missing, a validation error is generated.

### String Regular Expression (Regex)
```stylus
#string target - @regex(#string pattern)
```
Validates that the `target` string matches the regular expression pattern specified by the `pattern` parameter. The regular expression engine used here supports standard syntax from both POSIX (IEEE Portable Operating System Interface) Extended Regular Expressions and Perl-Compatible Regular Expressions (PCRE). For more details, please refer to [POSIX Regular Expressions](https://www.regular-expressions.info/posix.html).

### Email Address
```stylus
#string target - @email
```
Validates whether the `target` string represents a valid email address. It follows the SMTP protocol RFC 5322 specification for mailbox address format to identify a valid email address. In addition to conforming to this standard, it recognizes all widely used email address formats to ensure compatibility with various systems and user requirements.

### URL & URI Address
```stylus
#string target - @url(#string scheme)
```
Validates whether the `target` string is a valid URL (Uniform Resource Locator) or URI (Uniform Resource Identifier) with a specific scheme provided by the `scheme` parameter. It follows the RFC 3986 URI Generic Syntax to determine the validity of the URL or URI. In addition to conforming to this standard, it recognizes all widely used URL and URI address formats, ensuring compatibility with a wide range of systems and user requirements.

```stylus
#string target - @url
```
Validates whether the `target` string is a valid URL or URI with `HTTP` and `HTTPS` scheme. For more information please check the function `#string target - @url(#string scheme)`.

### Phone Number
```stylus
#string target - @phone
```
Validates whether the `target` string is a valid phone number. It follows the ITU-T E.163 and E.164 telephone number notation to determine the validity of the phone number. In addition to conforming to this standard, it recognizes all widely used national and international phone number formats, ensuring compatibility with a wide range of systems and user requirements.

### Date and Time
```stylus
#string target - @date(pattern)
```
Validates that the `target` string matches the date and time pattern specified by the `pattern` parameter. It fully supports the ISO 8601 date and time format. Beyond this standard, it also allows custom date and time formats, ensuring compatibility with various systems and meeting diverse users and businesses requirements. This [document](/JsonSchema-DotNet/articles/datetime.html) provides a comprehensive overview of the date-time custom patterns.

```
#string target - @time(pattern)
```
Both the `@date` and `@time` functions support a complete range of date-time patterns, enabling the precise definition of any date and time scenario. Therefore, these functions can be used interchangeably. When the sole consideration is the date or day of the month in a year, employing the `@date` function is the more convenient choice. In contrast, when it becomes necessary to specify a particular time on a date, the `@time` function is the more appropriate option. To learn more about date-time patterns, please refer to [this page](/JsonSchema-DotNet/articles/datetime.html).

### Date and Time Range
```stylus
#datetime target - @range(#string start, #string end)
```
Validates that the `target` date-time satisfies the range requirement specified by the parameters. It checks that the `target` date-time is from or after the `start` date-time specified and simultaneously until and before the `end` date-time specified. If not, a validation error will generate. The `start` and `end` parameters must be the string representation of the `target` data type, which can either be a `#date` or `#time` type.

If either the parameter values for `start` or `end` are unspecified or undefined, the `undefined` symbol `!` can be used in place of either of these parameters. The following examples illustrate the various use cases of the `@range` function of the two variations described above, for the target type:

| Ues Cases                                                        | Valid Values                                           | Invalid Values                           |
|------------------------------------------------------------------|--------------------------------------------------------|------------------------------------------|
| `@range("2010-01-01", "2010-12-31")`                             | `2010-01-01`; `2010-06-30`; `2010-12-31`               | `2009-12-31`; `2011-01-01`; `2030-11-05` |
| `@range("2010-01-01T00:00:00.000Z", "2010-12-31T23:59:59.999Z")` | `2010-01-01T00:00:00.000Z`; `2010-12-31T23:59:59.999Z` | `2009-12-31T23:59:59.999Z`               |
| `@range(!, "2010-12-31")`                                        | `1990-01-01`; `2010-12-31`                             | `2011-01-01`; `2030-11-05`               |
| `@range("2010-01-01", !)`                                        | `2010-01-01`; `2030-11-05`                             | `1990-01-01`; `2009-12-31`               |

### Date and Time Start
```stylus
#datetime target - @start(#string reference)
```
Validates that the `target` date-time starts from or finds after the specified `reference` date-time parameter. If the `target` date-time finds before the `reference` date-time, a validation error is triggered. The `reference` parameter must be the string representation of the `target` data type, which can either be a `#date` or `#time` type.

### Date and Time End
```stylus
#datetime target - @end(#string reference)
```
Validates that the `target` date-time finds before or ends at the specified `reference` date-time parameter. If the `target` date-time finds after the `reference` date-time, a validation error is triggered. The `reference` parameter must be the string representation of the `target` data type, which can either be a `#date` or `#time` type.

### Date and Time Before
```stylus
#datetime target - @before(#string reference)
```
Validates that the `target` date-time is exclusively before the `reference` date-time. If the `target` date-time finds on or after the `reference` date-time, a validation error is triggered. The `reference` parameter must be the string representation of the `target` data type, which can either be a `#date` or `#time` type.

### Date and Time After
```stylus
#datetime target - @after(#string reference)
```
Validates that the `target` date-time is exclusively after the `reference` date-time. If the `target` date-time finds on or before the `reference` date-time, a validation error is triggered. The `reference` parameter must be the string representation of the `target` data type, which can either be a `#date` or `#time` type.

### Number Positive
```stylus
#number target - @positive
```
Validates that the `target` number is positive. If the `target` number is zero or negative, it generates a validation error.

### Number Negative
```stylus
#number target - @negative
```
Validates that the `target` number is negative. If the `target` number is zero or positive, it generates a validation error.

### String Not Empty
```stylus
#string target - @nonempty
```
Validates that the `target` string is not empty. If the `target` string is empty, it generates a validation error.

### Array Not Empty
```stylus
#array target - @nonempty
```
Validates that the `target` array is not empty. If the `target` array is empty, it generates a validation error.

### Object Not Empty
```stylus
#object target - @nonempty
```
Validates that the `target` object is not empty. If the `target` object is empty, it generates a validation error.