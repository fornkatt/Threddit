using System.Text.RegularExpressions;

namespace Threddit.Domain.Extensions;

public static partial class StringExtensions
{
    [GeneratedRegex(@"[^a-z0-9\s-]")]
    private static partial Regex FindInvalidCharacters();
    
    [GeneratedRegex(@"\s+")]
    private static partial Regex FindSpaces();
    
    public static string GenerateSlug(this string input)
    {
        var str = input.ToLowerInvariant();
        str = FindInvalidCharacters().Replace(str, string.Empty);
        str = FindSpaces().Replace(str, " ");
        const int maxLength = 100;
        str = str.Length <= maxLength ? str : str[..maxLength].Trim();
        str = FindSpaces().Replace(str, "-");
        
        return $"{str}";
    }
}