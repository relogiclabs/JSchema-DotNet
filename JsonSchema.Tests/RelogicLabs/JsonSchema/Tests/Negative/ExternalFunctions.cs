using RelogicLabs.JsonSchema.Functions;
using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Types;

namespace RelogicLabs.JsonSchema.Tests.Negative;

public class ExternalFunctions1
{
    public bool Odd(JNumber target)
    {
        bool result = target % 2 != 0;
        if(!result) throw new Exception("Not an odd number");
        return true;
    }
}

public class ExternalFunctions2 : FunctionBase
{
    public ExternalFunctions2(RuntimeContext runtime) : base(runtime) { }
    
    public void Odd(JNumber target)
    {
        bool result = target % 2 != 0;
        if(!result) throw new Exception("Not an odd number");
    }
}

public class ExternalFunctions3 : FunctionBase
{
    public ExternalFunctions3(RuntimeContext runtime) : base(runtime) { }
    
    public bool Odd() => throw new InvalidOperationException();
}

public class ExternalFunctions4 : FunctionBase
{
    public ExternalFunctions4(RuntimeContext runtime) : base(runtime) { }
}

public class ExternalFunctions5 : FunctionBase
{
    public ExternalFunctions5() : base(null!) { }
}