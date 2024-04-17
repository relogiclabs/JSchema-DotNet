using RelogicLabs.JSchema.Exceptions;
using static RelogicLabs.JSchema.Message.ErrorCode;

namespace RelogicLabs.JSchema.Tests.Negative;

[TestClass]
public class FunctionTests
{

    [TestMethod]
    public void When_FunctionAppliedOnWrongType_ExceptionThrown()
    {
        var schema =
            """
            %schema: @range(10, 20)
            """;
        var json =
            """
            "test"
            """;

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(FUNC03, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_ExternalImportNotInheritBaseClass_ExceptionThrown()
    {
        var schema =
            """
            %import: RelogicLabs.JSchema.Tests.External.ExternalFunctions1,
                     RelogicLabs.JSchema.Tests
            %schema: @odd #integer
            """;
        var json = "10";

        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<InvalidImportException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(CLAS03, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_ExternalImportNotExisting_ExceptionThrown()
    {
        var schema =
            """
            %import: RelogicLabs.JSchema.Tests.NotExisting, RelogicLabs.JSchema.Tests
            %schema: @odd #integer
            """;
        var json = "10";

        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<ClassNotFoundException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(CLAS02, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_ExternalImportDuplicationOccurred_ExceptionThrown()
    {
        var schema =
            """
            %import: RelogicLabs.JSchema.Tests.External.ExternalFunctions,
            RelogicLabs.JSchema.Tests
            %import: RelogicLabs.JSchema.Tests.External.ExternalFunctions,
            RelogicLabs.JSchema.Tests
            %schema: @odd #integer
            """;
        var json = "10";

        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<DuplicateImportException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(CLAS01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_ExternalImportInstantiationNotSuccessful_ExceptionThrown()
    {
        var schema =
            """
            %import: RelogicLabs.JSchema.Tests.External.ExternalFunctions5,
                     RelogicLabs.JSchema.Tests
            %schema: @odd #integer
            """;
        var json = "10";

        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<ClassInstantiationException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(INST01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_ExternalFunctionWrongReturnType_ExceptionThrown()
    {
        var schema =
            """
            %import: RelogicLabs.JSchema.Tests.External.ExternalFunctions2,
                     RelogicLabs.JSchema.Tests
            %schema: @odd #integer
            """;
        var json = "10";

        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<InvalidFunctionException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(FUNC01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_ExternalFunctionWrongParameterNumber_ExceptionThrown()
    {
        var schema =
            """
            %import: RelogicLabs.JSchema.Tests.External.ExternalFunctions3,
                     RelogicLabs.JSchema.Tests
            %schema: @odd #integer
            """;
        var json = "10";

        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<InvalidFunctionException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(FUNC02, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_ExternalFunctionNotExists_ExceptionThrown()
    {
        var schema =
            """
            %import: RelogicLabs.JSchema.Tests.External.ExternalFunctions4,
                     RelogicLabs.JSchema.Tests
            %schema: @notExist #integer
            """;
        var json = "10";

        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<FunctionNotFoundException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(FUNC05, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_FunctionThrowArbitraryException_ExceptionThrown()
    {
        var schema =
            """
            %import: RelogicLabs.JSchema.Tests.External.ExternalFunctions4,
                     RelogicLabs.JSchema.Tests
            %schema: @canTest #integer
            """;
        var json = "10";

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<Exception>(
            () => JsonAssert.IsValid(schema, json));
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_IncompatibleTargetForExternalFunction_ExceptionThrown()
    {
        var schema =
            """
            %import: RelogicLabs.JSchema.Tests.External.ExternalFunctions,
                     RelogicLabs.JSchema.Tests
            %schema: @even #string
            """;
        var json = "\"test\"";

        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<JsonSchemaException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(FUNC03, exception.Code);
        Console.WriteLine(exception);
    }
}