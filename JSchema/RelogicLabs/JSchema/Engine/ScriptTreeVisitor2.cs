using RelogicLabs.JSchema.Antlr;
using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Script;
using RelogicLabs.JSchema.Types;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Engine.ScriptErrorHelper;
using static RelogicLabs.JSchema.Engine.ScriptTreeHelper;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.MessageFormatter;
using static RelogicLabs.JSchema.Tree.ScriptFunction;
using static RelogicLabs.JSchema.Types.IEUndefined;
using static RelogicLabs.JSchema.Types.IEValue;

namespace RelogicLabs.JSchema.Engine;

internal sealed partial class ScriptTreeVisitor
{
    private static readonly Evaluator ThrowCodeSupplier = static _ => GString.Of(THRO01);

    public override Evaluator VisitDotExpression(SchemaParser.DotExpressionContext context)
    {
        var ref1 = Visit(context.refExpression());
        var key2 = context.G_IDENTIFIER().GetText();
        return TryCatch(scope =>
        {
            var v1 = Dereference(ref1(scope));
            if(v1 is not IEObject o1) throw FailOnOperation(PRPS01, PROPERTY, v1, context.G_DOT());
            var v2 = o1.Get(key2);
            if(v2 == null) throw FailOnPropertyNotExist(o1, key2, context.G_IDENTIFIER().Symbol);
            return v2;
        }, PRPS03, context);
    }

    public override Evaluator VisitIndexExpression(SchemaParser.IndexExpressionContext context)
    {
        var ref1 = Visit(context.refExpression());
        var index2 = Visit(context.expression());
        return TryCatch(scope =>
        {
            var v1 = Dereference(ref1(scope));
            var v2 = Dereference(index2(scope));
            if(v1 is IEArray a1_1 && v2 is IEInteger i2_1)
            {
                try
                {
                    return a1_1.Get((int) i2_1.Value);
                }
                catch(Exception e) when(e is IndexOutOfRangeException or ArgumentOutOfRangeException)
                {
                    throw FailOnIndexOutOfBounds(a1_1, i2_1, context.expression().Start, e);
                }
            }
            if(v1 is IEArray a1_2 && v2 is GRange r2_2)
            {
                try
                {
                    return GArray.From(a1_2, r2_2);
                }
                catch(Exception e) when(e is IndexOutOfRangeException or ArgumentOutOfRangeException)
                {
                    throw FailOnInvalidRangeIndex(a1_2, r2_2, context.expression().Start, e);
                }
            }
            if(v1 is IEObject o1_3 && v2 is IEString s2_3)
            {
                var v3 = o1_3.Get(s2_3.Value);
                if(v3 == null) throw FailOnPropertyNotExist(o1_3, s2_3.Value,
                    context.expression().Start);
                return v3;
            }
            if(v1 is IEString s1_4 && v2 is IEInteger i2_4)
            {
                try
                {
                    return GString.Of(s1_4.Value[(int) i2_4.Value]);
                }
                catch(Exception e) when(e is IndexOutOfRangeException or ArgumentOutOfRangeException)
                {
                    throw FailOnIndexOutOfBounds(s1_4, i2_4, context.expression().Start, e);
                }
            }
            if(v1 is IEString s1_5 && v2 is GRange r2_5)
            {
                try
                {
                    return GString.From(s1_5, r2_5);
                }
                catch(Exception e) when(e is IndexOutOfRangeException or ArgumentOutOfRangeException)
                {
                    throw FailOnInvalidRangeIndex(s1_5, r2_5, context.expression().Start, e);
                }
            }
            if(v2 is IEInteger) throw FailOnOperation(INDX06, INDEX, v1, context.G_LBRACKET());
            if(v2 is GRange) throw FailOnOperation(RNGS10, RANGE, v1, context.G_LBRACKET());
            throw FailOnOperation(INDX07, INDEX, v1, v2, context.G_LBRACKET());
        }, INDX08, context);
    }

    public override Evaluator VisitInvokeExpression(SchemaParser.InvokeExpressionContext context)
    {
        var fn1 = context.G_IDENTIFIER();
        var param2 = context.expression().Select(Visit).ToList();
        var fns1 = fn1.GetText();
        var fid1 = fns1 + "#" + param2.Count;
        var fid2 = fns1 + "#...";
        return TryCatch(scope =>
        {
            try
            {
                var v1 = scope.Resolve(fid1);
                if(ReferenceEquals(v1, VOID)) v1 = scope.Resolve(fid2);
                if(v1 is not IRFunction f1) throw FailOnFunctionNotFound(fns1,
                    param2.Count, fn1.Symbol);
                var v2 = param2.Select(eval => Dereference(eval(scope))).ToList();
                return f1.Invoke(f1.Bind(scope, v2), v2);
            }
            catch(ScriptInvocationException e)
            {
                throw FailOnRuntime(e.Code, e.GetMessage(fns1), e.GetToken(fn1.Symbol), e);
            }
            catch(ScriptArgumentException e)
            {
                throw FailOnRuntime(e.Code, e.GetMessage(fns1), fn1.Symbol, e);
            }
        }, INVK01, context);
    }

    public override Evaluator VisitTryofExpression(SchemaParser.TryofExpressionContext context)
    {
        var expression = Visit(context.expression());
        return scope =>
        {
            try
            {
                var value = Dereference(expression(scope));
                return CreateTryofMonad(value, UNDEFINED);
            }
            catch(Exception e)
            {
                LogHelper.Log(e, context.expression().Start);
                return CreateTryofMonad(UNDEFINED, GString.Of(e.Message));
            }
        };
    }

    public override Evaluator VisitThrowExpression(SchemaParser.ThrowExpressionContext context)
    {
        var hasCode = context.G_COMMA() != null;
        var code = hasCode ? Visit(context.expression(0)) : ThrowCodeSupplier;
        var message = hasCode ? Visit(context.expression(1)) : Visit(context.expression(0));
        return TryCatch(scope =>
        {
            var s1 = Stringify(Dereference(code(scope)));
            var s2 = Stringify(Dereference(message(scope)));
            throw new ScriptInitiatedException(FormatForSchema(s1, s2, context.G_THROW().Symbol));
        }, THRO02, context);
    }

    public override Evaluator VisitTargetExpression(SchemaParser.TargetExpressionContext context)
    {
        return TryCatch(scope =>
        {
            var v1 = scope.Resolve(TARGET_HVAR);
            if(ReferenceEquals(v1, VOID)) throw FailOnTargetNotFound(context.G_TARGET().Symbol);
            return v1;
        }, TRGT02, context);
    }

    public override Evaluator VisitCallerExpression(SchemaParser.CallerExpressionContext context)
    {
        return TryCatch(scope =>
        {
            var v1 = scope.Resolve(CALLER_HVAR);
            if(ReferenceEquals(v1, VOID)) throw FailOnCallerNotFound(context.G_CALLER().Symbol);
            return v1;
        }, CALR02, context);
    }

    public override Evaluator VisitIdentifierExpression(SchemaParser.IdentifierExpressionContext context)
    {
        var id1 = context.G_IDENTIFIER();
        var ids1 = id1.GetText();
        return TryCatch(scope =>
        {
            var v1 = scope.Resolve(ids1);
            if(ReferenceEquals(v1, VOID)) throw FailOnIdentifierNotFound(id1.Symbol);
            return v1;
        }, VARD04, context);
    }

    public override Evaluator VisitRangeBothExpression(SchemaParser.RangeBothExpressionContext context)
    {
        var expr1 = Visit(context.expression(0));
        var expr2 = Visit(context.expression(1), VoidSupplier);
        if(expr2 == VoidSupplier) return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            if(v1 is not IEInteger i1) throw FailOnOperation(RNGS01, RANGE, v1, context.G_RANGE());
            return GRange.Of(i1, null);
        }, RNGS11, context);

        return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            if(v1 is not IEInteger i1 || v2 is not IEInteger i2)
                throw FailOnOperation(RNGS02, RANGE, v1, v2, context.G_RANGE());
            return GRange.Of(i1, i2);
        }, RNGS12, context);
    }

    public override Evaluator VisitRangeEndExpression(SchemaParser.RangeEndExpressionContext context)
    {
        var expr2 = Visit(context.expression());
        return TryCatch(scope =>
        {
            var v2 = Dereference(expr2(scope));
            if(v2 is not IEInteger i2) throw FailOnOperation(RNGS03, RANGE, v2, context.G_RANGE());
            return GRange.Of(null, i2);
        }, RNGS13, context);
    }
}