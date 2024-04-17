using RelogicLabs.JSchema.Functions;
using RelogicLabs.JSchema.Nodes;

// Functions for negative (error) test cases
namespace RelogicLabs.JSchema.Tests.External;

public class ExternalFunctions1
{
    public bool Odd(JNumber target)
    {
        bool result = target % 2 != 0;
        if(!result) throw new Exception("Not an odd number");
        return true;
    }
}

public class ExternalFunctions2 : FunctionProvider
{
    public void Odd(JNumber target)
    {
        bool result = target % 2 != 0;
        if(!result) throw new Exception("Not an odd number");
    }
}

public class ExternalFunctions3 : FunctionProvider
{
    public bool Odd() => throw new InvalidOperationException();
}

public class ExternalFunctions4 : FunctionProvider
{
    public bool CanTest(JNumber target)
    {
        // If you just want to throw any exception without details
        return Fail(new Exception("something went wrong"));
    }
}

public class ExternalFunctions5 : FunctionProvider
{
    public ExternalFunctions5() => throw new Exception("Error occurred");
}