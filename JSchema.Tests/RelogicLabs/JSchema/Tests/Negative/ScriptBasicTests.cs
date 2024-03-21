using RelogicLabs.JSchema.Exceptions;
using static RelogicLabs.JSchema.Message.ErrorCode;

namespace RelogicLabs.JSchema.Tests.Negative;

[TestClass]
public class ScriptBasicTests
{
    [TestMethod]
    public void When_DuplicateVariableDefinition_ExceptionThrown()
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
                        if(test != 30) return fail("Invalid: " + test);
                    }
                    if(test != 10) return fail("Invalid: " + test);
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
                "indexTest1": @indexArrayTest #array,
                "indexTest2": @indexStringTest #string
            }
            %script: {
                constraint indexArrayTest() {
                    var result1 = tryof(target[10]);
                    if(find(result1.error, "[INDX04]") < 0)
                        return fail("Invalid: " + result1);
                    var result2 = tryof(target[-1]);
                    if(find(result2.error, "[INDX05]") < 0)
                        return fail("Invalid: " + result2);
                    var result3 = tryof(target[10..]);
                    if(find(result3.error, "[RNGS07]") < 0)
                        return fail("Invalid: " + result3);
                    var result4 = tryof(target[..10]);
                    if(find(result4.error, "[RNGS08]") < 0)
                        return fail("Invalid: " + result4);
                    var result5 = tryof(target[-2..-4]);
                    if(find(result5.error, "[RNGS09]") < 0)
                        return fail("Invalid: " + result5);
                    var array = [10, 20];
                    // only assign at the end of array to add
                    // use fill function for array of specific size
                    var result6 = tryof(array[10] = 10);
                    if(find(result6.error, "[INDX04]") < 0)
                        return fail("Invalid: " + result6);
                }
                
                constraint indexStringTest() {
                    var result1 = tryof(target[100]);
                    if(find(result1.error, "[INDX02]") < 0)
                        return fail("Invalid: " + result1);
                    var result2 = tryof(target[-1]);
                    if(find(result2.error, "[INDX03]") < 0)
                        return fail("Invalid: " + result2);
                    var result3 = tryof(target[100..]);
                    if(find(result3.error, "[RNGS04]") < 0)
                        return fail("Invalid: " + result3);
                    var result4 = tryof(target[..100]);
                    if(find(result4.error, "[RNGS05]") < 0)
                        return fail("Invalid: " + result4);
                    var result5 = tryof(target[8..6]);
                    if(find(result5.error, "[RNGS06]") < 0)
                        return fail("Invalid: " + result5);
                }
            }
            """;
        var json =
            """
            {
                "indexTest1": [1, 2, 3, 4, 5, 6],
                "indexTest2": "This is a test"
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
                "operationTest": @operationTest #object
            }
            %script: {
                constraint operationTest() {
                    var result1 = tryof(target + 10);
                    if(find(result1.error, "[ADDI01]") < 0)
                        return fail("Invalid: " + result1);
                    var result2 = tryof(target - 10);
                    if(find(result2.error, "[SUBT01]") < 0)
                        return fail("Invalid: " + result2);
                    var result3 = tryof(target * 10);
                    if(find(result3.error, "[MULT01]") < 0)
                        return fail("Invalid: " + result3);
                    var result4 = tryof(target / 10);
                    if(find(result4.error, "[DIVD01]") < 0)
                        return fail("Invalid: " + result4);
                    var result5 = tryof(target > 10);
                    if(find(result5.error, "[RELA01]") < 0)
                        return fail("Invalid: " + result5);
                    var result6 = tryof(target >= 10);
                    if(find(result6.error, "[RELA02]") < 0)
                        return fail("Invalid: " + result6);
                    var result7 = tryof(target < 10);
                    if(find(result7.error, "[RELA03]") < 0)
                        return fail("Invalid: " + result7);
                    var result8 = tryof(target <= 10);
                    if(find(result8.error, "[RELA04]") < 0)
                        return fail("Invalid: " + result8);
                }
            }
            """;
        var json =
            """
            {
                "operationTest": {}
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
                "incDecTest1": @incDecTest1 #object,
                "incDecTest2": @incDecTest1 #array
            }
            %script: {
                constraint incDecTest1() {
                    var t = target;
                    var result1 = tryof(t++);
                    if(find(result1.error, "[INCR02]") < 0)
                        return fail("Invalid: " + result1);
                    var result2 = tryof(++t);
                    if(find(result2.error, "[INCR04]") < 0)
                        return fail("Invalid: " + result2);
                    var result3 = tryof(t--);
                    if(find(result3.error, "[DECR02]") < 0)
                        return fail("Invalid: " + result3);
                    var result4 = tryof(--t);
                    if(find(result4.error, "[DECR04]") < 0)
                        return fail("Invalid: " + result4);
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
    public void When_ReadonlyLValueForOperations_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "lvalueTest1": @lvalueTest1 #object,
                "lvalueTest2": @lvalueTest2 #array
            }
            %script: {
                constraint lvalueTest1() {
                    var result1 = tryof(target.test = 10);
                    if(find(result1.error, "[PRPS02]") < 0)
                        return fail("Invalid: " + result1);
                    var result2 = tryof(target.k1 = 10);
                    if(find(result2.error, "[ASIN01]") < 0)
                        return fail("Invalid: " + result2);
                }
                
                constraint lvalueTest2() {
                    var result1 = tryof(target[2] = 10);
                    if(find(result1.error, "[INDX04]") < 0)
                        return fail("Invalid: " + result1);
                    var result2 = tryof(target[0] = 10);
                    if(find(result2.error, "[ASIN01]") < 0)
                        return fail("Invalid: " + result2);
                }
            }
            """;
        var json =
            """
            {
                "lvalueTest1": {"k1": 100},
                "lvalueTest2": [100, 200]
            }
            """;
        //JsonSchema.IsValid(schema, json);
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_WrongLValueForPropertyAndIndexRangeOperations_ExceptionThrown()
    {
        var schema =
            """
            %schema:
            {
                "lvalueTest1": @lvalueTest1 #array,
                "lvalueTest2": @lvalueTest2 #object,
                "lvalueTest3": @lvalueTest3 #integer
            }
            %script: {
                constraint lvalueTest1() {
                    var result1 = tryof(target.test);
                    if(find(result1.error, "[PRPS01]") < 0)
                        return fail("Invalid: " + result1);
                    var result2 = tryof(target["test"]);
                    if(find(result2.error, "[INDX07]") < 0)
                        return fail("Invalid: " + result2);
                }
                
                constraint lvalueTest2() {
                    var result1 = tryof(target[0]);
                    if(find(result1.error, "[INDX06]") < 0)
                        return fail("Invalid: " + result1);
                    var result2 = tryof(target[0..]);
                    if(find(result2.error, "[RNGS10]") < 0)
                        return fail("Invalid: " + result2);
                }
                
                constraint lvalueTest3() {
                    var result1 = tryof(target.test);
                    if(find(result1.error, "[PRPS01]") < 0)
                        return fail("Invalid: " + result1);
                    var result2 = tryof(target[0]);
                    if(find(result2.error, "[INDX06]") < 0)
                        return fail("Invalid: " + result2);
                    var result3 = tryof(target[0..]);
                    if(find(result3.error, "[RNGS10]") < 0)
                        return fail("Invalid: " + result3);
                }
            }
            """;
        var json =
            """
            {
                "lvalueTest1": [10, 20],
                "lvalueTest2": { "k1": 10 },
                "lvalueTest3": 10
            }
            """;
        //JsonSchema.IsValid(schema, json);
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_RuntimeSystemException_ExceptionThrown()
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