using System;
public class ObjectPooler<T> where T : IEquatable<T>
{
    private T[] things;
    private T[] used;

    public ObjectPooler(int maxPoolSize)
    {
        things = new T[maxPoolSize];
        used = new T[maxPoolSize];
    }

    /// <summary>
    /// Note this is not to increase the number of items in the pool
    /// this is to add things to the pool initially
    /// </summary>
    /// <param name="thingToAdd"></param>
    public void AddToPool(T thingToAdd)
    {
        for (int i = 0; i < things.Length; i++)
        {
            if (things[i] == null)
            {
                things[i] = thingToAdd;
            }
        }
    }

    public T GetOne()
    {
        for (int i = 0; i < things.Length; i++)
        {
            if (things[i] != null)
            {
                //set it to used
                used[i] = things[i];
                //set it to null
                things[i] = default(T);

                return used[i];
            }
        }
        return default(T);
    }


    public void Return(T thingToReturn)
    {
        for (int i = 0; i < used.Length; i++)
        {
            if (used[i] != null && used[i].Equals(thingToReturn))
            {
                //set it to used
                used[i] = things[i];
                //set it to null
                things[i] = default(T);
            }
        }
    }

}
