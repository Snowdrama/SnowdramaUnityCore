using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A wrapper structure that behaves like Queue<T> but is actually a List<T> internally
/// 
/// Useful for situations where you want a list but want to access it like a Queue with push/pop/peek
/// 
/// Includes some Queue-Like functions like Enqueue/Dequeue
/// </summary>
/// <typeparam name="T">The type the 'stack' holds</typeparam>
public class QueueList<T> : IEnumerable<T>
{
    private List<T> items = new List<T>();
    public int Count => items.Count;

    #region Enqueue
    public void Enqueue(T item)
    {
        items.Insert(0, item);
    }
    #endregion

    #region Dequeue
    public T Deqeue()
    {
        var lastItem = items[items.Count - 1];
        items.RemoveAt(items.Count - 1);
        return lastItem;
    }
    #endregion

    #region List Functions
    public void Remove(T item)
    {
        items.Remove(item);
    }
    public void RemoveAt(int itemAtPosition)
    {
        items.RemoveAt(itemAtPosition);
    }
    public void Clear()
    {
        items.Clear();
    }
    public bool Contains(T item)
    {
        return items.Contains(item);
    }
    #endregion

    #region Enumerator/Enumerable
    public IEnumerator<T> GetEnumerator()
    {
        return items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return items.GetEnumerator();
    }
    #endregion
}