using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A wrapper structure that behaves like Stack<T> but is actually a List<T> internally
/// 
/// Useful for situations where you want a list but want to access it like a Stack with push/pop/peek
/// 
/// Includes some Stack-Like functions like PushBottom/PeekBottom as well as Peek(count) and Pop(count)
/// </summary>
/// <typeparam name="T">The type the 'stack' holds</typeparam>
public class StackList<T> : IEnumerable<T>
{
    private List<T> items = new List<T>();
    public int Count => items.Count;

    #region Push
    /// <summary>
    /// Pushes an item to the top of the StackList
    /// </summary>
    /// <param name="item">An item to add to the top of the StackList</param>
    public void Push(T item)
    {
        items.Add(item);
    }
    /// <summary>
    /// Pushes an item to the top of the StackList
    /// </summary>
    /// <param name="items">Array of items to add to the top of the StackList</param>
    public void Push(T[] items)
    {
        for (var i = 0; i < items.Length; i++)
        {
            this.items.Add(items[i]);
        }
    }
    /// <summary>
    /// Pushes an item to the top of the StackList
    /// </summary>
    /// <param name="items">List of items to add to the top of the StackList</param>
    public void Push(List<T> items)
    {
        for (var i = 0; i < items.Count; i++)
        {
            this.items.Add(items[i]);
        }
    }
    /// <summary>
    /// Pushes to the botom of the StackList
    /// </summary>
    /// <param name="item">The item to push to the botom of the StackList</param>
    public void PushBottom(T item)
    {
        items.Insert(0, item);
    }
    /// <summary>
    /// Pushes an item to the top of the StackList
    /// </summary>
    /// <param name="items">Array of items to add to the top of the StackList</param>
    public void PushBottom(T[] items)
    {
        for (var i = 0; i < items.Length; i++)
        {
            this.items.Insert(0, items[i]);
        }
    }
    /// <summary>
    /// Pushes an item to the top of the StackList
    /// </summary>
    /// <param name="items">List of items to add to the top of the StackList</param>
    public void PushBottom(List<T> items)
    {
        for (var i = 0; i < items.Count; i++)
        {
            this.items.Insert(0, items[i]);
        }
    }
    #endregion

    #region Pop
    /// <summary>
    /// Pops an element off the top of the StackList
    /// 
    /// If there's no elements returns default(T)
    /// </summary>
    /// <returns>The top element or default(T)</returns>
    public T Pop()
    {
        if (items.Count > 0)
        {
            var temp = items[items.Count - 1];
            items.RemoveAt(items.Count - 1);
            return temp;
        }
        else //throw error???
            return default(T);
    }
    /// <summary>
    /// Pops an element off the top of the StackList
    /// 
    /// If there's no elements returns default(T)
    /// </summary>
    /// <returns>The top element or default(T)</returns>
    public T PopBottom()
    {
        if (items.Count > 0)
        {
            var temp = items[0];
            items.RemoveAt(0);
            return temp;
        }
        else //throw error???
            return default(T);
    }
    #endregion

    #region Peek
    /// <summary>
    /// Peeks at the top element of the StackList without removing it from the StackList
    /// 
    /// If there's no elements returns default(T)
    /// </summary>
    /// <returns>The top lement of the StackList</returns>
    public T Peek()
    {
        if (items.Count > 0)
            return items[items.Count - 1];
        else
            return default(T);
    }
    /// <summary>
    /// Peek at the top 'count' values
    /// </summary>
    /// <param name="count">the number of elements to get</param>
    /// <returns>A list of elements from the top of the StackList</returns>
    public T[] Peek(int count)
    {
        return items.TakeLast(count).ToArray();
    }

    /// <summary>
    /// Peeks at bottom of StackList
    /// </summary>
    /// <returns>The bottom of he StackList</returns>
    public T PeekBottom()
    {
        return items[0];
    }
    /// <summary>
    /// Peeks at bottom 'count' elements of StackList
    /// </summary>
    /// <param name="count">the number of elements to get</param>
    /// <returns>A list of elements at the botom of the StackList</returns>
    public T[] PeekBottom(int count)
    {
        return items.TakeFirst(count).ToArray();
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