using System;
using System.Collections.Generic;
using System.Linq;

public static class RandomExtensions
{
    private static Random rng = new Random();

    public static T RandomElement<T>(this ICollection<T> list)
    {
        if (list.Count == 0)
            return default;
        return list.ElementAt(rng.Next(list.Count));
    }

    public static T RandomElement<T>(this IEnumerable<T> collection)
    {
        if (collection.Count() == 0)
            return default;
        return collection.ElementAt(rng.Next(collection.Count()));
    }

    public static void Shuffle<T>(this T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
}