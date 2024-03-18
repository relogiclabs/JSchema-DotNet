using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Message;
using RelogicLabs.JSchema.Nodes;
using RelogicLabs.JSchema.Script;
using RelogicLabs.JSchema.Types;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Engine.ScriptTreeHelper;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.MessageFormatter;
using static RelogicLabs.JSchema.Script.GBoolean;
using static RelogicLabs.JSchema.Tree.ScriptFunction;
using static RelogicLabs.JSchema.Types.IEValue;

namespace RelogicLabs.JSchema.Library;

internal sealed partial class ScriptLibrary
{
    private void ScriptPrintFunction()
    {
        NEvaluator evaluator = static (_, arguments) =>
        {
            Console.WriteLine(arguments.Select(Stringify).Join(" "));
            return VOID;
        };
        _symbols[PRINT_FNV] = new NFunction(evaluator, MESSAGE_ID, ARGS_IDV);
    }

    private void ScriptTypeFunction()
    {
        NEvaluator evaluator = static (_, arguments)
            => GString.Of(arguments[0].Type.Name);
        _symbols[TYPE_FN1] = new NFunction(evaluator, VALUE_ID);
    }

    private void ScriptSizeFunction()
    {
        NEvaluator evaluator = static (_, arguments) =>
        {
            var value = arguments[0];
            if(value is IEArray a) return GInteger.Of(a.Count);
            if(value is IEObject o) return GInteger.Of(o.Count);
            if(value is IEString s) return GInteger.Of(s.Length);
            throw FailOnInvalidArgumentType(SIZE01, VALUE_ID, value);
        };
        _symbols[SIZE_FN1] = new NFunction(evaluator, VALUE_ID);
    }

    private void ScriptStringifyFunction()
    {
        NEvaluator evaluator = static (_, arguments) => GString.Of(Stringify(arguments[0]));
        _symbols[STRINGIFY_FN1] = new NFunction(evaluator, VALUE_ID);
    }

    private void ScriptFindFunction1()
    {
        NEvaluator evaluator = static (scope, arguments) =>
        {
            var group = arguments[0];
            var item = arguments[1];
            var rt = scope.GetRuntime();
            if(group is IEArray a)
            {
                for(var i = 0; i < a.Count; i++)
                    if(AreEqual(a.Get(i), item, rt)) return GInteger.Of(i);
                return GInteger.Of(-1);
            }
            if(group is IEString s) return GInteger.Of(s.Value.IndexOf(
                GetString(item, ITEM_ID, FIND01)));
            throw FailOnInvalidArgumentType(FIND02, GROUP_ID, group);
        };
        _symbols[FIND_FN2] = new NFunction(evaluator, GROUP_ID, ITEM_ID);
    }

    private void ScriptFindFunction2()
    {
        NEvaluator evaluator = static (scope, arguments) =>
        {
            var group = arguments[0];
            var item = arguments[1];
            var from = arguments[2];
            var rt = scope.GetRuntime();
            if(group is IEArray a)
            {
                for(var i = (int) GetInteger(from, FROM_ID, FIND03); i < a.Count; i++)
                    if(AreEqual(a.Get(i), item, rt)) return GInteger.Of(i);
                return GInteger.Of(-1);
            }
            if(group is IEString s) return GInteger.Of(s.Value.IndexOf(
                GetString(item, ITEM_ID, FIND04), (int) GetInteger(from, FROM_ID, FIND05)));
            throw FailOnInvalidArgumentType(FIND06, GROUP_ID, group);
        };
        _symbols[FIND_FN3] = new NFunction(evaluator, GROUP_ID, ITEM_ID, FROM_ID);
    }

    private void ScriptRegularFunction()
    {
        NEvaluator evaluator = static (_, arguments) =>
        {
            var value = arguments[0];
            if(value is IENull) return FALSE;
            if(value is IEUndefined) return FALSE;
            if(ReferenceEquals(value, VOID)) return FALSE;
            return TRUE;
        };
        _symbols[REGULAR_FN1] = new NFunction(evaluator, VALUE_ID);
    }

    private void ScriptFailFunction1()
    {
        NEvaluator evaluator = static (scope, arguments) =>
        {
            var caller = scope.Resolve(CALLER_HVAR);
            if(ReferenceEquals(caller, VOID)) caller = null;
            Fail(scope, new ScriptInitiatedException(FormatForSchema(FAIL01,
                GetString(arguments[0], MESSAGE_ID, FAIL02), (JNode?) caller)));
            return FALSE;
        };
        _symbols[FAIL_FN1] = new NFunction(evaluator, MESSAGE_ID);
    }

    private void ScriptFailFunction2()
    {
        NEvaluator evaluator = static (scope, arguments) =>
        {
            var caller = scope.Resolve(CALLER_HVAR);
            if(ReferenceEquals(caller, VOID)) caller = null;
            Fail(scope, new ScriptInitiatedException(FormatForSchema(
                GetString(arguments[0], CODE_ID, FAIL03),
                GetString(arguments[1], MESSAGE_ID, FAIL04), (JNode?) caller)));
            return FALSE;
        };
        _symbols[FAIL_FN2] = new NFunction(evaluator, CODE_ID, MESSAGE_ID);
    }

    private void ScriptFailFunction3()
    {
        NEvaluator evaluator = static (scope, arguments) =>
        {
            var expected = Cast<IEObject>(arguments[2], EXPECTED_ID, FAIL05);
            var actual = Cast<IEObject>(arguments[3], ACTUAL_ID, FAIL06);
            Fail(scope, new JsonSchemaException(new ErrorDetail(
                    GetString(arguments[0], CODE_ID, FAIL07),
                    GetString(arguments[1], MESSAGE_ID, FAIL08)),
                new ExpectedDetail(GetValue<JNode>(expected, NODE_ID, EXPECTED_ID, FAIL09),
                    GetValue<IEString>(expected, MESSAGE_ID, EXPECTED_ID, FAIL10).Value),
                new ActualDetail(GetValue<JNode>(actual, NODE_ID, ACTUAL_ID, FAIL11),
                    GetValue<IEString>(actual, MESSAGE_ID, ACTUAL_ID, FAIL12).Value)));
            return FALSE;
        };
        _symbols[FAIL_FN4] = new NFunction(evaluator, CODE_ID, MESSAGE_ID, EXPECTED_ID, ACTUAL_ID);
    }

    private void ScriptExpectedFunction1()
    {
        NEvaluator evaluator = static (scope, arguments) =>
        {
            var result = new GObject(2);
            result.Set(NODE_ID, scope.Resolve(CALLER_HVAR));
            result.Set(MESSAGE_ID, Cast<IEString>(arguments[0], MESSAGE_ID, EXPC01));
            return result;
        };
        _symbols[EXPECTED_FN1] = new NFunction(evaluator, MESSAGE_ID);
    }

    private void ScriptExpectedFunction2()
    {
        NEvaluator evaluator = static (_, arguments) =>
        {
            var result = new GObject(2);
            result.Set(NODE_ID, Cast<JNode>(arguments[0], NODE_ID, EXPC02));
            result.Set(MESSAGE_ID, Cast<IEString>(arguments[1], MESSAGE_ID, EXPC03));
            return result;
        };
        _symbols[EXPECTED_FN2] = new NFunction(evaluator, NODE_ID, MESSAGE_ID);
    }

    private void ScriptActualFunction1()
    {
        NEvaluator evaluator = static (scope, arguments) =>
        {
            var result = new GObject(2);
            result.Set(NODE_ID, scope.Resolve(TARGET_HVAR));
            result.Set(MESSAGE_ID, Cast<IEString>(arguments[0], MESSAGE_ID, ACTL01));
            return result;
        };
        _symbols[ACTUAL_FN1] = new NFunction(evaluator, MESSAGE_ID);
    }

    private void ScriptActualFunction2()
    {
        NEvaluator evaluator = static (_, arguments) =>
        {
            var result = new GObject(2);
            result.Set(NODE_ID, Cast<JNode>(arguments[0], NODE_ID, ACTL02));
            result.Set(MESSAGE_ID, Cast<IEString>(arguments[1], MESSAGE_ID, ACTL03));
            return result;
        };
        _symbols[ACTUAL_FN2] = new NFunction(evaluator, NODE_ID, MESSAGE_ID);
    }

    private void ScriptCopyFunction()
    {
        NEvaluator evaluator = static (_, arguments) =>
        {
            var value = arguments[0];
            if(value is IEArray a) return new GArray(a.Values);
            if(value is IEObject o) return new GObject(o);
            if(value is IEString s) return GString.Of(s.Value);
            if(value is IEInteger i) return GInteger.Of(i.Value);
            if(value is IEDouble d) return GDouble.Of(d.Value);
            return value;
        };
        _symbols[COPY_FN1] = new NFunction(evaluator, VALUE_ID);
    }

    private void ScriptFillFunction()
    {
        NEvaluator evaluator = static (_, arguments) => GArray.FilledFrom(
            arguments[0], (int) GetInteger(arguments[1], SIZE_ID, FILL01));
        _symbols[FILL_FN2] = new NFunction(evaluator, VALUE_ID, SIZE_ID);
    }

    private void ScriptCeilFunction()
    {
        NEvaluator evaluator = static (_, arguments) => GInteger.Of((long) Math.Ceiling(
            GetNumber(arguments[0], VALUE_ID, CEIL01)));
        _symbols[CEIL_FN1] = new NFunction(evaluator, VALUE_ID);
    }

    private void ScriptFloorFunction()
    {
        NEvaluator evaluator = static (_, arguments) => GInteger.Of((long) Math.Floor(
            GetNumber(arguments[0], VALUE_ID, FLOR01)));
        _symbols[FLOOR_FN1] = new NFunction(evaluator, VALUE_ID);
    }

    private void ScriptModFunction()
    {
        NEvaluator evaluator = static (_, arguments) =>
        {
            var val1 = arguments[0];
            var val2 = arguments[1];
            if(val1 is IEInteger i1 && val2 is IEInteger i2)
                return GInteger.Of(i1.Value % i2.Value);
            var num1 = GetNumber(val1, VALUE1_ID, MODU01);
            var num2 = GetNumber(val2, VALUE2_ID, MODU02);
            return GDouble.Of(num1 % num2);
        };
        _symbols[MOD_FN2] = new NFunction(evaluator, VALUE1_ID, VALUE2_ID);
    }

    private void ScriptPowFunction()
    {
        NEvaluator evaluator = static (_, arguments) => GDouble.Of(Math.Pow(
            GetNumber(arguments[0], VALUE1_ID, POWR01),
            GetNumber(arguments[1], VALUE2_ID, POWR02)
        ));
        _symbols[POW_FN2] = new NFunction(evaluator, VALUE1_ID, VALUE2_ID);
    }

    private void ScriptLogFunction()
    {
        NEvaluator evaluator = static (_, arguments) => GDouble.Of(Math.Log(
            GetNumber(arguments[0], VALUE_ID, LOGA01)));
        _symbols[LOG_FN1] = new NFunction(evaluator, VALUE_ID);
    }

    private void ScriptTicksFunction()
    {
        NEvaluator evaluator = static (_, _) => GInteger.Of(Environment.TickCount64);
        _symbols[TICKS_FN0] = new NFunction(evaluator);
    }
}