namespace RelogicLabs.JSchema.Tests.Positive;

[TestClass]
public class AggregatedTests
{
    [TestMethod]
    public void When_SchemaAggregatedTest_ValidTrue()
    {
        var schema = """
        %title: "Example Schema For Some Json HTTP Request or Response"
        %version: "February 11, 2023"
        %import: RelogicLabs.JSchema.Tests.External.ExternalFunctions,
                 RelogicLabs.JSchema.Tests
        
        %pragma IgnoreUndefinedProperties: true
        
        %define $component1: {
            "key11": @regex("[a-z]+") #string,
            "key12": @email #string,
            "key13": $component2,
            "key14": $component3
        }
        %define $component2: [@length(1, 10) #string, @url #string]
        /* if it is null do not check for nested function or data type*/
        %define $component3: @range*(1, 100) #integer* #array #null
        %define $component4: @regex*("[A-Z]{3}") #string* ?
        
        %schema: {
            "key1": @range(1, 10) #integer,
            "key2": @enum("val1", "val2", "val3") #string,
            "key3": @elements(10, 15, 20) #integer*,
            "key4": @range(0, 1) #float #double,
            "key5": @even @range(1, 100) #integer,
            "key6": $component1,
            "key7": #boolean,
            "key8": $component4,
            "key9": ! ? // it is not decide yet
        }
        """;
        var json = """
        {
            "key1": 10,
            "key2": "val2",
            "key3": [5, 10, 15, 20],
            "key4": 0.5,
            "key5": 10,
            "key6": {
                "key11": "test",
                "key12": "email.address@gmail.com",
                "key13": ["Microsoft", "https://www.microsoft.com/en-us/"],
                "key14": [10, 20, 30, 40]
            },
            "key7": true,
            "key8": ["ABC", "EFG", "XYZ"]
        }
        """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_JsonAggregatedTest_ValidTrue()
    {
        var expected = """
        {
            "key1": 10,
            "key2": "val2",
            "key3": [5, 10, 15, 20],
            "key4": 0.5,
            "key5": 10E-10,
            "key6": {
                "key11": "test",
                "key12": "email.address@gmail.com",
                "key13": ["Microsoft", "https://www.microsoft.com/en-us/"],
                "key14": [10, 20, 30, 40]
            },
            "key7": true,
            "key8": ["ABC", "EFG", "XYZ"],
            "key9": null
        }
        """;
        var actual = """
        {
            "key1": 10,
            "key2": "val2",
            "key3": [5, 10, 15, 20],
            "key4": 0.5,
            "key5": 10E-10,
            "key6": {
                "key11": "test",
                "key12": "email.address@gmail.com",
                "key13": ["Microsoft", "https://www.microsoft.com/en-us/"],
                "key14": [10, 20, 30, 40]
            },
            "key7": true,
            "key8": ["ABC", "EFG", "XYZ"],
            "key9": null
        }
        """;
        JsonAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void When_JsonAggregatedFormatTest_ValidTrue()
    {
        var expected = """
        {
            "key1" : 10,
            "key2" : "val2",
            "key3" : [5, 10, 15, 20],
            "key4" : 0.5,
            "key5" : 10E-10,
            "key6" : {
                "key11" : "test",
                "key12" : "email.address@gmail.com",
                "key13" : [ "Microsoft", "https://www.microsoft.com/en-us/" ],
                "key14" : [ 10, 20, 30, 40 ]
            },
            "key7" : true,
            "key8" : [ "ABC", "EFG", "XYZ" ],
            "key9" : null
        }
        """;
        var actual = """
        {
            "key3": [5, 10, 15, 20],
            "key1": 10, "key2": "val2",
            "key4": 0.5, "key5": 10E-10,
            "key6": {
                "key12": "email.address@gmail.com",
                "key13": ["Microsoft", "https://www.microsoft.com/en-us/"],
                "key11": "test", "key14": [10, 20, 30, 40]
            },
            "key8": ["ABC", "EFG", "XYZ"], "key7": true, "key9": null
        }
        """;
        JsonAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void When_SimpleJsonSchemaAggregatedTest_ValidTrue()
    {
        var schema = """
        %title: "User Profile Response"
        %version: "1.0.0-basic"
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
        """;
        var json = """
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
        """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_ExtendedJsonSchemaAggregatedTest_ValidTrue()
    {
        var schema = """
        %title: "Extended User Profile Dashboard API Response"
        %version: "2.0.0-extended"
        %import: RelogicLabs.JSchema.Tests.External.ExternalFunctions,
                 RelogicLabs.JSchema.Tests
        
        %pragma DateDataTypeFormat: "DD-MM-YYYY"
        %pragma TimeDataTypeFormat: "DD-MM-YYYY hh:mm:ss"
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
                "registeredAt": @after("01-01-2010 00:00:00") #time,
                "dataAccess": @checkAccess(&role) #integer,
                "profile": {
                    "firstName": @regex("[A-Za-z]{3,50}") #string,
                    "lastName": @regex("[A-Za-z]{3,50}") #string,
                    "dateOfBirth": @before("01-01-2006") #date,
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
                "role": "admin",
                "isActive": true,
                "registeredAt": "06-09-2023 15:10:30",
                "dataAccess": 10,
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
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_ExtendedJsonSchemaWithScriptAggregatedTest_ValidTrue()
    {
        var schema = """
        %title: "Extended User Profile Dashboard API Response"
        %version: "2.0.0-extended"
        
        %pragma DateDataTypeFormat: "DD-MM-YYYY"
        %pragma TimeDataTypeFormat: "DD-MM-YYYY hh:mm:ss"
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
                "registeredAt": @after("01-01-2010 00:00:00") #time,
                "dataAccess": @checkAccess(&role) #integer,
                "profile": {
                    "firstName": @regex("[A-Za-z]{3,50}") #string,
                    "lastName": @regex("[A-Za-z]{3,50}") #string,
                    "dateOfBirth": @before("01-01-2006") #date,
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
        
        %script: {
            future checkAccess(role) {
                if(role[0] == "user" && target > 5) return fail(
                    "ERRACCESS", "Data access incompatible with 'user' role",
                    expected("an access at most 5 for 'user' role"),
                    actual("found access " + target + " which is greater than 5"));
                return true; // Skipping this line also works
            }
        }
        """;
        var json = """
        {
            "user": {
                "id": 1234,
                "username": "johndoe",
                "role": "admin",
                "isActive": true,
                "registeredAt": "06-09-2023 15:10:30",
                "dataAccess": 10,
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
                        "content": "C# provides great support for working with JSON...",
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
                    "price": 1.99,
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
                "isCloudy": true,
                "conditions": null
            }
        }
        """;
        JsonAssert.IsValid(schema, json);
    }
}