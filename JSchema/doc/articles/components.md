# Schema Components
A schema component, also known as a reusable schema fragment or sub-schema, plays a vital role in improving readability, reducing redundancy, and organizing the structure of a Schema document. In JSON validation, a schema component or fragment defines a validation rule that can be recursively composed of multiple nested validation rules, collectively specifying the expected and valid format of a JSON construct.

These schema components are used as an extension of data type validation, as core data types have limited features to validate the internal structure of a composite JSON value or construct. Therefore, a data type is parameterized with a schema component to validate the internal structure of such composite JSON constructs. 

The name or alias of a schema component always starts with `$` which also refers to the fact that they are named schema components or fragments defined elsewhere in the schema. Schema components can be referenced from any other part of the schema document, effectively reducing redundancy and enhancing reusability and readability. The following example defines a simple schema component named `$component` where the validation rule describes an object structure with two key-value pairs:
```js
%define $component: { "key1": #integer, "key2": #string }
```

A composite JSON construct is created by combining multiple values as defined by the JSON specification. These nested values can range from simple, like numbers or strings, to more complex, such as arrays or objects. While simple nested values of a composite construct can be validated using only nested data types and functions, handling hierarchical composite constructs with multiple layers of nested structures requires defining schema components.

The second and third rows of the following table illustrate how the component validates the value associated with the data type for which it is used as a parameter. If the associated data type is direct, the component validates the target value itself. Conversely, if the associated data type is nested, the component validates each of the nested values comprising the composite target construct.

| SN | Component Example                                                                | Valid Json         |
|----|----------------------------------------------------------------------------------|--------------------|
| 1  | `@range*(1, 10) @length(5) #integer* #array`                                     | `[1, 3, 5, 8, 10]` |
| 2  | `%define $cmp: @range*(1, 10) #integer*` <br> `%schema: @length(5) #array($cmp)` | `[1, 3, 5, 8, 10]` |
| 3  | `%define $cmp: @range(1, 10)` <br> `%schema: @length(5) #integer*($cmp) #array`  | `[1, 3, 5, 8, 10]` |

In the above table, all three rows have identical validation constraints for the input JSON array. This demonstrates that when dealing with simple and primitive nested values in a composite JSON construct, preferring the nested data types and functions is more convenient due to their simplicity and conciseness. However, in cases where the nested values are complex and composite, the schema component syntax becomes more suitable. The following example illustrates how the component syntax can be used to validate elements of a JSON array that are not as straightforward as the previous examples:
```js
%define $article: {
    "id": @range(1, 100) #integer,
    "title": @length(10, 100) #string,
    "preview": @length(30, 1000) #string,
    "tags": @length*(3, 30) @length(1, 5) #string* #array
} #object

%schema: @length(1, 10) #object*($article) #array
```

In practical scenarios, JSON arrays often hold multiple composite JSON constructs as elements, typically sharing a recurring pattern and structure similar to the example above. To facilitate the validation of such elements, using schema components is highly effective. 

By defining a reusable schema component with a clear and descriptive name, one can improve the overall clarity and readability of the Schema document with recurring structures. This clarity not only makes it easier to understand the structure and intent of the schema but also contributes to keeping your complex schema well-organized, concise, and more manageable. For instance, consider the following example of a JSON document which is valid against the schema example provided above, demonstrating the usage of a schema component:
```js
[
    {
        "id": 1,
        "title": "Getting Started",
        "preview": "This guide will show you through the essential steps to quickly...",
        "tags": ["JSON", "JSchema", "Quick Start"]
    },
    {
        "id": 2,
        "title": "Validation Syntax",
        "preview": "A JSON document is a structured data format used for the exchange...",
        "tags": ["JSON", "JSchema", "Validation Syntax"]
    },
    {
        "id": 3,
        "title": "Constraint Functions",
        "preview": "This document serves as a brief overview, providing key insights into...",
        "tags": ["JSON", "JSchema", "Constraint Functions"]
    }
]
```