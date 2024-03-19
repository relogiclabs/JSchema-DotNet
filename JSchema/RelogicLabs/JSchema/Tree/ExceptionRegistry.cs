using System.Collections;

namespace RelogicLabs.JSchema.Tree;

public sealed class ExceptionRegistry : IEnumerable<Exception>
{
    private int _disableException;
    private readonly List<Exception> _exceptions;

    internal List<Exception> TryBuffer { get; }
    public bool ThrowException { get; set; }
    public int CutoffLimit { get; set; } = 500;
    public int Count => _exceptions.Count;

    internal ExceptionRegistry(bool throwException)
    {
        _exceptions = new List<Exception>();
        ThrowException = throwException;
        TryBuffer = new List<Exception>();
    }

    private bool AddException(List<Exception> list, Exception exception)
    {
        if(list.Count <= CutoffLimit) list.Add(exception);
        return false;
    }

    internal bool Fail(Exception exception)
    {
        if(_disableException > 0) return AddException(TryBuffer, exception);
        if(ThrowException) throw exception;
        return AddException(_exceptions, exception);
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
            if(_disableException >= 1) _disableException -= 1;
            else throw new InvalidOperationException("Invalid runtime state");
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<Exception> GetEnumerator() => _exceptions.GetEnumerator();

    public void Clear()
    {
        _exceptions.Clear();
        TryBuffer.Clear();
    }
}