using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using RelogicLabs.JSchema.Antlr;
using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Script;
using RelogicLabs.JSchema.Tree;
using RelogicLabs.JSchema.Types;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Engine.ScriptErrorHelper;
using static RelogicLabs.JSchema.Engine.ScriptTreeHelper;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Types.IENull;
using static RelogicLabs.JSchema.Types.IEUndefined;
using static RelogicLabs.JSchema.Types.IEValue;

namespace RelogicLabs.JSchema.Engine;

internal sealed partial class ScriptTreeVisitor : SchemaParserBaseVisitor<Evaluator>
{
    private static readonly GBoolean TRUE = GBoolean.TRUE;
    private static readonly GBoolean FALSE = GBoolean.FALSE;

    private static readonly Evaluator VoidSupplier = static _ => VOID;
    private static readonly Evaluator TrueSupplier = static _ => TRUE;
    private static readonly Evaluator FalseSupplier = static _ => FALSE;
    private static readonly Evaluator NullSupplier = static _ => NULL;
    private static readonly Evaluator UndefinedSupplier = static _ => UNDEFINED;
    private static readonly Evaluator BreakSupplier = static _ => GControl.BREAK;

    public RuntimeContext Runtime { get; }
    public ParseTreeProperty<Evaluator> Scripts { get; }
    private Type? _returnType;

    public ScriptTreeVisitor(RuntimeContext runtime)
    {
        Runtime = runtime;
        Scripts = new ParseTreeProperty<Evaluator>();
    }

    public override Evaluator VisitCompleteSchema(SchemaParser.CompleteSchemaContext context)
    {
        var scripts = context.scriptNode().Select(ProcessScript).ToList();
        if(scripts.IsEmpty()) return VoidSupplier;
        return TryCatch(scope =>
        {
            foreach(var eval in scripts) eval(scope);
            return VOID;
        }, SRPT01, context);
    }

    private Evaluator ProcessScript(ParserRuleContext context)
    {
        var evaluator = Visit(context);
        Scripts.Put(context, evaluator);
        return evaluator;
    }

    public override Evaluator VisitShortSchema(SchemaParser.ShortSchemaContext context)
        => VoidSupplier;

    public override Evaluator VisitScriptNode(SchemaParser.ScriptNodeContext context)
    {
        var statements = context.globalStatement().Select(Visit).ToList();
        return TryCatch(scope =>
        {
            foreach(var eval in statements) eval(scope);
            return VOID;
        }, SRPT02, context);
    }

    public override Evaluator VisitFunctionDeclaration(SchemaParser.FunctionDeclarationContext context)
    {
        var baseName = context.name.Text;
        var mode = GetFunctionMode(context.G_CONSTRAINT(), context.G_FUTURE(), context.G_SUBROUTINE());
        var parameters = ToParameters(context.G_IDENTIFIER()[1..], context.G_ELLIPSIS());
        var constraint = IsConstraint(mode);
        var functionName = constraint
            ? ToConstraintName(FormatFunctionName(baseName, parameters))
            : FormatFunctionName(baseName, parameters);
        if(constraint) _returnType = typeof(IEBoolean);
        var functionBody = Visit(context.blockStatement());
        _returnType = null;
        var function = new GFunction(parameters, functionBody, mode);
        return TryCatch(scope =>
        {
            scope.AddFunction(functionName, function);
            if(constraint) Runtime.Functions.AddFunction(
                new ScriptFunction(baseName, function));
            return VOID;
        }, FUNS07, context);
    }

    public override Evaluator VisitVarStatement(SchemaParser.VarStatementContext context)
    {
        var varInitializations = context.varInitialization().Select(Visit).ToList();
        return TryCatch(scope =>
        {
            foreach(var eval in varInitializations) eval(scope);
            return VOID;
        }, VARD03, context);
    }

    public override Evaluator VisitVarInitialization(SchemaParser.VarInitializationContext context)
    {
        var varName = context.G_IDENTIFIER().GetText();
        var expression = Visit(context.expression(), VoidSupplier);
        return TryCatch(scope =>
        {
            scope.AddVariable(varName, expression(scope));
            return VOID;
        }, VRIN01, context);
    }

    public override Evaluator VisitExpressionStatement(SchemaParser.ExpressionStatementContext context)
    {
        var expression = Visit(context.expression());
        return TryCatch(scope =>
        {
            expression(scope);
            return VOID;
        }, EXPR01, context);
    }

    public override Evaluator VisitIfStatement(SchemaParser.IfStatementContext context)
    {
        var condition = Visit(context.expression());
        var thenStatement = Visit(context.statement(0));
        if(context.G_ELSE() == null) return TryCatch(scope =>
        {
            if(condition(scope).ToBoolean())
                return thenStatement(scope);
            return VOID;
        }, IFST01, context);

        var elseStatement = Visit(context.statement(1));
        return TryCatch(scope =>
        {
            if(condition(scope).ToBoolean())
                return thenStatement(scope);
            return elseStatement(scope);
        }, IFST02, context);
    }

    public override Evaluator VisitWhileStatement(SchemaParser.WhileStatementContext context)
    {
        var condition = Visit(context.expression());
        var statement = Visit(context.statement());
        return TryCatch(scope =>
        {
            while(condition(scope).ToBoolean())
            {
                var result = statement(scope);
                if(result is GControl ctrl) return ctrl.ToIteration();
            }
            return VOID;
        }, WHIL01, context);
    }

    public override Evaluator VisitForStatement(SchemaParser.ForStatementContext context)
    {
        var initialization = Visit(context.varStatement(),
            Visit(context.initialization, VoidSupplier));
        var condition = Visit(context.condition, TrueSupplier);
        var updation = Visit(context.updation, VoidSupplier);
        var statement = Visit(context.statement());
        return TryCatch(scope =>
        {
            var forScope = new ScopeContext(scope);
            for(initialization(forScope);
                condition(forScope).ToBoolean();
                updation(forScope))
            {
                var result = statement(forScope);
                if(result is GControl ctrl) return ctrl.ToIteration();
            }
            return VOID;
        }, FORS01, context);
    }

    public override Evaluator VisitExpressionList(SchemaParser.ExpressionListContext context)
    {
        var expressions = context.expression().Select(Visit).ToList();
        return TryCatch(scope =>
        {
            foreach(var eval in expressions) eval(scope);
            return VOID;
        }, EXPR02, context);
    }

    public override Evaluator VisitForeachStatement(SchemaParser.ForeachStatementContext context)
    {
        var varName = context.G_IDENTIFIER().GetText();
        var collection = Visit(context.expression());
        var statement = Visit(context.statement());
        return TryCatch(scope =>
        {
            var forScope = new ScopeContext(scope);
            var reference = forScope.AddVariable(varName, VOID);
            foreach(var v in new GIterator(collection(scope)))
            {
                reference.Value = v;
                var result = statement(forScope);
                if(result is GControl ctrl) return ctrl.ToIteration();
            }
            return VOID;
        }, FREC01, context);
    }

    public override Evaluator VisitReturnStatement(SchemaParser.ReturnStatementContext context)
    {
        var expression = Visit(context.expression());
        if(_returnType == null) return TryCatch(scope => GControl.OfReturn(
            expression(scope)), RETN02, context);
        var thisReturnType = _returnType;
        return TryCatch(scope =>
        {
            var v1 = expression(scope);
            if(!thisReturnType.IsInstanceOfType(v1))
                throw FailOnInvalidReturnType(v1, context.expression().Start);
            return GControl.OfReturn(v1);
        }, RETN03, context);
    }

    public override Evaluator VisitBreakStatement(SchemaParser.BreakStatementContext context)
        => BreakSupplier;

    public override Evaluator VisitBlockStatement(SchemaParser.BlockStatementContext context)
    {
        var statements = context.statement().Select(Visit).ToList();
        return TryCatch(scope =>
        {
            var blockScope = new ScopeContext(scope);
            foreach(var eval in statements)
            {
                var result = eval(blockScope);
                if(result is GControl ctrl) return ctrl;
            }
            return VOID;
        }, BLOK01, context);
    }

    public override Evaluator VisitTrueLiteral(SchemaParser.TrueLiteralContext context)
        => TrueSupplier;

    public override Evaluator VisitFalseLiteral(SchemaParser.FalseLiteralContext context)
        => FalseSupplier;

    public override Evaluator VisitNullLiteral(SchemaParser.NullLiteralContext context)
        => NullSupplier;

    public override Evaluator VisitUndefinedLiteral(SchemaParser.UndefinedLiteralContext context)
        => UndefinedSupplier;

    public override Evaluator VisitIntegerLiteral(SchemaParser.IntegerLiteralContext context)
    {
        var value = GInteger.Of(Convert.ToInt64(context.G_INTEGER().GetText()));
        return _ => value;
    }

    public override Evaluator VisitDoubleLiteral(SchemaParser.DoubleLiteralContext context)
    {
        var value = GDouble.Of(Convert.ToDouble(context.G_DOUBLE().GetText()));
        return _ => value;
    }

    public override Evaluator VisitStringLiteral(SchemaParser.StringLiteralContext context)
    {
        var value = GString.Of(context.GetText().ToEncoded());
        return _ => value;
    }

    public override Evaluator VisitArrayLiteral(SchemaParser.ArrayLiteralContext context)
    {
        var list = context.expression().Select(Visit).ToList();
        return TryCatch(scope => new GArray(list.Select(eval
            => eval(scope)).ToList()), ARRL01, context);
    }

    public override Evaluator VisitObjectLiteral(SchemaParser.ObjectLiteralContext context)
    {
        var keys = context._keys.Select(static k => k.Type == SchemaLexer.G_STRING
            ? k.Text.ToEncoded() : k.Text).ToList();
        var values = context._values.Select(Visit).ToList();
        return TryCatch(scope => new GObject(keys, values.Select(eval
            => eval(scope)).ToList()), OBJL01, context);
    }

    private static Evaluator TryCatch(Evaluator evaluator, string code, ParserRuleContext context)
    {
        return scope =>
        {
            try
            {
                return evaluator(scope);
            }
            catch(Exception e) when(e is ScriptRuntimeException or ScriptTemplateException)
            {
                throw;
            }
            catch(CommonException e)
            {
                throw FailOnRuntime(e.Code, e.Message, context.Start, e);
            }
            catch(Exception e)
            {
                throw FailOnSystemException(code, e, context.Start);
            }
        };
    }

    private Evaluator Visit(IParseTree? tree, Evaluator defaultValue)
        => tree == null ? defaultValue : base.Visit(tree);
}