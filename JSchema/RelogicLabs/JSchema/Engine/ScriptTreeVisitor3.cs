using RelogicLabs.JSchema.Antlr;
using RelogicLabs.JSchema.Script;
using RelogicLabs.JSchema.Types;
using static RelogicLabs.JSchema.Engine.ScriptErrorHelper;
using static RelogicLabs.JSchema.Engine.ScriptTreeHelper;
using static RelogicLabs.JSchema.Message.ErrorCode;

namespace RelogicLabs.JSchema.Engine;

internal sealed partial class ScriptTreeVisitor
{
    public override Evaluator VisitUnaryMinusExpression(SchemaParser.UnaryMinusExpressionContext context)
    {
        var expr1 = Visit(context.expression());
        return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            if(v1 is IEInteger i1) return GInteger.Of(-i1.Value);
            if(v1 is IENumber n1) return GDouble.Of(-n1.ToDouble());
            throw FailOnOperation(NEGT01, NEGATION, v1, context.G_MINUS());
        }, NEGT02, context);
    }

    public override Evaluator VisitLogicalNotExpression(SchemaParser.LogicalNotExpressionContext context)
    {
        var expr1 = Visit(context.expression());
        return TryCatch(scope => expr1(scope).ToBoolean() ? FALSE : TRUE, NOTL01, context);
    }

    public override Evaluator VisitPostIncrementExpression(SchemaParser.PostIncrementExpressionContext context)
    {
        var ref1 = Visit(context.refExpression());
        return TryCatch(scope =>
        {
            var v1 = ref1(scope);
            if(v1 is not GReference r1) throw FailOnInvalidLValueIncrement(INCR01,
                context.refExpression().Start);
            if(r1.Value is not IENumber n1) throw FailOnOperation(INCR02,
                INCREMENT, v1, context.G_INC());
            r1.Value = Increment(n1);
            return n1;
        }, INCR05, context);
    }

    public override Evaluator VisitPreIncrementExpression(SchemaParser.PreIncrementExpressionContext context)
    {
        var ref1 = Visit(context.refExpression());
        return TryCatch(scope =>
        {
            var v1 = ref1(scope);
            if(v1 is not GReference r1) throw FailOnInvalidLValueIncrement(INCR03,
                context.refExpression().Start);
            if(r1.Value is not IENumber n1) throw FailOnOperation(INCR04,
                INCREMENT, v1, context.G_INC());
            r1.Value = n1 = Increment(n1);
            return n1;
        }, INCR06, context);
    }

    public override Evaluator VisitPostDecrementExpression(SchemaParser.PostDecrementExpressionContext context)
    {
        var ref1 = Visit(context.refExpression());
        return TryCatch(scope =>
        {
            var v1 = ref1(scope);
            if(v1 is not GReference r1) throw FailOnInvalidLValueDecrement(DECR01,
                context.refExpression().Start);
            if(r1.Value is not IENumber n1) throw FailOnOperation(DECR02,
                DECREMENT, v1, context.G_DEC());
            r1.Value = Decrement(n1);
            return n1;
        }, DECR05, context);
    }

    public override Evaluator VisitPreDecrementExpression(SchemaParser.PreDecrementExpressionContext context)
    {
        var ref1 = Visit(context.refExpression());
        return TryCatch(scope =>
        {
            var v1 = ref1(scope);
            if(v1 is not GReference r1) throw FailOnInvalidLValueDecrement(DECR03,
                context.refExpression().Start);
            if(r1.Value is not IENumber n1) throw FailOnOperation(DECR04,
                DECREMENT, v1, context.G_DEC());
            r1.Value = n1 = Decrement(n1);
            return n1;
        }, DECR06, context);
    }

    public override Evaluator VisitMultiplicativeExpression(SchemaParser.MultiplicativeExpressionContext context)
    {
        var expr1 = Visit(context.expression(0));
        var expr2 = Visit(context.expression(1));
        if(context.G_MUL() != null) return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            if(v1 is IEInteger i1 && v2 is IEInteger i2)
                return GInteger.Of(i1.Value * i2.Value);
            if(v1 is IENumber n1 && v2 is IENumber n2)
                return GDouble.Of(n1.ToDouble() * n2.ToDouble());
            throw FailOnOperation(MULT01, MULTIPLICATION, v1, v2, context.G_MUL());
        }, MULT02, context);

        if(context.G_DIV() != null) return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            if(v1 is IEInteger i1 && v2 is IEInteger i2)
                return GInteger.Of(i1.Value / i2.Value);
            if(v1 is IENumber n1 && v2 is IENumber n2)
                return GDouble.Of(n1.ToDouble() / n2.ToDouble());
            throw FailOnOperation(DIVD01, DIVISION, v1, v2, context.G_DIV());
        }, DIVD02, context);

        throw new InvalidOperationException("Invalid parser state");
    }

    public override Evaluator VisitAdditiveExpression(SchemaParser.AdditiveExpressionContext context)
    {
        var expr1 = Visit(context.expression(0));
        var expr2 = Visit(context.expression(1));
        if(context.G_PLUS() != null) return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            if(v1 is IEInteger i1 && v2 is IEInteger i2)
                return GInteger.Of(i1.Value + i2.Value);
            if(v1 is IENumber n1 && v2 is IENumber n2)
                return GDouble.Of(n1.ToDouble() + n2.ToDouble());
            if(v1 is IEString || v2 is IEString)
                return GString.Of(Stringify(v1) + Stringify(v2));
            throw FailOnOperation(ADDT01, ADDITION, v1, v2, context.G_PLUS());
        }, ADDT02, context);

        if(context.G_MINUS() != null) return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            if(v1 is IEInteger i1 && v2 is IEInteger i2)
                return GInteger.Of(i1.Value - i2.Value);
            if(v1 is IENumber n1 && v2 is IENumber n2)
                return GDouble.Of(n1.ToDouble() - n2.ToDouble());
            throw FailOnOperation(SUBT01, SUBTRACTION, v1, v2, context.G_MINUS());
        }, SUBT02, context);

        throw new InvalidOperationException("Invalid parser state");
    }

    public override Evaluator VisitRelationalExpression(SchemaParser.RelationalExpressionContext context)
    {
        var expr1 = Visit(context.expression(0));
        var expr2 = Visit(context.expression(1));
        if(context.G_GT() != null) return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            if(v1 is not IENumber n1 || v2 is not IENumber n2)
                throw FailOnOperation(RELA01, COMPARISON, v1, v2, context.G_GT());
            double d1 = n1.ToDouble(), d2 = n2.ToDouble();
            if(Runtime.AreEqual(d1, d2)) return FALSE;
            return d1 > d2 ? TRUE : FALSE;
        }, RELA05, context);

        if(context.G_GE() != null) return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            if(v1 is not IENumber n1 || v2 is not IENumber n2)
                throw FailOnOperation(RELA02, COMPARISON, v1, v2, context.G_GE());
            double d1 = n1.ToDouble(), d2 = n2.ToDouble();
            if(Runtime.AreEqual(d1, d2)) return TRUE;
            return d1 > d2 ? TRUE : FALSE;
        }, RELA06, context);

        if(context.G_LT() != null) return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            if(v1 is not IENumber n1 || v2 is not IENumber n2)
                throw FailOnOperation(RELA03, COMPARISON, v1, v2, context.G_LT());
            double d1 = n1.ToDouble(), d2 = n2.ToDouble();
            if(Runtime.AreEqual(d1, d2)) return FALSE;
            return d1 < d2 ? TRUE : FALSE;
        }, RELA07, context);

        if(context.G_LE() != null) return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            if(v1 is not IENumber n1 || v2 is not IENumber n2)
                throw FailOnOperation(RELA04, COMPARISON, v1, v2, context.G_LE());
            double d1 = n1.ToDouble(), d2 = n2.ToDouble();
            if(Runtime.AreEqual(d1, d2)) return TRUE;
            return d1 < d2 ? TRUE : FALSE;
        }, RELA08, context);

        throw new InvalidOperationException("Invalid parser state");
    }

    public override Evaluator VisitEqualityExpression(SchemaParser.EqualityExpressionContext context)
    {
        var expr1 = Visit(context.expression(0));
        var expr2 = Visit(context.expression(1));
        if(context.G_EQ() != null) return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            return GBoolean.Of(AreEqual(v1, v2, Runtime));
        }, EQUL01, context);

        if(context.G_NE() != null) return TryCatch(scope =>
        {
            var v1 = Dereference(expr1(scope));
            var v2 = Dereference(expr2(scope));
            return GBoolean.Of(!AreEqual(v1, v2, Runtime));
        }, NEQL01, context);

        throw new InvalidOperationException("Invalid parser state");
    }

    public override Evaluator VisitLogicalAndExpression(SchemaParser.LogicalAndExpressionContext context)
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

    public override Evaluator VisitLogicalOrExpression(SchemaParser.LogicalOrExpressionContext context)
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

    public override Evaluator VisitAssignmentExpression(SchemaParser.AssignmentExpressionContext context)
    {
        var ref1 = Visit(context.refExpression());
        var expr2 = Visit(context.expression());
        return TryCatch(scope =>
        {
            var v2 = expr2(scope);
            var v1 = ref1(scope);
            if(v1 is not GReference r1) throw FailOnInvalidLValueAssignment(
                context.refExpression().Start);
            r1.Value = v2;
            return v2;
        }, ASIN02, context);
    }

    public override Evaluator VisitParenthesizedExpression(SchemaParser.ParenthesizedExpressionContext context)
        => Visit(context.expression());
}