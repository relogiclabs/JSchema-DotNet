using System.Text;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Time.JsonDateTime;

namespace RelogicLabs.JSchema.Time;

public class JsonUtcOffset
{
    private const int DEFAULT_UTC_OFFSET_HOUR = 0;
    private const int DEFAULT_UTC_OFFSET_MINUTE = 0;

    internal static readonly JsonUtcOffset DefaultUtcOffset = new();

    public int Hour { get; }
    public int Minute { get; }

    internal TimeSpan TimeSpan { get; }

    public JsonUtcOffset() : this(UNSET, UNSET) { }

    public JsonUtcOffset(int hour, int minute)
    {
        Hour = hour;
        Minute = minute;

        TimeSpan = new TimeSpan(
            DefaultIfUnset(hour, DEFAULT_UTC_OFFSET_HOUR),
            DefaultIfUnset(minute, DEFAULT_UTC_OFFSET_MINUTE), 0);
    }

    public override string ToString()
    {
        StringBuilder builder = new();
        if(Hour != UNSET) builder.Append($"UTC Offset Hour: {Hour}, ");
        if(Minute != UNSET) builder.Append($"UTC Offset Minute: {Minute}, ");
        return builder.ToString().RemoveEnd(", ");
    }
}