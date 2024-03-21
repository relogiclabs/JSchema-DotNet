using RelogicLabs.JSchema.Exceptions;

namespace RelogicLabs.JSchema.Tests.Negative;

[TestClass]
public class ScriptGeneralTests
{
    [TestMethod]
    public void When_WrongValueConditionUnsatisfiedV1_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "role": #string &role,
                "dataAccess": @checkAccess(&role) #integer
            }
            %script: {
                future constraint checkAccess(role) {
                    if(role[0] == "user" && target > 5) return fail(
                        "ERRACCESS01", "Data access incompatible with 'user' role",
                        // 'caller' is the default node added automatically
                        expected("an access at most 5 for 'user' role"),
                        // 'target' is the default node added automatically
                        actual("found access " + target + " which is greater than 5"));
                }
            }
            """;
        var json =
            """
            {
                "role": "user",
                "dataAccess": 6
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual("ERRACCESS01", exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_WrongValueConditionUnsatisfiedV2_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "role": #string &role,
                "dataAccess": @checkAccess(&role) #integer
            }
            %script: {
                future constraint checkAccess(role) {
                    if(role[0] == "user" && target > 5) return fail(
                        "ERRACCESS01", "Data access incompatible with 'user' role",
                        // Pass any node explicitly to the expected function
                        expected(caller, "an access at most 5 for 'user' role"),
                        // Pass any node explicitly to the actual function
                        actual(target, "found access " + target + " which is greater than 5"));
                }
            }
            """;
        var json =
            """
            {
                "role": "user",
                "dataAccess": 7
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual("ERRACCESS01", exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_WrongValueConditionUnsatisfiedV3_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "role": #string &role,
                "dataAccess": @checkAccess(&role) #integer
            }
            %script: {
                constraint checkAccess(role) {
                    if(role[0] == "user" && target > 5) return fail(
                        "ERRACCESS01", "Data access incompatible with 'user' role",
                        // Create an expected object explicitly without any function
                        { node: caller, message: "an access at most 5 for 'user' role" },
                        // Create an actual object explicitly without any function
                        { node: target, message: "found access " + target
                                + " which is greater than 5" });
                }
            }
            """;
        var json =
            """
            {
                "role": "user",
                "dataAccess": 8
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual("ERRACCESS01", exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_WrongValueConditionUnsatisfiedV4_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "role": #string &role,
                "dataAccess": @checkAccess(&role) #integer
            }
            %script: {
                future constraint checkAccess(role) {
                    // Fail with simple message and a code
                    if(role[0] == "user" && target > 5) return fail(
                        "ERRACCESS01", "Data access incompatible with 'user' role");
                }
            }
            """;
        var json =
            """
            {
                "role": "user",
                "dataAccess": 9
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<ScriptInitiatedException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual("ERRACCESS01", exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_WrongValueConditionUnsatisfiedV5_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "role": #string &role,
                "dataAccess": @checkAccess(&role) #integer
            }
            %script: {
                future constraint checkAccess(role) {
                    // Fail with just a message
                    if(role[0] == "user" && target > 5) return fail(
                        "Data access incompatible with 'user' role");
                }
            }
            """;
        var json =
            """
            {
                "role": "user",
                "dataAccess": 11
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<ScriptInitiatedException>(
            () => JsonAssert.IsValid(schema, json));
        // FAIL01 is the default code if no code provided
        Assert.AreEqual("FAIL01", exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_ThrowExceptionFromScriptV1_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "throwTest": @throwTest #array
            }
            %script: {
                future constraint throwTest() {
                    if(type(target) != "#array") return fail("Invalid: " + target);
                    if(find(target, 45.5) < 0) return fail("Invalid: " + target);
                    if(target[1] == 20) throw("ERROR01", "Throw test with code");
                    return fail("NOTTHRO01", "Exception not thrown");
                }
            }
            """;
        var json =
            """
            {
                "throwTest": [10, 20, 30, 45.5]
            }
            """;
        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<ScriptInitiatedException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual("ERROR01", exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_ThrowExceptionFromScriptV2_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "throwTest": @throwTest #array
            }
            %script: {
                future constraint throwTest() {
                    var c1 = copy(target[0]);
                    var c2 = copy(target[3]);
                    if(c1 != 10) return fail("Invalid: " + target);
                    if(c2 != 45.5) return fail("Invalid: " + target);
                    if(target[2] == 30) throw("Throw test without code");
                    return fail("NOTTHRO01", "Exception not thrown");
                }
            }
            """;
        var json =
            """
            {
                "throwTest": [10, 20, 30, 45.5]
            }
            """;
        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<ScriptInitiatedException>(
            () => JsonAssert.IsValid(schema, json));
        // THRO01 is the default code if no code provided
        Assert.AreEqual("THRO01", exception.Code);
        Console.WriteLine(exception);
    }
}