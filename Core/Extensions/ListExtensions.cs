using System;
using System.Collections.Generic;
using System.Linq;
public static class ListExtensions
{
    /// <summary>
    /// Shuffles a sourceList into a new sourceList, this does not modify the original sourceList
    /// and instead returns a new copy of the sourceList.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sourceList"></param>
    /// <returns></returns>
    public static IList<T> ShuffleList<T>(this IList<T> sourceList)
    {
        if (sourceList == null)
        {
            throw new ArgumentNullException(nameof(sourceList));
        }
        var newList = new List<T>(sourceList);
        var size = newList.Count;
        for (var i = 0; i < size; i++)
        {
            var temp = newList[i];
            var randIndex = RandomAndNoise.RandomRange(0, size);
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
    public static void ShuffleSelfMutate<T>(this IList<T> sourceList)
    {
        if (sourceList == null)
        {
            throw new ArgumentNullException(nameof(sourceList));
        }

        var size = sourceList.Count;
        for (var i = 0; i < size; i++)
        {
            var temp = sourceList[i];
            var randIndex = RandomAndNoise.RandomRange(0, size);
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

        var firstValue = sourceList[firstIndex];
        sourceList[firstIndex] = sourceList[secondIndex];
        sourceList[secondIndex] = firstValue;
    }

    public static void RemoveNullEntries<T>(this IList<T> list) where T : class
    {
        list = list.Where(x => x != null).ToList();
    }

    /// <summary>
    /// Boilerplate wrapper for adding only if the item
    /// isn't already in the list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="item"></param>
    public static void AddIfDoesntExist<T>(this IList<T> list, T item) where T : class
    {
        //prevent adding null items
        if (item != null && !list.Contains(item))
        {
            list.Add(item);
        }
    }

    /// <summary>
    /// Boilerplate wrapper for removing only if the item
    /// is already in the list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="item"></param>
    public static void RemoveIfExists<T>(this IList<T> list, T item) where T : class
    {
        if (list.Contains(item))
        {
            list.Remove(item);
        }
    }

    /// <summary>
    /// Gets a reandom item from the list
    /// 
    /// Throws an exception if the list is empty
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public static T GetRandom<T>(this IList<T> source)
    {
        if (source == null) throw new ArgumentNullException("source");
        if (source.Count == 0) throw new Exception("GetRandom can't be called since list has no values");

        return source.ElementAt(RandomAndNoise.RandomRange(0, source.Count));
    }

    /// <summary>
    /// Gets a number of items from the randomized list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static IList<T> GetRandomCount<T>(this IList<T> source, int count)
    {
        var randomizedList = source.ShuffleList();
        return randomizedList.Take(count).ToList();
    }
    public static void MoveUp<T>(this List<T> list, T item, int steps = 1)
    {
        var index = list.IndexOf(item);

        // Not found or already at top
        if (index <= 0 || steps <= 0)
            return;

        // Calculate how far we can actually move
        var newIndex = index - steps;

        // Clamp to top (index 0)
        if (newIndex < 0)
            newIndex = 0;

        // Remove and reinsert
        list.RemoveAt(index);
        list.Insert(newIndex, item);
    }

    public static void MoveDown<T>(this List<T> list, T item, int steps = 1)
    {
        var index = list.IndexOf(item);

        // Not found or already at bottom
        if (index < 0 || index >= list.Count - 1 || steps <= 0)
            return;

        // Calculate how far we can actually move
        var newIndex = index + steps;

        // Clamp to bottom
        if (newIndex >= list.Count)
            newIndex = list.Count - 1;

        // Remove and reinsert
        list.RemoveAt(index);
        list.Insert(newIndex, item);
    }
}
