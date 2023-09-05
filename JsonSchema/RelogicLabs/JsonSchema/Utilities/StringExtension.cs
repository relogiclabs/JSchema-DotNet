using System.Text;
using RelogicLabs.JsonSchema.Types;

namespace RelogicLabs.JsonSchema.Utilities;

internal static class StringExtension
{
    public static string Truncate(this string source, int length, string suffix = "...") 
        => source.Length <= length? source : source[..(length - 1)] + suffix;

    public static string ToString(this string source, string prefix = "", string suffix = "") 
        => $"{prefix}{source}{suffix}";

    public static string DoubleQuote(this string source)
        => $"\"{source}\"";
    
    public static string DoubleQuote(this JString source)
        => $"\"{source.Value}\"";
    
    public static string ToUpperFirstLetter(this string source) 
        => source[..1].ToUpper() + source[1..];
    
    public static string ToLowerFirstLetter(this string source) 
        => source[..1].ToLower() + source[1..];
    
    public static string ToEncoded(this string source)
    {
        StringBuilder builder = new();
        if(source.StartsWith('"') && source.EndsWith('"')) 
            source = source[1..^1];
        
        for(int i = 0; i < source.Length; i++)
        {
            char current = source[i];
            if(current == '\\')
            {
                char next = source[i + 1];
                switch(next)
                {
                    case '"': builder.Append('"'); i++; break;
                    case '\\': builder.Append('\\'); i++; break;
                    case '/': builder.Append('/'); i++; break;
                    case 'b': builder.Append('\b'); i++; break;
                    case 'f': builder.Append('\f'); i++; break;
                    case 'n': builder.Append('\n'); i++; break;
                    case 'r': builder.Append('\r'); i++; break;
                    case 't': builder.Append('\t'); i++; break;
                    case 'u': builder.Append((char) Convert.ToInt32(
                        source[(i + 2)..(i + 6)], 16)); i += 5; break;
                }
            } else builder.Append(current);
        }
        return builder.ToString();
    }
    
    public static string ToOutline(this string source, int outlineLength = 200)
    {
        int front = 2 * outlineLength / 3;
        int back = 1 * outlineLength / 3;
        if(front + back >= source.Length) return source;
        StringBuilder builder = new();
        return builder.Append(source[..front]).Append("...")
            .Append(source[^back..]).ToString();
    }
}