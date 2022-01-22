using System.Collections.Generic;

public static class DictionaryExtensions
{
    public static string ToReadableString<T, U>(this Dictionary<T, U> dict)
    {
        string text = string.Empty;
        foreach (var item in dict)
            text += $"Key: {item.Key}  || Value: {item.Value}\n";

        return text;
    }
}