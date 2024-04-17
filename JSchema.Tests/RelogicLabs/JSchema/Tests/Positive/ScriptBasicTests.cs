namespace RelogicLabs.JSchema.Tests.Positive;

[TestClass]
public class ScriptBasicTests
{
    [TestMethod]
    public void When_ArithmeticOnIntegerExpression_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "exprTest": @exprTest #integer* #array
            }
            %script: {
                constraint exprTest() {
                    var num = 6;
                    var result = num % 4 + 3 * (2 + 1) - 8 / 2;
                    result = -result;
                    if(+result != -target[0]) throw("Invalid: " + target);
                    result += 110;
                    if(result != target[1]) throw("Invalid: " + target);
                    result -= 50;
                    if(result != target[2]) throw("Invalid: " + target);
                    result *= 4;
                    if(result != target[3]) throw("Invalid: " + target);
                    result /= 53;
                    if(result != target[4]) throw("Invalid: " + target);
                    result %= 2;
                    if(result != target[5]) throw("Invalid: " + target);
                }
            }
            """;
        var json =
            """
            {
                "exprTest": [7, 103, 53, 212, 4, 0]
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_ArithmeticOnFloatingPointExpression_ValidTrue()
    {
        var schema =
            """
            %script: {
                constraint exprTest() {
                    var num = 11.8;
                    var result = 10.1 - (num % 8.3 - 1.2) * 16.1 + 45.15 / 3.5;
                    if(-result != +target[0]) throw("Invalid: " + target);
                    result += 154.53;
                    if(result != target[1]) throw("Invalid: " + result);
                    result -= 25.96;
                    if(result != target[2]) throw("Invalid: " + target);
                    result *= 4;
                    if(result != target[3]) throw("Invalid: " + target);
                    result /= 12;
                    if(result != target[4]) throw("Invalid: " + target);
                    result %= 4.46;
                    if(result != target[5]) throw("Invalid: " + target);
                }
            }
            %schema:
            {
                "exprTest": @exprTest #float* #array
            }
            """;
        var json =
            """
            {
                "exprTest": [14.03, 140.50, 114.54, 458.16, 38.18, 2.50]
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
                    var result = target.find("Ipsum");
                    result = target.find("Ipsum", result + 1);
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
                    var result = target.find(10);
                    result = target.find(10, result + 1);
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
                    if(target.type() != "#string") throw("Invalid: " + target);
                    var result = "Lorem " + "Ipsum" + 10 + [1, 20] + {k1: 100};
                    if(result != target) throw("Invalid: " + target);
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
                    if(!(target >= 10.1)) throw("Invalid: " + target);
                    if(!(target >= 10)) throw("Invalid: " + target);
                    if(!(target <= 10.1)) throw("Invalid: " + target);
                    if(!(target <= 11)) throw("Invalid: " + target);
                    if(!(target > 10.01)) throw("Invalid: " + target);
                    if(!(target > 10)) throw("Invalid: " + target);
                    if(!(target < 10.11)) throw("Invalid: " + target);
                    if(!(target < 11)) throw("Invalid: " + target);
                    if(target != 10.1) throw("Invalid: " + target);
                    if(target == 10) throw("Invalid: " + target);
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
                    if(result1.value != "test") throw("Invalid: " + target);
                    var result2 = tryof(target.k1[5].item);
                    if(result2.error) print("Error: " + result2.error);
                    else print("Value: " + result2.value);
                    if(!result2.error.find("[IDXR01]")) throw("Invalid: " + target);
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
                    if(!result.error.find("[ERR01]")) throw("Invalid: " + result);
                }

                subroutine function throwerFunc(value) {
                    if(value < 0) throw("ERR01", "Invalid argument value");
                    print("line not execute");
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
                    if(target.type() != type) throw("Invalid: " + target);
                }

                constraint checkType(value, type) {
                    if(value.type() != type) throw("Invalid: " + value);
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

                    if(integer.type() != "#integer") throw("Invalid: " + integer.type());
                    if(double.type() != "#double") throw("Invalid: " + double.type());
                    if(string.type() != "#string") throw("Invalid: " + string.type());
                    if(boolean.type() != "#boolean") throw("Invalid: " + boolean.type());
                    if(array.type() != "#array") throw("Invalid: " + array.type());
                    if(object.type() != "#object") throw("Invalid: " + object.type());
                    if(nullVal.type() != "#null") throw("Invalid: " + nullVal.type());
                    if(undefVal.type() != "#undefined") throw("Invalid: " + undefVal.type());
                    if(noneAssigned.type() != "#void") throw("Invalid: " + noneAssigned.type());
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
                    // updating 't' creates a writable copy of target
                    var t = target;
                    var preInc = ++t;
                    if(preInc != t) throw("Invalid: " + t);
                    var postInc = t++;
                    if(postInc != t - 1) throw("Invalid: " + t);
                    var preDec = --t;
                    if(preDec != t) throw("Invalid: " + t);
                    var postDec = t--;
                    if(postDec != t + 1) throw("Invalid: " + t);
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
    public void When_CheckRangeOperations_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "testRangeType": @testRange #string
            }
            %script: {
                constraint testRange() {
                    var range = 5..9;
                    if(target[range] != "is a") throw("Invalid: " + target[range]);
                    if(testRange(range) != 1..10) throw("Invalid: " + range);
                }

                subroutine testRange(range) {
                    if(range.string() != "5..9") throw("Invalid: " + range);
                    if(range != 5..9) throw("Invalid: " + range);
                    return 1..10;
                }
            }
            """;
        var json =
            """
            {
                "testRangeType": "This is a string"
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
                    if(target != value) throw("Invalid: " + target);
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
                    if(target != 20) throw("Invalid: " + target);
                }
            }
            %script: {
                constraint blockTest3() {
                    if(target != 30) throw("Invalid: " + target);
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