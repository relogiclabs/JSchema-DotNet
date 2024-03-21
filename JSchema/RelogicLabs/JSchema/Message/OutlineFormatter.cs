namespace RelogicLabs.JSchema.Message;

public static class OutlineFormatter
{
    private const string AbbreviateMarker = "...";
    private static int _outlineLength = 200;
    private static int _startLength = 2 * _outlineLength / 3;
    private static int _endLength = _outlineLength / 3;
    public static int OutlineLength
    {
        get => _outlineLength;
        set
        {
            _outlineLength = value;
            _startLength = 2 * value / 3;
            _endLength = value / 3;
        }
    }

    public static string CreateOutline(object value)
    {
        var _string = value.ToString()
                ?? throw new InvalidOperationException("Invalid runtime state");
        return _outlineLength >= _string.Length ? _string
            : $"{_string[.._startLength]}{AbbreviateMarker}{_string[^_endLength..]}";
    }
}