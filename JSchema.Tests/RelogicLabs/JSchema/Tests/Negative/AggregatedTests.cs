using RelogicLabs.JsonSchema.Exceptions;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Tests.External.ExternalFunctions;

namespace RelogicLabs.JsonSchema.Tests.Negative;

[TestClass]
public class AggregatedTests
{
    [TestMethod]
    public void When_JsonSchemaAggregatedTestWithWrongData_ExceptionThrown()
    {
        var schema =
            """
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
                    "profile": {
                        "firstName": @regex("[A-Za-z ]{3,50}") #string,
                        "lastName": @regex("[A-Za-z ]{3,50}") #string,
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
            """;
        var json =
            """
            {
                "user": {
                    "id": "not number",
                    "username": "john doe",
                    "role": "user",
                    "isActive": true,
                    "profile": {
                        "firstName": "John",
                        "lastName": "Doe",
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
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DTYP04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_ExtendedAggregatedTestWithInvalidAccess_ExceptionThrown()
    {
        var schema = """
        %title: "Extended User Profile Dashboard API Response"
        %version: 2.0.0
        %include: RelogicLabs.JsonSchema.Tests.External.ExternalFunctions,
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
        %define $tags: @length(1, 10) #string*($tag) #array
        %define $tag: @length(3, 20) @regex("[A-Za-z_]+") #string
        %schema: 
        {
            "user": {
                "id": @range(1, 10000) #integer,
                /*username does not allow special characters*/
                "username": @regex("[a-z_]{3,30}") #string,
                "role": @enum("user", "admin") #string &role,
                "isActive": #boolean, //user account current status
                "registeredAt": @time("DD-MM-YYYY hh:mm:ss") #string,
                "dataAccess": @checkAccess(&role) #integer,
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
                "temperature": @range(-50, 60) #integer #float,
                "isCloudy": #boolean
            }
        }
        """;
        var json = """
        {
            "user": {
                "id": 1234,
                "username": "johndoe",
                "role": "user",
                "isActive": true,
                "registeredAt": "06-09-2023 15:10:30",
                "dataAccess": 6,
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
                        "cpu": "Intel i11",
                        "ram": "11GB",
                        "storage": "11GB SSD"
                    }
                }
            ],
            "weather": {
                "temperature": 25.5,
                "isCloudy": false,
                "conditions": null
            }
        }
        """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(ERRACCESS01, exception.Code);
        Console.WriteLine(exception);
    }
}