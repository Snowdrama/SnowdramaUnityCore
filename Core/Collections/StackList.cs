using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// something that behaves like a stack
/// but is actually a list internally
/// </summary>
/// <typeparam name="T">The type the 'stack' holds</typeparam>
public class StackList<T>
{
    private List<T> items = new List<T>();

    public void Push(T item)
    {
        items.Add(item);
    }
    public T Pop()
    {
        if (items.Count > 0)
        {
            var temp = items[items.Count - 1];
            items.RemoveAt(items.Count - 1);
            return temp;
        }
        else
            return default(T);
    }
    public T Peek()
    {
        return items[items.Count - 1];
    }
    public void Remove(int itemAtPosition)
    {
        items.RemoveAt(itemAtPosition);
    }
}