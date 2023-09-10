# A New JSON Schema
A JSON Schema is crucial for making communication, interoperability, validation, testing, documentation, and specification seamless. All of this combined contributes to better maintenance and evolution of data-driven applications and systems. For a comprehensive overview of the roles and uses of JSON Schema in modern web applications, we invite you to explore our dedicated post available [here](https://www.relogiclabs.com/2023/01/the-roles-of-json-schema.html).

## Design Goals
The traditional standard JSON Schema rigorously follows the conventional JSON structure, which unfortunately comes at the expense of simplicity, conciseness, and readability. Our goal is to develop a new JSON Schema that promotes these essential aspects that were previously missing.

This new schema is simple, lucid, easy to grasp, and doesn't require much prior knowledge to understand it. It also offers a shallow learning curve for both reading and writing. Additionally, its simplicity and conciseness allow us and machines to read-write more efficiently. Moreover, a large set of constraint data types and functions within the core schema facilitates the precise definition of JSON documents, significantly reducing the potential for communication gaps among collaborators. Furthermore, its inherent extensibility simplifies the process of integrating new constraints and functionalities to meet the diverse requirements of modern web services.

## Basic Example
Let's explore an example of our schema for a typical JSON API response containing information about a user profile or account. The schema is very self-explanatory and thus almost no prior knowledge is required to understand the schema and the JSON responses specified by this schema.
```stylus
%title: "User Profile Response"
%version: 1.0.0
%schema:
{
    "user": {
        "id": @range(1, 10000) #integer,
        /*username does not allow special characters*/
        "username": @regex("[a-z_]{3,30}") #string,
        /*currently only one role is allowed by system*/
        "role": "user" #string,
        "isActive": #boolean, //user account current status
        "registeredAt": #time,
        "profile": {
            "firstName": @regex("[A-Za-z ]{3,50}") #string,
            "lastName": @regex("[A-Za-z ]{3,50}") #string,
            "dateOfBirth": #date,
            "age": @range(18, 130) #integer,
            "email": @email #string,
            "pictureURL": @url #string,
            "address": {
                "street": @length(10, 200) #string,
                "city": @length(3, 50) #string,
                "country": @regex("[A-Za-z ]{3,50}") #string
            } #object #null
        }
    }
}
```
In the above example, two types of constraint or rule descriptors are used: constraint functions (also known as validation functions, such as `@range(1, 10000)`) and constraint data types (also known as validation data types, such as `#integer`). All constraint functions begin with the `@` symbol, while all constraint data types start with `#`. C-style comments are also permitted in the schema. Please note that `address` can be `null` (eg. an optional input for users) and if it is `null` then no constraints of `address` are applicable. The following JSON is one of the examples which can successfully validate against the above schema. To start your journey with the JSON validation library, please consult the documentation available [here](/JsonSchema-DotNet/articles/intro.html).
```json
{
    "user": {
        "id": 1234,
        "username": "johndoe",
        "role": "user",
        "isActive": true,
        "registeredAt": "2023-09-06T15:10:30.639Z",
        "profile": {
            "firstName": "John",
            "lastName": "Doe",
            "dateOfBirth": "1993-06-17",
            "age": 30,
            "email": "john.doe@example.com",
            "pictureURL": "https://example.com/picture.jpg",
            "address": {
                "street": "123 Some St",
                "city": "Some town",
                "country": "Some Country"
            }
        }
    }
}
```
## Extended Example
The next example represents an expanded version of the previous one, which brings more complexity. To effectively construct such schemas with multiple layers of nested structures, it's beneficial to have a fundamental understanding of this schema format. While the syntax may seem difficult at first, it becomes straightforward once you have a basic understanding of it. For more detailed information, reference documentation is available [here](/JsonSchema-DotNet/articles/intro.html).
```stylus
%title: "Extended User Profile Dashboard API Response"
%version: 2.0.0
%include: RelogicLabs.JsonSchema.Tests.Positive.ExternalFunctions,
            RelogicLabs.JsonSchema.Tests

%pragma IgnoreUndefinedProperties: true

%define $post: {
    "id": @range(1, 1000) #integer,
    "title": @length(10, 100) #string,
    "content": @length(30, 1000) #string,
    "tags": $tags
} #object

%define $product: {
    "id": @length(2, 10) @regex("[a-z][a-z0-9]+") #string,
    "name": @length(5, 30) #string,
    "brand": @length(5, 30) #string,
    "price": @range(0.1, 1000000),
    "inStock": #boolean,
    "specs": {
        "cpu": @length(5, 30) #string,
        "ram": @regex("[0-9]{1,2}GB") #string,
        "storage": @regex("[0-9]{1,4}GB (SSD|HDD)") #string
    } #object #null
}

%define $tags: @length(1, 10) #array($tag)
%define $tag: @length(3, 20) @regex("[A-Za-z_]+") #string

%schema: 
{
    "user": {
        "id": @range(1, 10000) #integer,
        /*username does not allow special characters*/
        "username": @regex("[a-z_]{3,30}") #string,
        /*currently only one role is allowed by system*/
        "role": "user" #string,
        "isActive": #boolean, //user account current status
        "registeredAt": #time,
        "profile": {
            "firstName": @regex("[A-Za-z]{3,50}") #string,
            "lastName": @regex("[A-Za-z]{3,50}") #string,
            "dateOfBirth": @date("DD-MM-YYYY") #string,
            "age": @range(18, 128) #integer,
            "email": @email #string,
            "pictureURL": @url #string,
            "address": {
                "street": @length(10, 200) #string,
                "city": @length(3, 50) #string,
                "country": @regex("[A-Za-z ]{3,50}") #string
            } #object #null,
            "hobbies": !?
        },
        "posts": @length(0, 1000) #object*($post) #array,
        "preferences": {
            "theme": @enum("light", "dark") #string,
            "fontSize": @range(9, 24) #integer,
            "autoSave": #boolean
        }
    },
    "products": #object*($product) #array,
    "weather": {
        "temperature": @range(-50.0, 60.0) #float,
        "isCloudy": #boolean
    }
}
```
The subsequent JSON sample is an illustrative example that successfully validates against the expanded schema mentioned earlier. Within this example, recurring JSON structure appear that can be validated through defining components. By reusing defined components, you can achieve a clear and concise schema when validating large JSON with repetitive structures instead of duplicating large and complex validation constraints across the schema. This improves the overall readability and maintainability of the schema.
```json
{
    "user": {
        "id": 1234,
        "username": "johndoe",
        "role": "user",
        "isActive": true,
        "registeredAt": "2023-09-06T15:10:30.639Z",
        "profile": {
            "firstName": "John",
            "lastName": "Doe",
            "dateOfBirth": "17-06-1993",
            "age": 30,
            "email": "john.doe@example.com",
            "pictureURL": "https://example.com/picture.jpg",
            "address": {
                "street": "123 Some St",
                "city": "Some town",
                "country": "Some Country"
            }
        },
        "posts": [
            {
                "id": 1,
                "title": "Introduction to JSON",
                "content": "JSON (JavaScript Object Notation) is a lightweight data interchange format...",
                "tags": [
                    "JSON",
                    "tutorial",
                    "data"
                ]
            },
            {
                "id": 2,
                "title": "Working with JSON in C#",
                "content": "C# provides built-in support for working with JSON...",
                "tags": [
                    "CSharp",
                    "JSON",
                    "tutorial"
                ]
            },
            {
                "id": 3,
                "title": "Introduction to JSON Schema",
                "content": "A JSON schema defines the structure and data types of JSON objects...",
                "tags": [
                    "Schema",
                    "JSON",
                    "tutorial"
                ]
            }
        ],
        "preferences": {
            "theme": "dark",
            "fontSize": 14,
            "autoSave": true
        }
    },
    "products": [
        {
            "id": "p1",
            "name": "Smartphone",
            "brand": "TechGiant",
            "price": 599.99,
            "inStock": true,
            "specs": null
        },
        {
            "id": "p2",
            "name": "Laptop",
            "brand": "SuperTech",
            "price": 1299.99,
            "inStock": false,
            "specs": {
                "cpu": "Intel i7",
                "ram": "16GB",
                "storage": "512GB SSD"
            }
        }
    ],
    "weather": {
        "temperature": 25.5,
        "isCloudy": true,
        "conditions": null
    }
}
```
For more information about the schema syntax format and library functionalities, please refer to the reference documentation [here](/JsonSchema-DotNet/api/index.html).
