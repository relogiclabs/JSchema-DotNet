using Antlr4.Runtime;
using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Script;
using RelogicLabs.JSchema.Types;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Antlr.SchemaParser;
using static RelogicLabs.JSchema.Engine.ScriptErrorHelper;
using static RelogicLabs.JSchema.Engine.ScriptTreeHelper;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.MessageFormatter;
using static RelogicLabs.JSchema.Tree.IEFunction;
using static RelogicLabs.JSchema.Tree.ScriptFunction;
using static RelogicLabs.JSchema.Types.IEUndefined;

namespace RelogicLabs.JSchema.Engine;

internal sealed partial class ScriptTreeVisitor
{
    private static readonly Evaluator ThrowCodeSupplier = static _ => GString.From(THRO01);

    public override Evaluator VisitMemberBracketExpression(MemberBracketExpressionContext context)
        => HandleBracketReadExpression(context);

    public override Evaluator VisitMemberDotExpression(MemberDotExpressionContext context)
        => HandleDotExpression(context);

    public override Evaluator VisitIdentifierExpression(IdentifierExpressionContext context)
        => HandleIdentifierExpression(context);

    private Evaluator HandleReferenceUpdateExpression(ParserRuleContext context)
    {
        if(context.HasToken(G_LBRACKET)) return HandleBracketUpdateExpression(context);
        if(context.HasToken(G_DOT)) return HandleDotExpression(context);
        return HandleIdentifierExpression(context);
    }

    private Evaluator HandleBracketReadExpression(ParserRuleContext context)
    {
        var expressions = context.GetRuleContexts<ExpressionContext>();
        var ref1 = Visit(expressions[0]);
        var index2 = Visit(expressions[1]);
        return TryCatch(scope =>
        {
            var v1 = Dereference(ref1(scope));
            var v2 = Dereference(index2(scope));
            var v3 = EvaluateArrayIndex(v1, v2, expressions[1])
                     ?? EvaluateArrayRange(v1, v2, expressions[1])
                     ?? EvaluateStringIndex(v1, v2, expressions[1])
                     ?? EvaluateStringRange(v1, v2, expressions[1])
                     ?? EvaluateObjectProperty(v1, v2, expressions[1]);

            if(v3 != null) return v3;
            if(v2 is IEInteger) throw FailOnOperation(IDXR01, OpIndex, v1, context.GetToken(G_LBRACKET));
            if(v2 is GRange) throw FailOnOperation(RNGR01, OpRange, v1, context.GetToken(G_LBRACKET));
            throw FailOnOperation(BKTR01, OpBracket, v1, v2, context.GetToken(G_LBRACKET));
        }, BKTR02, expressions[0]);
    }

    private Evaluator HandleBracketUpdateExpression(ParserRuleContext context)
    {
        var expressions = context.GetRuleContexts<ExpressionContext>();
        var ref1 = Visit(expressions[0]);
        var index2 = Visit(expressions[1]);
        return TryCatch(scope =>
        {
            var v1 = Dereference(ref1(scope));
            var v2 = Dereference(index2(scope));
            var v3 = EvaluateArrayIndex(v1, v2, expressions[1])
                     ?? EvaluateObjectProperty(v1, v2, expressions[1]);
            if(v3 != null) return v3;
            ThrowBracketUpdateException(v1, v2, context);
            throw new InvalidOperationException("Invalid runtime state");
        }, BKTU02, expressions[0]);
    }

    private static void ThrowBracketUpdateException(IEValue v1, IEValue v2, ParserRuleContext context)
    {
        if(v1 is IEString) throw FailOnStringUpdate(context.HasToken(G_ASSIGN)
            ? SASN01 : SUPD01, context.GetToken(G_LBRACKET));
        if(v1 is IEArray && v2 is GRange) throw FailOnArrayRangeUpdate(context.HasToken(G_ASSIGN)
            ? ARAS01 : ARUD01, context.GetToken(G_LBRACKET));
        if(v2 is IEInteger) throw FailOnOperation(context.HasToken(G_ASSIGN)
            ? IDXA01 : IDXU01, OpIndex, v1, context.GetToken(G_LBRACKET));
        if(v2 is GRange) throw FailOnOperation(context.HasToken(G_ASSIGN)
            ? RNGA01 : RNGU01, OpRange, v1, context.GetToken(G_LBRACKET));

        if(context.HasToken(G_ASSIGN)) throw FailOnOperation(BKTA01,
            OpBracketedAssignment, v1, v2, context.GetToken(G_LBRACKET));
        throw FailOnOperation(BKTU01, OpBracket, v1, v2, context.GetToken(G_LBRACKET));
    }

    private Evaluator HandleDotExpression(ParserRuleContext context)
    {
        var expressions = context.GetRuleContexts<ExpressionContext>();
        var identifier = context.GetToken(G_IDENTIFIER);
        var ref1 = Visit(expressions[0]);
        var key2 = identifier.GetText();
        return TryCatch(scope =>
        {
            var v1 = Dereference(ref1(scope));
            if(v1 is not IEObject o1) throw FailOnOperation(PRPT01,
                OpProperty, v1, context.GetToken(G_DOT));
            var v2 = o1.Get(key2);
            if(v2 == null) throw FailOnPropertyNotExist(PRPT02, o1, key2, identifier.Symbol);
            return v2;
        }, PRPT04, expressions[0]);
    }

    private static Evaluator HandleIdentifierExpression(ParserRuleContext context)
    {
        var identifier = context.GetToken(G_IDENTIFIER);
        var ids1 = identifier.GetText();
        return TryCatch(scope =>
        {
            var v1 = scope.Resolve(ids1)
                ?? throw FailOnIdentifierNotFound(IDEN01, identifier.Symbol);
            return v1;
        }, IDEN02, context);
    }

    private static IEValue? EvaluateArrayIndex(IEValue v1, IEValue v2, ExpressionContext context)
    {
        if(v1 is not IEArray a1 || v2 is not IEInteger i2) return null;
        try
        {
            return a1.Get((int) i2.Value);
        }
        catch(Exception e) when(e is IndexOutOfRangeException or ArgumentOutOfRangeException)
        {
            throw FailOnIndexOutOfBounds(a1, i2, context.Start, e);
        }
    }

    private static GArray? EvaluateArrayRange(IEValue v1, IEValue v2, ExpressionContext context)
    {
        if(v1 is not IEArray a1 || v2 is not GRange r2) return null;
        try
        {
            return GArray.From(a1, r2);
        }
        catch(Exception e) when(e is IndexOutOfRangeException or ArgumentOutOfRangeException)
        {
            throw FailOnInvalidRangeIndex(a1, r2, context.Start, e);
        }
    }

    private static IEValue? EvaluateObjectProperty(IEValue v1, IEValue v2, ExpressionContext context)
    {
        if(v1 is not IEObject o1 || v2 is not IEString s2) return null;
        var v3 = o1.Get(s2.Value);
        if(v3 == null) throw FailOnPropertyNotExist(PRPT03, o1, s2.Value, context.Start);
        return v3;
    }

    private static GString? EvaluateStringIndex(IEValue v1, IEValue v2, ExpressionContext context)
    {
        if(v1 is not IEString s1 || v2 is not IEInteger i2) return null;
        try
        {
            return GString.From(s1.Value[(int) i2.Value]);
        }
        catch(Exception e) when(e is IndexOutOfRangeException or ArgumentOutOfRangeException)
        {
            throw FailOnIndexOutOfBounds(s1, i2, context.Start, e);
        }
    }

    private static GString? EvaluateStringRange(IEValue v1, IEValue v2, ExpressionContext context)
    {
        if(v1 is not IEString s1 || v2 is not GRange r2) return null;
        try
        {
            return GString.From(s1, r2);
        }
        catch(Exception e) when(e is IndexOutOfRangeException or ArgumentOutOfRangeException)
        {
            throw FailOnInvalidRangeIndex(s1, r2, context.Start, e);
        }
    }

    public override Evaluator VisitInvokeFunctionExpression(InvokeFunctionExpressionContext context)
    {
        var fn1 = context.G_IDENTIFIER();
        var name1 = fn1.GetText();
        var param2 = context.expression().Select(Visit).ToList();
        var fid1 = $"{name1}#{param2.Count}";
        var fid2 = $"{name1}#{VariadicArity}";
        return TryCatch(scope =>
        {
            try
            {
                var v1 = scope.Resolve(fid1) ?? scope.Resolve(fid2);
                if(v1 is not IRFunction f1) throw FailOnFunctionNotFound(name1,
                    param2.Count, fn1.Symbol);
                var p2 = param2.Select(eval => Dereference(eval(scope))).ToList();
                return f1.Invoke(f1.Bind(scope, p2), p2);
            }
            catch(ScriptInvocationException e)
            {
                throw FailOnRuntime(e.Code, e.GetMessage(name1), e.GetToken(fn1.Symbol), e);
            }
            catch(ScriptArgumentException e)
            {
                throw FailOnRuntime(e.Code, e.GetMessage(name1), fn1.Symbol, e);
            }
        }, FNVK04, context);
    }

    public override Evaluator VisitInvokeMethodExpression(InvokeMethodExpressionContext context)
    {
        var self1 = Visit(context.expression(0));
        var method2 = context.G_IDENTIFIER();
        var name2 = method2.GetText();
        var param3 = context.expression().GetRange(1).Select(Visit).ToList();
        return TryCatch(scope =>
        {
            var s1 = Dereference(self1(scope));
            var m2 = s1.GetMethod(name2, param3.Count);
            var p3 = param3.Select(eval => Dereference(eval(scope))).ToList();
            return m2(s1, p3, scope);
        }, MNVK02, context);
    }

    public override Evaluator VisitTryofExpression(TryofExpressionContext context)
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
                return CreateTryofMonad(UNDEFINED, GString.From(e.Message));
            }
        };
    }

    public override Evaluator VisitThrowExpression(ThrowExpressionContext context)
    {
        var hasCode = context.G_COMMA() != null;
        var code = hasCode ? Visit(context.expression(0)) : ThrowCodeSupplier;
        var message = hasCode ? Visit(context.expression(1)) : Visit(context.expression(0));
        return TryCatch(scope =>
        {
            var c1 = Stringify(Dereference(code(scope)));
            var m2 = Stringify(Dereference(message(scope)));
            throw new ScriptInitiatedException(FormatForSchema(c1, m2, context.G_THROW().Symbol));
        }, THRO02, context);
    }

    public override Evaluator VisitTargetExpression(TargetExpressionContext context)
    {
        return TryCatch(scope =>
        {
            var v1 = scope.Resolve(TARGET_HVAR)
                ?? throw FailOnTargetNotFound(context.G_TARGET().Symbol);
            return v1;
        }, TRGT02, context);
    }

    public override Evaluator VisitCallerExpression(CallerExpressionContext context)
    {
        return TryCatch(scope =>
        {
            var v1 = scope.Resolve(CALLER_HVAR)
                ?? throw FailOnCallerNotFound(context.G_CALLER().Symbol);
            return v1;
        }, CALR02, context);
    }

    public override Evaluator VisitRangeBothExpression(RangeBothExpressionContext context)
    {
        var expr1 = Visit(context.expression(0));
        var expr2 = Visit(context.expression(1), VoidSupplier);
        if(ReferenceEquals(expr2, VoidSupplier)) return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            if(v1 is not IEInteger i1) throw FailOnOperation(RNGT01,
                OpRangeSetup, v1, context.G_RANGE());
            return GRange.From(i1, null);
        }, RNGT02, context);

        return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            if(v1 is not IEInteger i1 || v2 is not IEInteger i2)
                throw FailOnOperation(RNGT03, OpRangeSetup, v1, v2, context.G_RANGE());
            return GRange.From(i1, i2);
        }, RNGT04, context);
    }

    public override Evaluator VisitRangeEndExpression(RangeEndExpressionContext context)
    {
        var expr2 = Visit(context.expression());
        return TryCatch(scope =>
        {
            var v2 = Dereference(expr2(scope));
            if(v2 is not IEInteger i2) throw FailOnOperation(RNGT05,
                OpRangeSetup, v2, context.G_RANGE());
            return GRange.From(null, i2);
        }, RNGT06, context);
    }
}