using System.Collections;

namespace RelogicLabs.JsonSchema.Tree;

public class ExceptionRegistry : IEnumerable<Exception>
{
    private int _disableException;

    public bool ThrowException { get; set; }
    public int CutoffLimit { get; set; } = 200;
    public Queue<Exception> Exceptions { get; }


    internal ExceptionRegistry(bool throwException)
    {
        ThrowException = throwException;
        Exceptions = new Queue<Exception>();
    }

    public void TryAdd(Exception exception)
    {
        if(_disableException == 0 && Exceptions.Count < CutoffLimit)
            Exceptions.Enqueue(exception);
    }

    public void TryThrow(Exception exception)
    {
        if(ThrowException && _disableException == 0) throw exception;
    }

    internal T TryExecute<T>(Func<T> function)
    {
        try
        {
            _disableException += 1;
            return function();
        }
        finally
        {
            _disableException -= 1;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<Exception> GetEnumerator() => Exceptions.GetEnumerator();
    public void Clear() => Exceptions.Clear();
}