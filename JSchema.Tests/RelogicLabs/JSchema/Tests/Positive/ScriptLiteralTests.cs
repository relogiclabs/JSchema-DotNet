namespace RelogicLabs.JSchema.Tests.Positive;

[TestClass]
public class ScriptLiteralTests
{
    [TestMethod]
    public void When_ObjectCreateWithAddAndUpdate_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "value": @objectTest
            }
            %script: {
                constraint objectTest() {
                    var test = null;
                    var obj = { "k1": 1, k3: {
                        k2: 20.5 + 4.2 * 3.5 + 8.7 - 5.0,
                        k3: test
                    } };
                    obj["k2"] = "test";
                    obj.k4 = 100.5;
                    obj.k1 = 100;
                    obj.k3.k1 = 10;
                    if(obj["k3"].k2 != 38.9) return fail("Invalid: " + obj);
                    if(obj != target) return fail("Invalid: " + target);
                }
            }
            """;
        var json =
            """
            {
                "value": {
                    "k1": 100,
                    "k2": "test",
                    "k3": {
                        "k1": 10,
                        "k2": 38.9,
                        "k3": null
                    },
                    "k4": 100.5
                }
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_ArrayCreateWithAddAndUpdateInObject_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "value": @arrayTest
            }
            %script: {
                constraint arrayTest() {
                    var val = 30;
                    var obj = { k1: 1, k2: 20, "k3": {
                        k2: [ 10, 30.9, 20.5 + 4.2 * 3.5 + 8.7 - 5.0,
                            { k4 : val }
                        ]
                    } };
                    obj["k1"] = "test";
                    obj["k3"].k2[4] = [10, 20];
                    if(obj.k3.k2 != target) return fail("Invalid: " + target);
                    if(obj.k3["k2"][2] != 38.9) return fail("Invalid: " + obj);
                }
            }
            """;
        var json =
            """
            {
                "value": [10, 30.9, 38.9, {"k4": 30}, [10, 20]]
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_ArrayCreateWithAddAndUpdate_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "value": @arrayTest
            }
            %script: {
                constraint arrayTest() {
                    var test = null;
                    var arr = [1, 20, [100.5, 1E-3, test], 50, test, undefined];
                    arr[0] = { k10: 10E2 };
                    arr[6] = 34.5;
                    if(arr[2] != target) return fail("Invalid: " + target);
                    if(arr[0] != { k10: 10E2 }) return fail("Invalid: " + arr);
                }
            }
            """;
        var json =
            """
            {
                "value": [100.5, 1E-3, null]
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_CheckUndefinedFromSchema_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "value1": @checkRange(10, !) #integer,
                "value2": @checkRange(!, 1000) #integer
            }
            %script: {
                future constraint checkRange(min, max) {
                    if(min != undefined && target < min) return fail(
                        "RANGEERR01", "The value is less than minimum",
                        expected("a value in range " + [min, max]),
                        actual("but found " + target + " which is out of range"));
                    if(max != undefined && target > max) return fail(
                        "RANGEERR02", "The value is greater than maximum",
                        expected("a value in range " + [min, max]),
                        actual("but found " + target + " which is out of range"));
                }
            }
            """;
        var json =
            """
            {
                "value1": 10,
                "value2": 1000
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_CheckNullFromSchema_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "value1": @checkRange(10, null) #integer,
                "value2": @checkRange(null, 10000) #integer
            }
            %script: {
                future constraint checkRange(min, max) {
                    if(min != null && target < min) return fail(
                        "RANGEERR01", "The value is less than minimum",
                        expected("a value in range " + [min, max]),
                        actual("but found " + target + " which out of range"));
                    if(max != null && target > max) return fail(
                        "RANGEERR02", "The value is greater than maximum",
                        expected("a value in range " + [min, max]),
                        actual("but found " + target + " which out of range"));
                }
            }
            """;
        var json =
            """
            {
                "value1": 90,
                "value2": 500
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_CheckNullOrUndefinedFromSchema_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "value1": @checkRange(0, 100) #integer,
                "value2": @checkRange(200, 1000) #integer,
                "value3": @checkRange(20, !) #integer,
                "value4": @checkRange(null, 1000) #integer
            }
            %script: {
                future constraint checkRange(min, max) {
                    if(min && target < min) return fail(
                        "RANGEERR01", "The value is less than minimum",
                        expected("a value in range " + [min, max]),
                        actual("but found " + target + " which out of range"));
                    if(max && target > max) return fail(
                        "RANGEERR02", "The value is greater than maximum",
                        expected("a value in range " + [min, max]),
                        actual("but found " + target + " which out of range"));
                }
            }
            """;
        var json =
            """
            {
                "value1": 90,
                "value2": 500,
                "value3": 20,
                "value4": 1000
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_CheckRangeAndIteratorOnString_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "value": @stringTest
            }
            %script: {
                constraint stringTest() {
                    if(target[-4..] != "text") throw("Invalid: " + target);
                    if(target[0..] != "This is a text") throw("Invalid: " + target);
                    if(target[2..7] != "is is") throw("Invalid: " + target);
                    if(target[5..-5] != "is a") throw("Invalid: " + target);
                    if(target[..-5] != "This is a") throw("Invalid: " + target);
                    var string = "";
                    foreach(var c in target) string += c;
                    if(target != string) throw("Invalid: " + target);
                }
            }
            """;
        var json =
            """
            {
                "value": "This is a text"
            }
            """;
        JsonAssert.IsValid(schema, json);
    }

    [TestMethod]
    public void When_CheckRangeAndIteratorOnArray_ValidTrue()
    {
        var schema =
            """
            %schema:
            {
                "value": @arrayTest
            }
            %script: {
                constraint arrayTest() {
                    if(target[3..] != [4, 5, 6, 7]) throw("Invalid: " + target);
                    if(target[3..6] != [4, 5, 6]) throw("Invalid: " + target);
                    if(target[-5..-2] != [3, 4, 5]) throw("Invalid: " + target);
                    if(target[..4] != [1, 2, 3, 4]) throw("Invalid: " + target);
                    var array = [], index = 0;
                    foreach(var e in target) array[index++] = e;
                    if(target != array) throw("Invalid: " + target);
                }
            }
            """;
        var json =
            """
            {
                "value": [1, 2, 3, 4, 5, 6, 7]
            }
            """;
        JsonAssert.IsValid(schema, json);
    }
}