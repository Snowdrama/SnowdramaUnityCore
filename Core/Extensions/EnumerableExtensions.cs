using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class EnumerableExtensions
{
    private static Random random = new Random();

    /// <summary>
    /// Gets the last `count` elements from the enumeration
    /// 
    /// Example: [1,2,3,4,5,6,7,8,9,10].TakeLast(5);
    /// 
    /// Output: [6,7,8,9,10] - last 5 elements
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

        //source is [0:10]
        //first loop queue.Count = 0 and count = 5; Enqueue(0)
        //first loop queue.Count = 1 and count = 5; Enqueue(1)
        //first loop queue.Count = 2 and count = 5; Enqueue(2)
        //first loop queue.Count = 3 and count = 5; Enqueue(3)
        //first loop queue.Count = 4 and count = 5; Enqueue(4)
        //first loop queue.Count = 5 and count = 5; Dequeue(0) Enqueue(5)
        //first loop queue.Count = 6 and count = 5; Dequeue(1) Enqueue(6) 
        //first loop queue.Count = 7 and count = 5; Dequeue(2) Enqueue(7)
        //first loop queue.Count = 8 and count = 5; Dequeue(3) Enqueue(8)
        //first loop queue.Count = 9 and count = 5; Dequeue(4) Enqueue(9)
        foreach (var t in source)
        {
            //since this is a queue, we dequeue
            //until we get the last 'count' elements
            if (queue.Count == count) { queue.Dequeue(); }

            queue.Enqueue(t);
        }

        foreach (var t in queue)
            yield return t;
    }

    /// <summary>
    /// Gets the first `count` elements from the enumeration
    /// 
    /// Example: [1,2,3,4,5,6,7,8,9,10].TakeFirst(5);
    /// 
    /// Output: [1,2,3,4,5] - first 5 elements
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IEnumerable<T> TakeFirst<T>(this IEnumerable<T> source, int count)
    {
        if (source == null) throw new ArgumentNullException("source");
        if (count < 0) throw new ArgumentOutOfRangeException("count");

        if (count == 0) yield break;

        //create a queue of some number
        var queue = new Queue<T>(count);
        foreach (var t in source)
        {
            //if we've already completed 'count' loops we're done
            if (queue.Count == count) { break; }
            yield return t;
        }
    }

    /// <summary>
    /// Gets a random assortment of values from the source, CAN have duplicates.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public static IEnumerable<T> GetRandom<T>(this IEnumerable<T> source, int count)
    {
        if (source == null) throw new ArgumentNullException("source");
        var size = source.Count();
        if (size == 0) throw new Exception("GetRandom can't be called since list has no values");
        if (count == 0) yield break;

        for (var i = 0; i < count; i++)
        {
            yield return source.ElementAt(RandomAndNoise.RandomRange(0, size)); ;
        }
    }

    /// <summary>
    /// Gets a random assortment of values from the source, Can't have duplicates.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public static IEnumerable<T> GetRandomNoDuplicates<T>(this IEnumerable<T> source, int count)
    {
        if (source == null) throw new ArgumentNullException("source");
        var size = source.Count();
        if (size == 0) throw new Exception("GetRandom can't be called since list has no values");
        if (count == 0) yield break;

        //copy the source into a new list
        var list = new List<T>(source);
        //shuffle the list
        list.Shuffle();
        //then get count from that duplicated list
        for (var i = 0; i < count; i++)
        {
            yield return list[i];
        }
    }

    public static T GetRandom<T>(this IEnumerable<T> source)
    {
        if (source == null) throw new ArgumentNullException("source");
        var count = source.Count();
        if (count == 0) throw new Exception("GetRandom can't be called since list has no values");

        return source.ElementAt(RandomAndNoise.RandomRange(0, count));
    }

    public static T GetRandom<T>(this ICollection<T> source)
    {
        if (source == null) throw new ArgumentNullException("source");
        if (source.Count == 0) throw new Exception("GetRandom can't be called since list has no values");

        return source.ElementAt(RandomAndNoise.RandomRange(0, source.Count));
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> sourceList)
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
            var randIndex = random.Next(0, size);
            newList[i] = newList[randIndex];
            newList[randIndex] = temp;
        }

        return newList;
    }

    #region "Tests"
    private static bool Test_GetRandomNoDuplicates()
    {
        //since the list only has 5 elements, it should only get 5 elements
        //if any are duplicated, then this is broken
        var randomTest = new List<int>() { 1, 2, 3, 4, 5 }.GetRandomNoDuplicates(5);
        var test = new List<int>();
        foreach (var item in randomTest)
        {
            if (test.Contains(item))
            {
                //failed there's a duplicate
                return false;
            }
            test.Add(item);
        }
        return true;
    }

    #endregion
}
