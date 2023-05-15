using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    /// <summary>
    /// Shuffles a sourceList into a new sourceList, this does not modify the original sourceList
    /// and instead returns a new copy of the sourceList.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sourceList"></param>
    /// <returns></returns>
    public static List<T> ShuffleList<T>(this List<T> sourceList)
    {
        if (sourceList == null)
        {
            throw new ArgumentNullException(nameof(sourceList));
        }
        List<T> newList = new List<T>(sourceList);
        int size = newList.Count;
        for (int i = 0; i < size; i++)
        {
            T temp = newList[i];
            int randIndex = UnityEngine.Random.Range(0, size);
            newList[i] = newList[randIndex];
            newList[randIndex] = temp;
        }

        return newList;
    }
    /// <summary>
    /// Similar to ShuffleList but shuffles the source sourceList itself.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sourceList"></param>
    /// <returns></returns>
    public static void ShuffleSelfMutate<T>(this List<T> sourceList)
    {
        if (sourceList == null)
        {
            throw new ArgumentNullException(nameof(sourceList));
        }

        int size = sourceList.Count;
        for (int i = 0; i < size; i++)
        {
            T temp = sourceList[i];
            int randIndex = UnityEngine.Random.Range(0, size);
            sourceList[i] = sourceList[randIndex];
            sourceList[randIndex] = temp;
        }
    }

    public static void Swap<T>(this IList<T> sourceList, int firstIndex, int secondIndex)
    {
        if (sourceList == null)
        {
            throw new ArgumentNullException(nameof(sourceList));
        }

        if (sourceList.Count < 2)
        {
            throw new ArgumentException("List count should be at least 2 for a swap.");
        }

        if (firstIndex < 0 || firstIndex >= sourceList.Count || secondIndex < 0 || secondIndex >= sourceList.Count)
        {
            throw new Exception($"Indexes {firstIndex} and {secondIndex} need to be within the range of " +
                $"the sourceList which has {sourceList.Count} elements.");
        }

        T firstValue = sourceList[firstIndex];
        sourceList[firstIndex] = sourceList[secondIndex];
        sourceList[secondIndex] = firstValue;
    }

    public static void RemoveNullEntries<T>(this IList<T> list) where T : class
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (Equals(list[i], null))
            {
                list.RemoveAt(i);
            }
        }
    }
}
