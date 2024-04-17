using RelogicLabs.JSchema.Exceptions;
using static RelogicLabs.JSchema.Message.ErrorCode;

namespace RelogicLabs.JSchema.Tests.Negative;

[TestClass]
public class ScriptBasicTests
{
    [TestMethod]
    public void When_DuplicateVariableDeclaration_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "variableTest": @variableTest #integer
            }
            %script: {
                constraint variableTest() {
                    var test = 10;
                    if(true) {
                        // Variable shadowing in inner scope
                        var test = 30;
                        if(test != 30) throw("Invalid: " + test);
                    }
                    if(test != 10) throw("Invalid: " + test);
                    // Variable 'test' already defined in this scope
                    var test = 20;
                }
            }
            """;
        var json =
            """
            {
                "variableTest": 2
            }
            """;
        JsonSchema.IsValid(schema, json);
        // throw only causes ScriptInitiatedException
        var exception = Assert.ThrowsException<ScriptRuntimeException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(VARD01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_ForeachWithNonIterableValue_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "iteratorTest": @iteratorTest #integer
            }
            %script: {
                constraint iteratorTest() {
                    foreach(var i in target) print(i);
                }
            }
            """;
        var json =
            """
            {
                "iteratorTest": 2
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<ScriptRuntimeException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(ITER01, exception.Code);
        Console.WriteLine(exception);
    }

    [TestMethod]
    public void When_WrongIndexAndRangeOutOfBounds_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "stringTest": @stringTest #string,
                "arrayTest": @arrayTest #array
            }
            %script: {
                constraint stringTest() {
                    var result1 = tryof(target[100]);
                    if(!result1.error.find("[SIDX01]")) throw("Invalid: " + target);
                    var result2 = tryof(target[-1]);
                    if(!result2.error.find("[SIDX02]")) throw("Invalid: " + target);
                    var result3 = tryof(target[100..]);
                    if(!result3.error.find("[SRNG01]")) throw("Invalid: " + target);
                    var result4 = tryof(target[..100]);
                    if(!result4.error.find("[SRNG02]")) throw("Invalid: " + target);
                    var result5 = tryof(target[8..6]);
                    if(!result5.error.find("[SRNG03]")) throw("Invalid: " + target);
                }

                constraint arrayTest() {
                    var result1 = tryof(target[10]);
                    if(!result1.error.find("[AIDX01]")) throw("Invalid: " + target);
                    var result2 = tryof(target[-1]);
                    if(!result2.error.find("[AIDX02]")) throw("Invalid: " + target);
                    var result3 = tryof(target[10..]);
                    if(!result3.error.find("[ARNG01]")) throw("Invalid: " + target);
                    var result4 = tryof(target[..10]);
                    if(!result4.error.find("[ARNG02]")) throw("Invalid: " + target);
                    var result5 = tryof(target[-2..-4]);
                    if(!result5.error.find("[ARNG03]")) throw("Invalid: " + target);
                    var array = [0, 1];
                    // only assign at the end of array to add
                    // use fill method for array of specific size
                    var result6 = tryof(array[10] = 10);
                    if(!result6.error.find("[AWRT01]")) throw("Invalid: " + array);
                }
            }
            """;
        var json =
            """
            {
                "stringTest": "This is a test",
                "arrayTest": [1, 2, 3, 4, 5, 6]
            }
            """;
        //JsonSchema.IsValid(schema, json);
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_WrongTypeForDifferentOperations_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "operationTest1": @unaryOperation #array,
                "operationTest2": @binaryOperation #object,
                "operationTest3": @comparisonOperation #string
            }
            %script: {
                constraint unaryOperation() {
                    var result1 = tryof(+target);
                    if(!result1.error.find("[PLUS01]")) throw("Invalid: " + target);
                    var result2 = tryof(-target);
                    if(!result2.error.find("[MINS01]")) throw("Invalid: " + target);
                }

                constraint binaryOperation() {
                    var result1 = tryof(target + 10);
                    if(!result1.error.find("[ADDT01]")) throw("Invalid: " + target);
                    var result2 = tryof(target - 10);
                    if(!result2.error.find("[SUBT01]")) throw("Invalid: " + target);
                    var result3 = tryof(target * 10);
                    if(!result3.error.find("[MULT01]")) throw("Invalid: " + target);
                    var result4 = tryof(target / 10);
                    if(!result4.error.find("[DIVD01]")) throw("Invalid: " + target);
                    var result5 = tryof(target % 10);
                    if(!result5.error.find("[MODU01]")) throw("Invalid: " + target);
                }

                constraint comparisonOperation() {
                    var result1 = tryof(target > 10);
                    if(!result1.error.find("[RELA01]")) throw("Invalid: " + target);
                    var result2 = tryof(target >= 10);
                    if(!result2.error.find("[RELA02]")) throw("Invalid: " + target);
                    var result3 = tryof(target < 10);
                    if(!result3.error.find("[RELA03]")) throw("Invalid: " + target);
                    var result4 = tryof(target <= 10);
                    if(!result4.error.find("[RELA04]")) throw("Invalid: " + target);
                }
            }
            """;
        var json =
            """
            {
                "operationTest1": [10, 20],
                "operationTest2": {"k1": 10},
                "operationTest3": "test"
            }
            """;
        //JsonSchema.IsValid(schema, json);
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_WrongLValueForIncrementDecrement_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "incDecTest1": @incDecTest #object,
                "incDecTest2": @incDecTest #array
            }
            %script: {
                constraint incDecTest() {
                    var t = target;
                    var result1 = tryof(t++);
                    if(!result1.error.find("[INCT02]")) throw("Invalid: " + target);
                    var result2 = tryof(++t);
                    if(!result2.error.find("[INCE02]")) throw("Invalid: " + target);
                    var result3 = tryof(t--);
                    if(!result3.error.find("[DECT02]")) throw("Invalid: " + target);
                    var result4 = tryof(--t);
                    if(!result4.error.find("[DECE02]")) throw("Invalid: " + target);
                }
            }
            """;
        var json =
            """
            {
                "incDecTest1": {},
                "incDecTest2": []
            }
            """;
        //JsonSchema.IsValid(schema, json);
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_ReadonlyLValueForUpdateOperations_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "lvalueTest1": @lvalueTest1 #array,
                "lvalueTest2": @lvalueTest2 #object,
                "lvalueTest3": @lvalueTest3 #any
            }
            %script: {
                constraint lvalueTest1() {
                    var result1 = tryof(target[2] = 10);
                    if(!result1.error.find("[AUPD01]")) throw("Invalid: " + target);
                    var result2 = tryof(target[0] = 10);
                    if(!result2.error.find("[AUPD01]")) throw("Invalid: " + target);
                }

                constraint lvalueTest2() {
                    var result1 = tryof(target.test = 10);
                    if(!result1.error.find("[OUPD01]")) throw("Invalid: " + target);
                    var result2 = tryof(target.k1 = 10);
                    if(!result2.error.find("[OUPD01]")) throw("Invalid: " + target);
                }

                constraint lvalueTest3() {
                    var result1 = tryof(target[1] = "a");
                    if(!result1.error.find("[SASN01]")) throw("Invalid: " + target);
                    var result2 = tryof(target[11] = "a");
                    if(!result2.error.find("[SASN01]")) throw("Invalid: " + target);
                }
            }
            """;
        var json =
            """
            {
                "lvalueTest1": [100, 200],
                "lvalueTest2": {"k1": 100},
                "lvalueTest3": "test string"
            }
            """;
        //JsonSchema.IsValid(schema, json);
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_WrongLValueForDifferentOperations_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "lvalueTest1": @lvalueTest1 #array,
                "lvalueTest2": @lvalueTest2 #object,
                "lvalueTest3": @lvalueTest3 #integer,
                "lvalueTest4": @lvalueTest4 #any
            }
            %script: {
                constraint lvalueTest1() {
                    var result1 = tryof(target.test);
                    if(!result1.error.find("[PRPT01]")) throw("Invalid: " + target);
                    var result2 = tryof(target["test"]);
                    if(!result2.error.find("[BKTR01]")) throw("Invalid: " + target);
                }

                constraint lvalueTest2() {
                    var result1 = tryof(target[0]);
                    if(!result1.error.find("[IDXR01]")) throw("Invalid: " + target);
                    var result2 = tryof(target[0..]);
                    if(!result2.error.find("[RNGR01]")) throw("Invalid: " + target);
                }

                constraint lvalueTest3() {
                    var result1 = tryof(target.test);
                    if(!result1.error.find("[PRPT01]")) throw("Invalid: " + target);
                    var result2 = tryof(target[0]);
                    if(!result2.error.find("[IDXR01]")) throw("Invalid: " + target);
                    var result3 = tryof(target[0..]);
                    if(!result3.error.find("[RNGR01]")) throw("Invalid: " + target);
                    var result4 = tryof(target[0] = 0);
                    if(!result4.error.find("[IDXA01]")) throw("Invalid: " + target);
                    var result5 = tryof(target[0..] = 10);
                    if(!result5.error.find("[RNGA01]")) throw("Invalid: " + target);
                }

                constraint lvalueTest4() {
                    var string = "string is an immutable value type";
                    var result1 = tryof(string[1] = "a");
                    if(!result1.error.find("[SASN01]")) throw("Invalid: " + string);
                    var array = [10, 20, 30, 40];
                    // an array range update is not supported
                    var result2 = tryof(array[1..3]++);
                    if(!result2.error.find("[ARUD01]")) throw("Invalid: " + array);
                    var result3 = tryof(array[1..3] = "test");
                    if(!result3.error.find("[ARAS01]")) throw("Invalid: " + array);
                }
            }
            """;
        var json =
            """
            {
                "lvalueTest1": [10, 20],
                "lvalueTest2": { "k1": 10 },
                "lvalueTest3": 10,
                "lvalueTest4": ""
            }
            """;
        //JsonSchema.IsValid(schema, json);
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_InvalidRangeSetupForWrongBoundaryValue_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "testRangeSetup": @testRangeSetup #string
            }
            %script: {
                constraint testRangeSetup() {
                    var result1 = tryof(target..);
                    if(!result1.error.find("[RNGT01]")) throw("Invalid: " + target);
                    var result2 = tryof(5..target);
                    if(!result2.error.find("[RNGT03]")) throw("Invalid: " + target);
                    var result3 = tryof(..target);
                    if(!result3.error.find("[RNGT05]")) throw("Invalid: " + target);
                }
            }
            """;
        var json =
            """
            {
                "testRangeSetup": "This is a string"
            }
            """;
        //JsonSchema.IsValid(schema, json);
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_RuntimeSystemOperationException_ExceptionThrown()
    {
        // Exception like OutOfMemoryError / DivideByZero etc.
        var schema =
            """
            %schema:
            {
                "exceptionTest": @exceptionTest #integer
            }
            %script: {
                constraint exceptionTest() {
                    var result = target / 0;
                }
            }
            """;
        var json =
            """
            {
                "exceptionTest": 10
            }
            """;
        JsonSchema.IsValid(schema, json);
        var exception = Assert.ThrowsException<SystemOperationException>(
            () => JsonAssert.IsValid(schema, json));
        Assert.AreEqual(DIVD02, exception.Code);
        Console.WriteLine(exception);
    }
}