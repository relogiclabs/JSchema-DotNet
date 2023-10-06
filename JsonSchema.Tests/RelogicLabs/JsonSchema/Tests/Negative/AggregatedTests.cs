using RelogicLabs.JsonSchema.Exceptions;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

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
}