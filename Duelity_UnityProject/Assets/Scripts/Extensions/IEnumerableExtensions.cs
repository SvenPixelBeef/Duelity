using System.Collections.Generic;
using System.Linq;

public static class IEnumerableExtensions
{
    /// <summary>
    /// Does a list contain all values of another list?
    /// </summary>
    /// <remarks>Needs .NET 3.5 or greater.  Source:  https://stackoverflow.com/a/1520664/1037948 </remarks>
    /// <typeparam name="T">list value type</typeparam>
    /// <param name="containingList">the larger list we're checking in</param>
    /// <param name="lookupList">the list to look for in the containing list</param>
    /// <returns>true if it has everything</returns>
    public static bool ContainsAll<T>(this IEnumerable<T> containingList, IEnumerable<T> lookupList)
    {
        return !lookupList.Except(containingList).Any();
    }

    public static string ToReadableString<T>(this IEnumerable<T> list)
    {
        string text = string.Empty;
        foreach (var item in list)
            text += $"{item}, ";

        return text;
    }
}
