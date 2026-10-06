using System;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtensions
{
    private static System.Random random = new System.Random();

    public static T GetRandom<T>(this T[] source)
    {
        if (source == null) throw new ArgumentNullException("source");
        if (source.Length == 0) throw new Exception("GetRandom can't be called since list has no values");
        return source[random.Next(0, source.Length)];
    }

    public static T? GetRandom_EnsureNotSameAsLast<T>(this T[] source, T last)
    {
        if (source == null) throw new ArgumentNullException("source");
        if (source.Length == 0) throw new Exception("GetRandomNoRepeat can't be called since list has no values");
        if (source.Length == 1) throw new Exception("GetRandomNoRepeat can't be called since to not repeat it needs at least 2 values");

        //get a random one and ensure that it's not the same as the last element
        var next = source[random.Next(0, source.Length)];
        while (next != null && next.Equals(last))
        {
            next = source[random.Next(0, source.Length)];
        }

        return next;
    }

    public static T[] Shuffle<T>(this T[] sourceList)
    {
        if (sourceList == null)
        {
            throw new ArgumentNullException(nameof(sourceList));
        }
        var newList = new T[sourceList.Length];
        var size = newList.Length;
        for (var i = 0; i < size; i++)
        {
            var temp = newList[i];
            var randIndex = random.Next(0, size);
            newList[i] = newList[randIndex];
            newList[randIndex] = temp;
        }

        return newList;
    }

    public static void Swap<T>(this T[] sourceList, int firstIndex, int secondIndex)
    {
        if (sourceList == null)
        {
            throw new ArgumentNullException(nameof(sourceList));
        }

        if (sourceList.Length < 2)
        {
            throw new ArgumentException("List count should be at least 2 for a swap.");
        }

        if (firstIndex < 0 || firstIndex >= sourceList.Length || secondIndex < 0 || secondIndex >= sourceList.Length)
        {
            throw new Exception($"Indexes {firstIndex} and {secondIndex} need to be within the range of " +
                $"the sourceList which has {sourceList.Length} elements.");
        }

        var firstValue = sourceList[firstIndex];
        sourceList[firstIndex] = sourceList[secondIndex];
        sourceList[secondIndex] = firstValue;
    }

    public static T[] RemoveNullEntries<T>(this T[] list)
    {
        var newList = new List<T>(list);

        for (var i = newList.Count - 1; i >= 0; i--)
        {
            if (Equals(newList[i], null))
            {
                newList.RemoveAt(i);
            }
        }

        return newList.ToArray();
    }

    public static bool TryGetValue<T>(this T[] values, int index, out T? result)
    {
        if (index >= 0 && index < values.Length)
        {
            result = values[index];
            return true;
        }
        result = default;
        return false;
    }

    public static bool TryGetValue<T>(this T[,] values, Vector2Int index, out T? result)
    {
        if (values.IsIndexInBounds(index))
        {
            result = values[index.x, index.y];
            return true;
        }
        result = default;
        return false;
    }

    public static bool IsIndexInBounds<T>(this T[,] values, Vector2Int index)
    {
        if (index.x >= 0 &&
            index.y >= 0 &&
            index.x < values.GetLength(0) &&
            index.y < values.GetLength(1)
            )
        {
            return true;
        }
        return false;
    }

    public static bool IndexInBounds<T>(this T[] list, int index)
    {
        if (index >= 0 && index < list.Length)
        {
            return true;
        }
        return false;
    }

    public static bool IndexInBounds<T>(this T[,] list, int x, int y)
    {
        if (x >= 0 && x < list.GetLength(0) && y >= 0 && y < list.GetLength(1))
        {
            return true;
        }
        return false;
    }
}
