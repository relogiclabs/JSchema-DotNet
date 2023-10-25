using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Message;

internal class MatchReport
{
    public static readonly MatchReport Success = new();
    public static readonly MatchReport TypeError = new(DTYP04, DTYP06);
    public static readonly MatchReport ArgumentError = new(DTYP05, DTYP07);
    public static readonly MatchReport AliasError = new(DEFI03, DEFI04);

    private readonly string _code1;
    private readonly string _code2;

    private MatchReport(string code1, string code2)
    {
        _code1 = code1;
        _code2 = code2;
    }

    private MatchReport() : this(string.Empty, string.Empty) { }

    public string GetCode(bool nested)
        => nested ? _code2 : _code1;
}