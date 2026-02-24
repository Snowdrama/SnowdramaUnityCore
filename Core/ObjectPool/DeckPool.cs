using System.Collections.Generic;

/// <summary>
/// A deck pool is a shuffled limited pool of things.
/// 
/// Instead of being used as a bunch of clones, it's a bunch
/// of unique objects and you're expected to pull objects
/// from it to add randomness
/// 
/// It's called this because it's intended to be used for cases
/// where you want some set of things that you draw from
/// Like a deck of cards!
/// </summary>
public class DeckPool<T>
{
    public Stack<T> pile = new Stack<T>();
    public Stack<T> used = new Stack<T>();
    public DeckPool(List<T> items)
    {
        foreach (var item in items)
        {
            pile.Push(item);
        }

        //randomize the list
        pile.Shuffle();
    }

    public T Draw()
    {
        if (pile.Count > 0)
        {
            var thing = pile.Pop();
            used.Push(thing);
            return thing;
        }
        return default(T);
    }

    /// <summary>
    /// takes everything puts it back in the pile
    /// and reshuffled
    /// </summary>
    public void Reshuffle()
    {
        foreach (var item in used)
        {
            pile.Push(item);
        }
        used.Clear();
        pile.Shuffle();
    }


}
