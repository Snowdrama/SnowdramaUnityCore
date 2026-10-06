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
    public StackList<T> pile = new StackList<T>();
    public StackList<T> used = new StackList<T>();
    public StackList<T> discard = new StackList<T>();
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
        //Throw error?
        return default(T);
    }

    public void Discard(T card)
    {
        //remove it from the used pile
        //since we're moving it to 
        //the discard
        if (used.Contains(card))
        {
            used.Remove(card);
        }
        //push it to the discard
        discard.Push(card);
    }

    /// <summary>
    /// takes everything puts it back in the pile
    /// and reshuffled
    /// </summary>
    public void ReshuffleAll()
    {
        this.ShuffleDiscardIntoDeck();
        this.ShuffleUsedIntoDeck();
        pile.Shuffle();
    }

    /// <summary>
    /// Shuffles only the discard pile back into the deck
    /// </summary>
    public void ShuffleDiscardIntoDeck()
    {
        foreach (var item in discard)
        {
            pile.Push(item);
        }
        discard.Clear();
        pile.Shuffle();
    }

    /// <summary>
    /// Shuffles only the used pile back into the deck
    /// </summary>
    public void ShuffleUsedIntoDeck()
    {
        foreach (var item in used)
        {
            pile.Push(item);
        }
        used.Clear();
        pile.Shuffle();
    }


}
