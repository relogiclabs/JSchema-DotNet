using RelogicLabs.JSchema.Types;
using static RelogicLabs.JSchema.Types.IEValue;

namespace RelogicLabs.JSchema.Script;

internal sealed class GControl : IEValue
{
    private const int BreakFlag = 1;
    private const int ReturnFlag = 2;
    public static readonly GControl BREAK = new(BreakFlag, VOID);

    private readonly int _flag;
    public IEValue Value { get; }

    private GControl(int flag, IEValue value)
    {
        _flag = flag;
        Value = value;
    }

    public static GControl OfReturn(IEValue value) => new(ReturnFlag, value);
    public bool IsBreak() => _flag == BreakFlag;
    public bool IsReturn() => _flag == ReturnFlag;
    public IEValue ToIteration() => _flag == BreakFlag ? VOID : this;
    public IEValue ToFunction() => Value;
    public override string ToString() => Value.ToString()
            ?? throw new InvalidOperationException("Invalid runtime state");
}