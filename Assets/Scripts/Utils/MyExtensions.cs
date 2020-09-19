using System;
using System.Collections.Generic;

public static class MyExtensions
{
    public static void Shuffle<T>(this IList<T> list, Random rnd, int startIndex, int endIndex)
    {
        for (var i = endIndex + 1; i > startIndex; i--)
            list.Swap(startIndex, rnd.Next(startIndex, i));
    }

    public static void Swap<T>(this IList<T> list, int i, int j)
    {
        var temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }
}
