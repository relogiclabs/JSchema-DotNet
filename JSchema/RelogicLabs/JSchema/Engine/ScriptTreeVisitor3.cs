using Antlr4.Runtime;
using RelogicLabs.JSchema.Script;
using RelogicLabs.JSchema.Types;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Antlr.SchemaParser;
using static RelogicLabs.JSchema.Engine.ScriptErrorHelper;
using static RelogicLabs.JSchema.Engine.ScriptTreeHelper;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Script.GBoolean;

namespace RelogicLabs.JSchema.Engine;

internal sealed partial class ScriptTreeVisitor
{
    public override Evaluator VisitUnaryPlusExpression(UnaryPlusExpressionContext context)
    {
        var expr1 = Visit(context.expression());
        return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            if(v1 is not IENumber) throw FailOnOperation(PLUS01, OpUnaryPlus, v1, context.G_PLUS());
            return v1;
        }, PLUS02, context);
    }

    public override Evaluator VisitUnaryMinusExpression(UnaryMinusExpressionContext context)
    {
        var expr1 = Visit(context.expression());
        return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            if(v1 is IEInteger i1) return GInteger.From(-i1.Value);
            if(v1 is IENumber n1) return GDouble.From(-n1.ToDouble());
            throw FailOnOperation(MINS01, OpUnaryMinus, v1, context.G_MINUS());
        }, MINS02, context);
    }

    public override Evaluator VisitLogicalNotExpression(LogicalNotExpressionContext context)
    {
        var expr1 = Visit(context.expression());
        return TryCatch(scope => expr1(scope).ToBoolean() ? FALSE : TRUE, NOTL01, context);
    }

    public override Evaluator VisitPreIncDecExpression(PreIncDecExpressionContext context)
    {
        if(context.G_INC() != null) return HandlePreIncrementExpression(context);
        if(context.G_DEC() != null) return HandlePreDecrementExpression(context);
        throw new InvalidOperationException("Invalid parser state");
    }

    public override Evaluator VisitPostIncDecExpression(PostIncDecExpressionContext context)
    {
        if(context.G_INC() != null) return HandlePostIncrementExpression(context);
        if(context.G_DEC() != null) return HandlePostDecrementExpression(context);
        throw new InvalidOperationException("Invalid parser state");
    }

    private Evaluator HandlePostIncrementExpression(ParserRuleContext context)
    {
        var ref1 = HandleReferenceUpdateExpression(context);
        return TryCatch(scope =>
        {
            var v1 = ref1(scope);
            if(v1 is not GReference r1) throw FailOnInvalidLValueIncrement(INCT01,
                context.GetToken(G_INC).Symbol);
            if(r1.Value is not IENumber n1) throw FailOnOperation(INCT02,
                OpIncrement, v1, context.GetToken(G_INC));
            r1.Value = Increment(n1);
            return n1;
        }, INCT03, context);
    }

    private Evaluator HandlePreIncrementExpression(ParserRuleContext context)
    {
        var ref1 = HandleReferenceUpdateExpression(context);
        return TryCatch(scope =>
        {
            var v1 = ref1(scope);
            if(v1 is not GReference r1) throw FailOnInvalidLValueIncrement(INCE01,
                context.GetToken(G_INC).Symbol);
            if(r1.Value is not IENumber n1) throw FailOnOperation(INCE02,
                OpIncrement, v1, context.GetToken(G_INC));
            r1.Value = n1 = Increment(n1);
            return n1;
        }, INCE03, context);
    }

    private Evaluator HandlePostDecrementExpression(ParserRuleContext context)
    {
        var ref1 = HandleReferenceUpdateExpression(context);
        return TryCatch(scope =>
        {
            var v1 = ref1(scope);
            if(v1 is not GReference r1) throw FailOnInvalidLValueDecrement(DECT01,
                context.GetToken(G_DEC).Symbol);
            if(r1.Value is not IENumber n1) throw FailOnOperation(DECT02,
                OpDecrement, v1, context.GetToken(G_DEC));
            r1.Value = Decrement(n1);
            return n1;
        }, DECT03, context);
    }

    private Evaluator HandlePreDecrementExpression(ParserRuleContext context)
    {
        var ref1 = HandleReferenceUpdateExpression(context);
        return TryCatch(scope =>
        {
            var v1 = ref1(scope);
            if(v1 is not GReference r1) throw FailOnInvalidLValueDecrement(DECE01,
                context.GetToken(G_DEC).Symbol);
            if(r1.Value is not IENumber n1) throw FailOnOperation(DECE02,
                OpDecrement, v1, context.GetToken(G_DEC));
            r1.Value = n1 = Decrement(n1);
            return n1;
        }, DECE03, context);
    }

    public override Evaluator VisitMultiplicativeExpression(MultiplicativeExpressionContext context)
    {
        var expr1 = Visit(context.expression(0));
        var expr2 = Visit(context.expression(1));
        if(context.G_MUL() != null) return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            if(v1 is IEInteger i1 && v2 is IEInteger i2)
                return GInteger.From(i1.Value * i2.Value);
            if(v1 is IENumber n1 && v2 is IENumber n2)
                return GDouble.From(n1.ToDouble() * n2.ToDouble());
            throw FailOnOperation(MULT01, OpMultiplication, v1, v2, context.G_MUL());
        }, MULT02, context);

        if(context.G_DIV() != null) return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            if(v1 is IEInteger i1 && v2 is IEInteger i2)
                return GInteger.From(i1.Value / i2.Value);
            if(v1 is IENumber n1 && v2 is IENumber n2)
                return GDouble.From(n1.ToDouble() / n2.ToDouble());
            throw FailOnOperation(DIVD01, OpDivision, v1, v2, context.G_DIV());
        }, DIVD02, context);

        if(context.G_MOD() != null) return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            if(v1 is IEInteger i1 && v2 is IEInteger i2)
                return GInteger.From(i1.Value % i2.Value);
            if(v1 is IENumber n1 && v2 is IENumber n2)
                return GDouble.From(n1.ToDouble() % n2.ToDouble());
            throw FailOnOperation(MODU01, OpModulus, v1, v2, context.G_MOD());
        }, MODU02, context);

        throw new InvalidOperationException("Invalid parser state");
    }

    public override Evaluator VisitAdditiveExpression(AdditiveExpressionContext context)
    {
        var expr1 = Visit(context.expression(0));
        var expr2 = Visit(context.expression(1));
        if(context.G_PLUS() != null) return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            if(v1 is IEInteger i1 && v2 is IEInteger i2)
                return GInteger.From(i1.Value + i2.Value);
            if(v1 is IENumber n1 && v2 is IENumber n2)
                return GDouble.From(n1.ToDouble() + n2.ToDouble());
            if(v1 is IEString || v2 is IEString)
                return GString.From(Stringify(v1) + Stringify(v2));
            throw FailOnOperation(ADDT01, OpAddition, v1, v2, context.G_PLUS());
        }, ADDT02, context);

        if(context.G_MINUS() != null) return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            if(v1 is IEInteger i1 && v2 is IEInteger i2)
                return GInteger.From(i1.Value - i2.Value);
            if(v1 is IENumber n1 && v2 is IENumber n2)
                return GDouble.From(n1.ToDouble() - n2.ToDouble());
            throw FailOnOperation(SUBT01, OpSubtraction, v1, v2, context.G_MINUS());
        }, SUBT02, context);

        throw new InvalidOperationException("Invalid parser state");
    }

    public override Evaluator VisitRelationalExpression(RelationalExpressionContext context)
    {
        var expr1 = Visit(context.expression(0));
        var expr2 = Visit(context.expression(1));
        if(context.G_GT() != null) return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            if(v1 is not IENumber n1 || v2 is not IENumber n2)
                throw FailOnOperation(RELA01, OpComparison, v1, v2, context.G_GT());
            double d1 = n1.ToDouble(), d2 = n2.ToDouble();
            if(Runtime.AreEqual(d1, d2)) return FALSE;
            return d1 > d2 ? TRUE : FALSE;
        }, RELA05, context);

        if(context.G_GE() != null) return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            if(v1 is not IENumber n1 || v2 is not IENumber n2)
                throw FailOnOperation(RELA02, OpComparison, v1, v2, context.G_GE());
            double d1 = n1.ToDouble(), d2 = n2.ToDouble();
            if(Runtime.AreEqual(d1, d2)) return TRUE;
            return d1 > d2 ? TRUE : FALSE;
        }, RELA06, context);

        if(context.G_LT() != null) return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            if(v1 is not IENumber n1 || v2 is not IENumber n2)
                throw FailOnOperation(RELA03, OpComparison, v1, v2, context.G_LT());
            double d1 = n1.ToDouble(), d2 = n2.ToDouble();
            if(Runtime.AreEqual(d1, d2)) return FALSE;
            return d1 < d2 ? TRUE : FALSE;
        }, RELA07, context);

        if(context.G_LE() != null) return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            if(v1 is not IENumber n1 || v2 is not IENumber n2)
                throw FailOnOperation(RELA04, OpComparison, v1, v2, context.G_LE());
            double d1 = n1.ToDouble(), d2 = n2.ToDouble();
            if(Runtime.AreEqual(d1, d2)) return TRUE;
            return d1 < d2 ? TRUE : FALSE;
        }, RELA08, context);

        throw new InvalidOperationException("Invalid parser state");
    }

    public override Evaluator VisitEqualityExpression(EqualityExpressionContext context)
    {
        var expr1 = Visit(context.expression(0));
        var expr2 = Visit(context.expression(1));
        if(context.G_EQ() != null) return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            return AreEqual(v1, v2, Runtime) ? TRUE : FALSE;
        }, EQUL01, context);

        if(context.G_NE() != null) return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            return AreEqual(v1, v2, Runtime) ? FALSE : TRUE;
        }, NEQL01, context);

        throw new InvalidOperationException("Invalid parser state");
    }

    public override Evaluator VisitLogicalAndExpression(LogicalAndExpressionContext context)
    {
        var expr1 = Visit(context.expression(0));
        var expr2 = Visit(context.expression(1));
        return TryCatch(scope =>
        {
            var v1 = expr1(scope);
            if(!v1.ToBoolean()) return v1;
            return expr2(scope);
        }, ANDL01, context);
    }

    public override Evaluator VisitLogicalOrExpression(LogicalOrExpressionContext context)
    {
        var expr1 = Visit(context.expression(0));
        var expr2 = Visit(context.expression(1));
        return TryCatch(scope =>
        {
            var v1 = expr1(scope);
            if(v1.ToBoolean()) return v1;
            return expr2(scope);
        }, ORLG01, context);
    }

    public override Evaluator VisitAssignmentAugExpression(AssignmentAugExpressionContext context)
    {
        var ref1 = HandleReferenceUpdateExpression(context);
        var expr2 = Visit(context.expression()[^1]);
        if(context.G_ADD_ASSIGN() != null) return TryCatch(scope =>
        {
            var v2 = Dereference(expr2(scope));
            if(ref1(scope) is not GReference r1)
                throw FailOnInvalidLValueAssignment(ADDN01, context.Start);
            var v1 = r1.Value;
            if(v1 is IEInteger i1 && v2 is IEInteger i2)
                return r1.Value = GInteger.From(i1.Value + i2.Value);
            if(v1 is IENumber n1 && v2 is IENumber n2)
                return r1.Value = GDouble.From(n1.ToDouble() + n2.ToDouble());
            if(v1 is IEString || v2 is IEString)
                return r1.Value = GString.From(Stringify(v1) + Stringify(v2));
            throw FailOnOperation(ADDN02, OpAdditionAssignment, v1, v2, context.G_ADD_ASSIGN());
        }, ADDN03, context);

        if(context.G_SUB_ASSIGN() != null) return TryCatch(scope =>
        {
            var v2 = Dereference(expr2(scope));
            if(ref1(scope) is not GReference r1)
                throw FailOnInvalidLValueAssignment(SUBN01, context.Start);
            var v1 = r1.Value;
            if(v1 is IEInteger i1 && v2 is IEInteger i2)
                return r1.Value = GInteger.From(i1.Value - i2.Value);
            if(v1 is IENumber n1 && v2 is IENumber n2)
                return r1.Value = GDouble.From(n1.ToDouble() - n2.ToDouble());
            throw FailOnOperation(SUBN02, OpSubtractionAssignment, v1, v2, context.G_SUB_ASSIGN());
        }, SUBN03, context);

        if(context.G_MUL_ASSIGN() != null) return TryCatch(scope =>
        {
            var v2 = Dereference(expr2(scope));
            if(ref1(scope) is not GReference r1)
                throw FailOnInvalidLValueAssignment(MULN01, context.Start);
            var v1 = r1.Value;
            if(v1 is IEInteger i1 && v2 is IEInteger i2)
                return r1.Value = GInteger.From(i1.Value * i2.Value);
            if(v1 is IENumber n1 && v2 is IENumber n2)
                return r1.Value = GDouble.From(n1.ToDouble() * n2.ToDouble());
            throw FailOnOperation(MULN02, OpMultiplicationAssignment, v1, v2, context.G_MUL_ASSIGN());
        }, MULN03, context);

        if(context.G_DIV_ASSIGN() != null) return TryCatch(scope =>
        {
            var v2 = Dereference(expr2(scope));
            if(ref1(scope) is not GReference r1)
                throw FailOnInvalidLValueAssignment(DIVN01, context.Start);
            var v1 = r1.Value;
            if(v1 is IEInteger i1 && v2 is IEInteger i2)
                return r1.Value = GInteger.From(i1.Value / i2.Value);
            if(v1 is IENumber n1 && v2 is IENumber n2)
                return r1.Value = GDouble.From(n1.ToDouble() / n2.ToDouble());
            throw FailOnOperation(DIVN02, OpDivisionAssignment, v1, v2, context.G_DIV_ASSIGN());
        }, DIVN03, context);

        if(context.G_MOD_ASSIGN() != null) return TryCatch(scope =>
        {
            var v2 = Dereference(expr2(scope));
            if(ref1(scope) is not GReference r1)
                throw FailOnInvalidLValueAssignment(MODN01, context.Start);
            var v1 = r1.Value;
            if(v1 is IEInteger i1 && v2 is IEInteger i2)
                return r1.Value = GInteger.From(i1.Value % i2.Value);
            if(v1 is IENumber n1 && v2 is IENumber n2)
                return r1.Value = GDouble.From(n1.ToDouble() % n2.ToDouble());
            throw FailOnOperation(MODN02, OpModulusAssignment, v1, v2, context.G_MOD_ASSIGN());
        }, MODN03, context);

        throw new InvalidOperationException("Invalid parser state");
    }

    public override Evaluator VisitAssignmentBracketExpression(AssignmentBracketExpressionContext context)
    {
        var expr1 = Visit(context.expression(0));
        var expr2 = Visit(context.expression(1));
        var expr3 = Visit(context.expression(2));
        return TryCatch(scope =>
        {
            var v3 = expr3(scope);
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            if(v1 is IEArray a1 && v2 is IEInteger i2)
            {
                a1.Set((int) i2.Value, v3);
                return v3;
            }
            if(v1 is IEObject o1 && v2 is IEString s2)
            {
                o1.Set(s2.Value, v3);
                return v3;
            }
            ThrowBracketUpdateException(v1, v2, context);
            throw new InvalidOperationException("Invalid runtime state");
        }, ASIN01, context);
    }

    public override Evaluator VisitAssignmentDotExpression(AssignmentDotExpressionContext context)
    {
        var expr1 = Visit(context.expression(0));
        var id2 = context.G_IDENTIFIER();
        var expr3 = Visit(context.expression(1));
        var ids2 = id2.GetText();
        return TryCatch(scope =>
        {
            var v3 = expr3(scope);
            var v1 = Dereference(expr1(scope));
            if(v1 is IEObject o1)
            {
                o1.Set(ids2, v3);
                return v3;
            }
            throw FailOnOperation(ASIN02, OpPropertyAssignment, v1, context.G_DOT());
        }, ASIN03, context);
    }

    public override Evaluator VisitAssignmentIdExpression(AssignmentIdExpressionContext context)
    {
        var id1 = context.G_IDENTIFIER();
        var expr2 = Visit(context.expression());
        var ids1 = id1.GetText();
        return TryCatch(scope =>
        {
            var v2 = expr2(scope);
            var v1 = scope.Resolve(ids1) ?? throw FailOnIdentifierNotFound(IDEN03, id1.Symbol);
            if(v1 is not GReference r1) throw FailOnInvalidLValueAssignment(ASIN04, id1.Symbol);
            r1.Value = v2;
            return v2;
        }, ASIN05, context);
    }

    public override Evaluator VisitParenthesizedExpression(ParenthesizedExpressionContext context)
        => Visit(context.expression());
}