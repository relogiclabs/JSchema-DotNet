using System.Reflection;
using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Tree;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.MessageFormatter;

namespace RelogicLabs.JSchema.Functions;

internal static class FunctionLoader
{
    public static FunctionMap Load(Type providerImpl, Context? context = null)
    {
        var instance = CreateInstance(providerImpl, context);
        var providerBase = typeof(FunctionProvider);
        var functions = new FunctionMap();
        foreach(var m in providerImpl.GetMethods())
        {
            // Methods in ancestor class or in base class
            if(!providerBase.IsAssignableFrom(m.DeclaringType)
               || providerBase == m.DeclaringType) continue;
            ParameterInfo[] parameters = m.GetParameters();
            if(!IsValidReturnType(m.ReturnType)) throw failOnReturnType(m, context);
            if(parameters.Length == 0 || parameters[0].IsParams())
                throw failOnTargetParameter(m, context);
            functions.Add(new NativeFunction(m, parameters, instance));
        }
        return functions;
    }

    private static FunctionProvider CreateInstance(Type type, Context? context)
    {
        try
        {
            var instance = (FunctionProvider?) Activator.CreateInstance(type);
            if(instance != null) return instance;
        }
        catch(TargetInvocationException ex) { throw FailOnCreateInstance(INST01, ex, type, context); }
        catch(MissingMethodException ex) { throw FailOnCreateInstance(INST02, ex, type, context); }
        catch(MethodAccessException ex) { throw FailOnCreateInstance(INST03, ex, type, context); }
        catch(MemberAccessException ex) { throw FailOnCreateInstance(INST04, ex, type, context); }
        catch(TypeLoadException ex) { throw FailOnCreateInstance(INST05, ex, type, context); }
        catch(FileNotFoundException ex) { throw FailOnCreateInstance(INST06, ex, type, context); }
        catch(NotSupportedException ex) { throw FailOnCreateInstance(INST07, ex, type, context); }
        catch(BadImageFormatException ex) { throw FailOnCreateInstance(INST08, ex, type, context); }
        catch(FileLoadException ex) { throw FailOnCreateInstance(INST09, ex, type, context); }
        catch(Exception ex) { throw FailOnCreateInstance(INST10, ex, type, context); }
        throw FailOnCreateInstance(INST11, null, type, context);
    }

    private static bool IsValidReturnType(Type type)
    {
        if(type == typeof(bool)) return true;
        if(type == typeof(FutureFunction)) return true;
        return false;
    }

    private static ClassInstantiationException FailOnCreateInstance(string code, Exception? ex,
                Type type, Context? context)
        => new(FormatForSchema(code, $"Fail to create instance of {type.FullName}", context), ex);

    private static InvalidFunctionException failOnReturnType(MethodInfo method, Context? context)
        => new(FormatForSchema(FUNC01, $"Function [{
            method.GetSignature()}] requires valid return type", context));

    private static InvalidFunctionException failOnTargetParameter(MethodInfo method, Context? context)
        => new(FormatForSchema(FUNC02, $"Function [{
            method.GetSignature()}] requires valid target parameter", context));
}