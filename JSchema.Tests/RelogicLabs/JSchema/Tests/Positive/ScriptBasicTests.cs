namespace RelogicLabs.JSchema.Tests.Positive;

[TestClass]
public class ScriptBasicTests
{
    [TestMethod]
    public void When_ArithmeticIntegerExpression_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "exprTest": @exprTest #integer
            }
            %script: {
                constraint exprTest() {
                    var num = 2;
                    var result = num + 3 * (2 + 1) - 8 / 2;
                    result = -result;
                    if(result == -target) return true;
                    return fail("Invalid: " + target);
                }
            }
            """;
        var json =
            """
            {
                "exprTest": 7
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_ArithmeticDoubleExpression_ValidTrue()
    {
        var schema =
            """
            %script: {
                constraint exprTest() {
                    var num = 3.5;
                    var result = 10.1 - (3.5 - 1.2) * 16.1 + 45.15 / num;
                    if(-result == target) return true;
                    return fail("Invalid: " + target);
                }
            }
            %schema:
            {
                "exprTest": @exprTest #float #double
            }
            """;
        var json =
            """
            {
                "exprTest": 14.03
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_FindSecondOccurrenceInString_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "stringTest": @stringTest #string
            }
            %script: {
                constraint stringTest() {
                    var result = find(target, "Ipsum");
                    result = find(target, "Ipsum", result + 1);
                    print("Second Occurrence at: " + result);
                    if(result != 12) return fail("Invalid: " + target);
                }
            }
            """;
        var json =
            """
            {
                "stringTest": "Lorem Ipsum Ipsum"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_FindSecondOccurrenceInArray_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "arrayTest": @arrayTest #array
            }
            %script: {
                constraint arrayTest() {
                    var result = find(target, 10);
                    result = find(target, 10, result + 1);
                    print("Second Occurrence at: " + result);
                    if(result != 4) return fail("Invalid: " + target);
                }
            }
            """;
        var json =
            """
            {
                "arrayTest": [5, 10, 20, 30, 10, 50, 80]
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_ConcatWithStringAutoConvertToString_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "stringTest": @stringTest #string
            }
            %script: {
                constraint stringTest() {
                    if(type(target) != "#string") return fail("Invalid: " + target);
                    var result = "Lorem " + "Ipsum" + 10 + [1, 20] + {k1: 100};
                    if(result != target) return fail("Invalid: " + target);
                }
            }
            """;
        var json =
            """
            {
                "stringTest": "Lorem Ipsum10[1, 20]{\"k1\": 100}"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_ComparisonOperationWithNumber_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "comparisonTest": @comparisonTest #float
            }
            %script: {
                constraint comparisonTest() {
                    if(!(target >= 10.1)) return fail("Invalid: " + target);
                    if(!(target >= 10)) return fail("Invalid: " + target);
                    if(!(target <= 10.1)) return fail("Invalid: " + target);
                    if(!(target <= 11)) return fail("Invalid: " + target);
                    if(!(target > 10.01)) return fail("Invalid: " + target);
                    if(!(target > 10)) return fail("Invalid: " + target);
                    if(!(target < 10.11)) return fail("Invalid: " + target);
                    if(!(target < 11)) return fail("Invalid: " + target);
                    if(target != 10.1) return fail("Invalid: " + target);
                    if(target == 10) return fail("Invalid: " + target);
                }
            }
            """;
        var json =
            """
            {
                "comparisonTest": 10.1
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_TryofWithSuccessAndFailOperations_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "tryofTest": @tryofTest
            }
            %script: {
                constraint tryofTest() {
                    var result1 = tryof(target.k2);
                    if(result1.error) print("Error: " + result1.error);
                    else print("Value: " + result1.value);
                    if(result1.value != "test") return fail("Invalid: " + target);
                    var result2 = tryof(target.k1[5].item);
                    if(result2.error) print("Error: " + result2.error);
                    else print("Value: " + result2.value);
                    if(find(result2.error, "[INDX06]") < 0) return fail("Invalid: " + target);
                }
            }
            """;
        var json =
            """
            {
                "tryofTest": { "k1": 10, "k2": "test", 
                    "k3": null, "k4": 100.5 }
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_TryofWithThrowError_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "tryofTest": @tryofTest
            }
            %script: {
                constraint tryofTest() {
                    var result = tryof(throwerFunc(-1));
                    if(result.error) print("Error: " + result.error);
                    else print("Value : " + result.value);
                    if(find(result.error, "[ERR01]") < 0) return fail("Invalid: " + result);
                }
                
                subroutine function throwerFunc(value) {
                    if(value < 0) throw("ERR01", "Invalid argument value");
                    print("This line not execute");
                }
            }
            """;
        var json =
            """
            {
                "tryofTest": { "k1": 10, "k2": "test", 
                    "k3": null, "k4": 100.5 }
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_CheckSchemaNodeDataType_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "type1": @checkType("#integer") #integer,
                "type2": @checkType("#float") #float,
                "type3": @checkType("#double") #double,
                "type4": @checkType("#string") #string,
                "type5": @checkType("#boolean") #boolean,
                "type6": @checkType("#array") #array,
                "type7": @checkType("#object") #object,
                // No validation on null if allowed
                "type8": @checkType(null, "#null") #number,
                // Undefined is a special value of schema
                "type9": @checkType(!, "#undefined") #number
            }
            %script: {
                constraint checkType(type) {
                    if(type(target) != type) return fail("Invalid: " + type(target));
                }
                
                constraint checkType(value, type) {
                    if(type(value) != type) return fail("Invalid: " + type(value));
                }
            }
            """;
        var json =
            """
            {
                "type1": 10,
                "type2": 100.55,
                "type3": 10E3,
                "type4": "Test",
                "type5": true,
                "type6": [],
                "type7": {},
                "type8": 0,
                "type9": 0
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_CheckScriptValueDataType_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "scriptType": @checkType #integer
            }
            %script: {
                constraint checkType() {
                    var integer = 10;
                    var double = 10.5E3;
                    var string = "Test";
                    var boolean = true;
                    var array = [];
                    var object = {};
                    var nullVal = null;
                    var undefVal = undefined;
                    var noneAssigned;
                    
                    if(type(integer) != "#integer") return fail("Invalid: " + type(integer));
                    if(type(double) != "#double") return fail("Invalid: " + type(double));
                    if(type(string) != "#string") return fail("Invalid: " + type(string));
                    if(type(boolean) != "#boolean") return fail("Invalid: " + type(boolean));
                    if(type(array) != "#array") return fail("Invalid: " + type(array));
                    if(type(object) != "#object") return fail("Invalid: " + type(object));
                    if(type(nullVal) != "#null") return fail("Invalid: " + type(nullVal));
                    if(type(undefVal) != "#undefined") return fail("Invalid: " + type(undefVal));
                    if(type(noneAssigned) != "#void") return fail("Invalid: " + type(noneAssigned));
                }
            }
            """;
        var json =
            """
            {
                "scriptType": 10
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_PrePostIncrementDecrement_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "testIncDec": @testIncDec
            }
            %script: {
                constraint testIncDec() {
                    // target is a readonly reference to the node
                    var t = target;
                    var preInc = ++t;
                    if(preInc != t) return fail("Invalid: " + t);
                    var postInc = t++;
                    if(postInc != t - 1) return fail("Invalid: " + t);
                    var preDec = --t;
                    if(preDec != t) return fail("Invalid: " + t);
                    var postDec = t--;
                    if(postDec != t + 1) return fail("Invalid: " + t);
                }
            }
            """;
        var json =
            """
            {
                "testIncDec": 10
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_MultipleScriptBlocksAndComponents_ValidTrue()
    {
        var schema =
            """
            %script: {
                constraint blockTest1() {
                    var value = 0;
                    for(;;) {
                        if(value < 10) value++;
                        else break;
                    }
                    if(target != value) return fail("Invalid: " + target);
                }
            }
            %define $component2: @range(10, 20) @blockTest2 #integer
            %schema:
            {
                "block1": $component1,
                "block2": $component2,
                "block3": @blockTest3 #integer
            }
            %script: {
                constraint blockTest2() {
                    if(target != 20) return fail("Invalid: " + target);
                }
            }
            %script: {
                constraint blockTest3() {
                    if(target != 30) return fail("Invalid: " + target);
                }
            }
            %define $component1: @range(1, 10) @blockTest1 #integer
            """;
        var json =
            """
            {
                "block1": 10,
                "block2": 20,
                "block3": 30
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
}