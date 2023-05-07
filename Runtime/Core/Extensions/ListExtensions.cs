using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    /// <summary>
    /// Shuffles a list into a new list, this does not modify the original list
    /// and instead returns a new copy of the list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sourceList"></param>
    /// <returns></returns>
    public static List<T> ShuffleList<T>(this List<T> sourceList)
    {
        List<T> newList = new List<T>(sourceList);
        int size = newList.Count;
        for (int i = 0; i < size; i++)
        {
            T temp = newList[i];
            int randIndex = Random.Range(0, size);
            newList[i] = newList[randIndex];
            newList[randIndex] = temp;
        }

        return newList;
    }
    /// <summary>
    /// Similar to ShuffleList but shuffles the source list itself.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sourceList"></param>
    /// <returns></returns>
    public static void ShuffleSelf<T>(this List<T> sourceList)
    {
        int size = sourceList.Count;
        for (int i = 0; i < size; i++)
        {
            T temp = sourceList[i];
            int randIndex = Random.Range(0, size);
            sourceList[i] = sourceList[randIndex];
            sourceList[randIndex] = temp;
        }
    }
}
