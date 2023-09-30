<style>
pre code { font-size: 1.3em; }
</style>

# Validation Syntax
A JSON document is a structured data format used for the exchange of information between applications. It primarily consists of two types of values: composite values and non-composite values, the latter also referred to as primitive values.

Composite values in a JSON document act as containers. They can hold zero or more composite or non-composite primitive values. These composite values provide the structural framework for organizing data hierarchically within JSON documents. There are two types of composite values allowed in JSON documents: Arrays and Objects.

Conversely, Non-composite values are the atomic data elements in a JSON document. They cannot contain other values within them. There are four types of primitive values supported by JSON specification: Strings, Numbers, Booleans, and Nulls.

## Validation Format
A JSON Schema ensures the correctness and consistency of JSON documents and it also defines the structure and constraints that a JSON document must conform to. It specifies how both composite and non-composite values should be organized and validated the input document based on the rules specified in the schema document. Thus a key element of JSON Schema is the validation rule syntax, which provides the required instructions for the validation process. A validation rule is typically expressed using the following notation:
```yaml
[Value] [Function-Set] [DataType-Set] [Optional]
[Undefined] [Optional]
```

| SN | Component                                  | Example                                                  |
|----|--------------------------------------------|----------------------------------------------------------|
| 1  | `Value`                                    | `10`, `"string"`, `[10, 20, 30]`, `{ "key1": "value1" }` |
| 2  | `Function-Set`                             | `@range(1, 10)`, `@length(5, 50) @regex("[A-Za-z]+")`    |
| 3  | `DataType-Set`                             | `#string`, `#object #null` `#number* #array`             |
| 4  | `Value Optional`                           | `10 ?`, `"string" ?`, `[10, 20, 30] ?`                   |
| 5  | `Function-Set Optional`                    | `@range(1, 10) ?`, `@length(5, 50) ?`                    |
| 6  | `DataType-Set Optional`                    | `#string ?`, `#integer ?`, `#array ?`                    |
| 7  | `Function-Set DataType-Set`                | `@range(1, 10) #integer`, `@length(5, 10) #string`       |
| 8  | `Function-Set DataType-Set Optional`       | `@range(1, 10) #integer ?`, `@length(5, 10) #string ?`   |
| 9  | `Value Function-Set DataType-Set Optional` | `10 @range(1, 100) #integer ?`                           |
| 10 | `Undefined`                                | `!`                                                      |
| 11 | `Undefined Optional`                       | `! ?`                                                    |

The syntax in the 9th row of the table is valid but uncommon in real-world scenarios. Since constraint function and data type provide unnecessary validation considering the validation for value is succeeded.

Now, let's explore the components of this notation and their functionalities. In the context of the validation rule, `Value` refers to a specific input JSON value of the document. This value can be either a composite value (e.g., an object or an array) or a primitive value (e.g., a string or a number). The inclusion of `Value` in the validation rule is optional, meaning that you can choose whether or not to specify a particular JSON value for validation. When `Value` is present in the rule, it serves as a requirement, implying that the specified JSON value must match with the input JSON value of the document for the validation to succeed. If it does not match with the input value, the validation will fail.

The `Function-Set` is an optional part of the validation rule, and it can consist of one or more function constraints. Function constraints are rules or conditions that are applied to validate the input JSON value. These functions can be of two types:

  1. Functions applied directly to the target value itself for which the validation rule is defined.
  2. Functions applied to components or items within the target value are applicable and valid only if the value is a composite JSON value. An asterisk `*` is used after the function name to indicate that the constraint function is applied to the nested components.

The validation of the `Function-Set` as a whole is considered successful only if each function constraint within it succeeds, regardless of its type and application mode.

Similar to the `Function-Set`, the `Datatype-Set` is also an optional part of the validation rule and can contain one or more data type constraints. Data type constraints define the expected data types applicable to the value itself or its nested components, depending on whether the value is composite or primitive for the input JSON value. As like function constraints data type can be of two types:

  1. Data type applied directly to the target value itself for which the validation rule is defined.
  2. Data type applied to components or items within the target value are applicable and valid only if the value is a composite JSON value. An asterisk `*` is used after the data type name to indicate that the data type is applied to the nested components.

Validation of the `Datatype-Set` is deemed successful if validation is successful for one of the type 1 (or top-level) data types and one of the type 2 (nested) data types. This becomes particularly relevant in scenarios where an optional composite target value, such as an array or object, is also permitted to be null. In real-world scenarios, it's rare for a target value to exhibit multiple data types, like switching between being a number and a string. Consequently, schema rules typically do not specify multiple top-level or nested data types.

The `Optional` specifier, denoted as `?`, signifies that the presence of the target value is optional within the input JSON document. When `Optional` is specified, it indicates that the value may or may not be present. If it is absent, no validation is performed for that specific target value, and the JSON document is considered valid. However, if the target value is present, the validation rule must succeed for the document to be considered valid and conform to Schema. Failure to meet the validation rule renders the JSON document invalid. In the absence of the `Optional` specifier, the target JSON value must be present in the provided input JSON document for validation to succeed.

In instances where no validation rule (or no components of the validation rule) is explicitly defined for a target input JSON value, the use of the undefined marker `!` signifies that any valid JSON value is acceptable for the target. This allows more flexibility in JSON data validation for specific cases.