using RelogicLabs.JSchema.Exceptions;
using static RelogicLabs.JSchema.Message.ErrorCode;

namespace RelogicLabs.JSchema.Tests.Negative;

[TestClass]
public class ScriptFunctionTests
{
    [TestMethod]
    public void When_ConstraintFunctionNotExists_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "funcTest": @funcTest #integer
            }
            """;
        var json =
            """
            {
                "funcTest": 2
            }
            """;
        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<FunctionNotFoundException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(FUNC05, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_SubroutineFunctionNotExists_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "funcTest": @funcTest #integer
            }
            
            %script: {
                constraint funcTest() {
                    subroutineFunction(target);
                }
            }
            """;
        var json =
            """
            {
                "funcTest": 2
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<ScriptRuntimeException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(FUNS04, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_SubroutineVariadicCallWithoutRequiredArguments_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "funcTest": @funcTest #integer
            }
            
            %script: {
                constraint funcTest() {
                    testFunction(10);
                }
                
                subroutine function testFunction(p1, p2, p3...) {
                    return false;
                }
            }
            """;
        var json =
            """
            {
                "funcTest": 2
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<ScriptRuntimeException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(FUNS05, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidReturnTypeOfConstraint_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "funcTest": @funcTest #integer
            }
            %script: {
                // constraint functions are available to schema context
                // subroutine functions are not available to schema context
                constraint function funcTest() {
                    return 10;
                }
            }
            """;
        var json =
            """
            {
                "funcTest": 2
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<ScriptRuntimeException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(RETN01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_MultipleVariadicSubroutineWithSameName_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "funcTest": @funcTest #integer
            }
            %script: {
                constraint funcTest() {
                    return true;
                }
                
                // Regardless of required params only one variadic subroutine
                // can be overloaded with the same name but any number of
                // fixed params subroutine can be overload with the same name
                subroutine testFunction(p1, p2, p3...) {
                    return false;
                }
                
                subroutine testFunction(p1, p2...) {
                    return false;
                }
            }
            """;
        var json =
            """
            {
                "funcTest": 2
            }
            """;
        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<ScriptRuntimeException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(FUNS03, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidArgumentTypeWithNativeSubroutine_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "funcTest": @funcTest
            }
            %script: {
                constraint funcTest() {
                    // type can be checked using the type function
                    if(!regular(target)) return fail("Invalid: " + target);
                    var result = find(target, 10);
                }
            }
            """;
        var json =
            """
            {
                "funcTest": 2
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<ScriptRuntimeException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(FIND02, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_InvalidArgumentValueWithNativeSubroutine_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "funcTest": @funcTest
            }
            %script: {
                constraint funcTest() {
                    return fail("ERR01", "Test Message", { node: null }, {});
                }
            }
            """;
        var json =
            """
            {
                "funcTest": 2
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<ScriptRuntimeException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(FAIL09, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_ScriptInitiatedCallNotFoundTargetNode_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "targetTest": #integer
            }
            %script: {
                // target is available inside subroutine from call stack
                // when call initiated from schema
                subroutine targetTest() {
                    var test = target;
                    return 10;
                }
                var test = targetTest();
            }
            """;
        var json =
            """
            {
                "targetTest": 2
            }
            """;
        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<ScriptRuntimeException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(TRGT01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_ScriptInitiatedCallNotFoundCallerNode_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "callerTest": #integer
            }
            %script: {
                // caller is available inside subroutine from call stack
                // when call initiated from schema
                subroutine callerTest() {
                    var test = caller;
                    return 10;
                }
                var test = callerTest();
            }
            """;
        var json =
            """
            {
                "callerTest": 2
            }
            """;
        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<ScriptRuntimeException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(CALR01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_DuplicateConstraintDefinitionsConflict_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "funcTest": @funcTest(5)
            }
            %script: {
                constraint funcTest(param1) {
                    return true;
                }
                
                constraint funcTest(param1) {
                    return true;
                }
            }
            """;
        var json =
            """
            {
                "funcTest": 10
            }
            """;
        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<ScriptRuntimeException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(FUNS02, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_DuplicateConstraintDefinitionsWithFutureConflict_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "funcTest": @funcTest(5)
            }
            %script: {
                constraint funcTest(param1) {
                    return true;
                }
                
                // future functions are also constraint functions
                future constraint funcTest(param1) {
                    return true;
                }
            }
            """;
        var json =
            """
            {
                "funcTest": 10
            }
            """;
        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<ScriptRuntimeException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(FUNS02, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_DuplicateSubroutineDefinitionsConflict_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "funcTest": @funcTest(5)
            }
            %script: {
                constraint funcTest(param1) {
                    return true;
                }
                
                // Constraint functions are special functions and are not callable
                // from script thus preventing any conflict with subroutines
                subroutine funcTest(param1) {
                    return true;
                }
                
                subroutine funcTest(param1) {
                    return true;
                }
            }
            """;
        var json =
            """
            {
                "funcTest": 10
            }
            """;
        //JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<ScriptRuntimeException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(FUNS03, exception.Code);
        Console.WriteLine(exception);
    }
}