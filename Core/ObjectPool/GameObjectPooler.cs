using UnityEngine;

/// <summary>
/// A Variant of <class>ObjectPooler</class> that
/// allows objects to auto return when deactivated
/// </summary>
public class GameObjectPooler
{
    private GameObject[] things;
    private GameObject[] used;

    public GameObjectPooler(int maxPoolSize)
    {
        things = new GameObject[maxPoolSize];
        used = new GameObject[maxPoolSize];
    }

    /// <summary>
    /// Note this is not to increase the number of items in the pool
    /// this is to add things to the pool initially
    /// </summary>
    /// <param name="thingToAdd"></param>
    public void AddToPool(GameObject thingToAdd)
    {
        for (int i = 0; i < things.Length; i++)
        {
            if (things[i] == null)
            {
                things[i] = thingToAdd;
            }
        }
    }


    /// <summary>
    /// gets one object from the unused pile and
    /// puts it in the used pile, then returns a
    /// reference to that
    /// </summary>
    /// <returns></returns>
    public GameObject GetOne()
    {
        for (int i = 0; i < things.Length; i++)
        {
            if (things[i] != null)
            {
                //set it to used
                used[i] = things[i];
                //set it to null
                things[i] = null;

                return used[i];
            }
        }
        return null;
    }

    /// <summary>
    /// If you need to return something manally you can use this
    /// if it's not in the used queue then it won't be returned or added
    /// </summary>
    /// <param name="thingToReturn"></param>
    public void Return(GameObject thingToReturn)
    {
        for (int i = 0; i < used.Length; i++)
        {
            if (used[i] != null && used[i].Equals(thingToReturn))
            {
                //return it to the usable pile
                things[i] = used[i];
                //set it to null
                used[i] = null;
            }
        }
    }

    /// <summary>
    /// This should be called in update, this will auto
    /// return any game object that is no longer active
    /// </summary>
    public void Update()
    {
        for (int i = 0; i < used.Length; i++)
        {
            if (used[i] != null && used[i].activeInHierarchy)
            {
                //return it to the usable pile
                things[i] = used[i];
                //set it to null
                used[i] = null;
            }
        }
    }
}