using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtensions
{
    public static T GetRandom<T>(this T[] source)
    {
        if (source == null) throw new ArgumentNullException("source");
        if (source.Length == 0) throw new Exception("GetRandom can't be called since list has no values");

        return source[UnityEngine.Random.Range(0, source.Length)];
    }

    public static T[] Shuffle<T>(this T[] sourceList)
    {
        if (sourceList == null)
        {
            throw new ArgumentNullException(nameof(sourceList));
        }
        T[] newList = new T[sourceList.Length];
        int size = newList.Length;
        for (int i = 0; i < size; i++)
        {
            T temp = newList[i];
            int randIndex = UnityEngine.Random.Range(0, size);
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

        T firstValue = sourceList[firstIndex];
        sourceList[firstIndex] = sourceList[secondIndex];
        sourceList[secondIndex] = firstValue;
    }

    public static T[] RemoveNullEntries<T>(this T[] list)
    {
        List<T> newList = new List<T>(list);

        for (int i = newList.Count - 1; i >= 0; i--)
        {
            if (Equals(newList[i], null))
            {
                newList.RemoveAt(i);
            }
        }

        return newList.ToArray();
    }
}
