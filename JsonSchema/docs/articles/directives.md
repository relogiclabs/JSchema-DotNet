<style>
pre code { font-size: 1.1em; }
</style>

# Validation Directives
Directives serve as special instructions or commands for the Schema and JSON parsers, interpreters, and validators. They are used to control various aspects of the validation process or to provide metadata for documentation. Additionally, they offer crucial information about Schema and JSON and provide custom validation functions to meet specific Schema validation requirements.

## Title Directive
Within a schema, the title directive is used to provide a name, label, or a brief intent of the schema for which it is written. Besides, the title directive is optional and additional description can be supplied as multiple comments inside the schema document to provide more detail.

However, this directive is only used for documentation purposes and does not have any impact in the validation process. To represent the title directive, the following example of notation can be used:
```stylus
%title: "Example name or brief description"
```

## Version Directive
In a schema, the version directive is used to provide a version number of the schema which helps to keep track of updates. Although optional, the version directive is useful for documentation purposes and does not affect the validation process. The version directive can be represented using the following notation example:
```stylus
%version: 2023.09.11.01
```

## Include Directive
Include directive enables the addition or inclusion of a class, as defined by object-oriented programming, to a schema along with a set of methods that have specific signatures for performing custom validations. This feature extends the built-in validation capabilities of the schema. In the C# language, it is also necessary to specify the assembly name together with the class name. The example below illustrates how to utilize the include directive in C# language:
```stylus
%include: RelogicLabs.JsonSchema.Tests.Positive.ExternalFunctions,
                                     RelogicLabs.JsonSchema.Tests
```

## Pragma Directive
Pragma directives provide a mechanism to modify the default settings for certain predefined parameters of the validation process. This allows you to adjust the behaviours of schema validation process as per your requirements.

### Ignore Undefined Properties
The `IgnoreUndefinedProperties` pragma directive serves the purpose of instructing the validation process on how to handle JSON properties that are not explicitly defined in the schema, not even by the undefined marker or symbol. You can use this directive to specify whether such properties should be ignored during validation or if validation errors should be raised for any undefined properties in the JSON document.

The default value of this directive is `false`, which means that by default, undefined properties in the JSON document are not ignored, and validation errors will be raised for them. For example, the following usage of this directive instructs the validation process to ignore any undefined properties in the JSON document:
```stylus
%pragma IgnoreUndefinedProperties: true
```

### Floating Point Tolerance
The `FloatingPointTolerance` pragma directive allows you to define the tolerance level for relative errors in floating-point numbers during calculations and computations carried out by the validation process. By default, this directive is set to `1E-10`, indicating a small tolerance. However, you have the flexibility to adjust this value to any desired number. To specify a custom tolerance value of `1E-07`, you can use the following notation as an example:
```stylus
%pragma FloatingPointTolerance: 1E-07
```

### Ignore Object Property Order
The `IgnoreObjectPropertyOrder` pragma directive provides a means to enforce a specific order or sequence of JSON object properties, following the schema definition. This requirement for strict ordering is only needed in certain special cases. By default, this directive is set to `true`, meaning that the object property order outlined in the schema document is not mandatory. However, you can override this default by setting it to `false`, as shown in the following example below:
```stylus
%pragma IgnoreObjectPropertyOrder: false
```

## Definition / Define Directive
This feature in JSON schemas allows you to define a name for a fragment of schema or validation rules, which can be referenced from various parts of your schema. This means that if you encounter similar validation requirements in different sections of your schema, you can conveniently refer to the named fragment instead of duplicating the same validation rules. 

By providing clear and descriptive names for these validation rules or sub-schemas, you enhance the overall clarity and context of your schema. This clarity not only makes it easier to understand the structure and intent of the schema but also contributes to keeping your complex schema well-organized, concise, and more manageable. 

The name or alias of the directive is always start with `$` which also refers to that they are named fragment defined elsewhere in the schema. Here is a simple example of how to use this directive:
```stylus
%define $product: {
    "id": @length(2, 10) @regex("[a-z][a-z0-9]+") #string,
    "name": @length(5, 100) #string,
    "price": @range(0.1, 1000000),
    "inStock": #boolean
}
```

## Schema Directive
The schema directive serves as the starting or entry point for both the schema document and the schema validation process. It becomes mandatory when other directives are present within the document. In such cases, the schema directive explicitly designates the beginning of the schema document and defines the entry point for validation process.

However, if there are no other directives used in the document, the entire document itself is automatically considered as the schema document, with the document's beginning serving as its entry point. To illustrate, here is a simple example of a schema document with schema directive:
```stylus
%schema:
{
    "user": {
        "id": @range(1, 10000) #integer,
        /*username does not allow special characters*/
        "username": @regex("[a-z_]{3,30}") #string,
        /*currently only one role is allowed by system*/
        "role": "user" #string,
        "isActive": #boolean, //user account current status
        "registeredAt": #time
    }
}
```