using System;
using System.Collections.Generic;
using System.Linq;

public static class EnumerableExtensions
{
    private static Random random = new Random();

    /// <summary>
    /// Gets the last `count` elements from the enumeration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int count)
    {
        if (source == null) throw new ArgumentNullException("source");
        if (count < 0) throw new ArgumentOutOfRangeException("count");

        if (count == 0) yield break;

        //create a queue of some number
        var queue = new Queue<T>(count);

        foreach (var t in source)
        {
            if (queue.Count == count) queue.Dequeue();

            queue.Enqueue(t);
        }

        foreach (var t in queue)
            yield return t;
    }

    public static T GetRandom<T>(this IEnumerable<T> source)
    {
        if (source == null) throw new ArgumentNullException("source");
        int count = source.Count();
        if (count == 0) throw new Exception("GetRandom can't be called since list has no values");

        return source.ElementAt(UnityEngine.Random.Range(0, count));
    }

    public static T GetRandom<T>(this ICollection<T> source)
    {
        if (source == null) throw new ArgumentNullException("source");
        if (source.Count == 0) throw new Exception("GetRandom can't be called since list has no values");

        return source.ElementAt(UnityEngine.Random.Range(0, source.Count));
    }


    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> sourceList)
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
            int randIndex = random.Next(0, size);
            newList[i] = newList[randIndex];
            newList[randIndex] = temp;
        }

        return newList;
    }
}