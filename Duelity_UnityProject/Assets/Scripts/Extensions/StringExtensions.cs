using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public static class StringExtensions
{
    public static bool HasSpecialChars(this string yourString)
    {
        if (yourString is null)
            throw new ArgumentNullException(nameof(yourString));

        return yourString.Any(ch => !char.IsLetterOrDigit(ch) && ch != '_' && ch != '.');
    }

    public static string RemoveSpecialCharacters(this string str)
    {
        return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
    }

    public static bool Contains(this string source, string toCheck, StringComparison comp)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        if (toCheck is null)
            throw new ArgumentNullException(nameof(toCheck));

        return source?.IndexOf(toCheck, comp) >= 0;
    }

    public static string ReplaceFirst(this string text, string search, string replace)
    {
        if (text is null)
            throw new ArgumentNullException(nameof(text));

        if (search is null)
            throw new ArgumentNullException(nameof(search));

        int pos = text.IndexOf(search);
        if (pos < 0)
            return text;
        return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
    }

    public static string DealWithDoubleQuotesFromCsv(this string text)
    {
        // 1 case: there are double quotes at the beginning and end
        // -> results in triple double quotes the beginning and end of the string
        // we want to remove 2 out of these 3

        // 2 case: there are double quotes somewhere inside the string
        // -> results in all of these double quotes being doubled
        // we want to remove the extra double quote

        // 3 case: both of the above
        StringBuilder sb = new StringBuilder(text);

        if (text.StartsWith("\""))
            sb.Remove(0, 1);

        if (text.EndsWith("\""))
            sb.Remove(sb.Length - 1, 1);

        return Regex.Replace(sb.ToString(), @"(""{2,3})", "\"");
    }

    public static string ReplaceValuePlaceholders(this string text)
    {
        return text.Replace("VALUE1", "{0}")
            .Replace("VALUE2", "{1}")
            .Replace("VALUE3", "{2}");
    }

    public static bool TryGetNumberOfFormatTokens(this string text, out int count)
    {
        count = 0;

        if (string.IsNullOrWhiteSpace(text))
            return false;

        MatchCollection matches = Regex.Matches(text, @"{[0-9]+?}");
        count = matches.Count;
        return count > 0;
    }
}
