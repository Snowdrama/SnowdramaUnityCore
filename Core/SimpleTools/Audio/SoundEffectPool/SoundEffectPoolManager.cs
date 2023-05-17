using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SoundEffectPoolManager : ScriptableObject
{
    public static Dictionary<string, SoundEffectPool> sourcePoolList = new Dictionary<string, SoundEffectPool>();

    public static void AddPool(string poolName, SoundEffectPool pool)
    {
        if(sourcePoolList == null)
        {
            sourcePoolList = new Dictionary<string, SoundEffectPool>();
        }
        if (!sourcePoolList.ContainsKey(poolName))
        {
            sourcePoolList.Add(poolName.Trim().ToLower(), pool);
        }
        else
        {
            Debug.LogWarning($"Pool {poolName} already exists in pool list(double add?)");
        }
    }
    public static void RemovePool(string poolName)
    {
        if (sourcePoolList == null)
        {
            sourcePoolList = new Dictionary<string, SoundEffectPool>();
        }
        if (sourcePoolList.ContainsKey(poolName))
        {
            sourcePoolList.Remove(poolName.Trim().ToLower());
        }
        else
        {
            Debug.LogWarning($"Pool {poolName} not found in pool list");
        }
    }

    public static void PlayClip(string poolName, AudioClip clip, Vector2? pitchMinMax = null)
    {
        if (sourcePoolList.ContainsKey(poolName.Trim().ToLower()))
        {
            sourcePoolList[poolName.Trim().ToLower()].PlaySource(clip, pitchMinMax);
        }
        else
        {
            Debug.LogError($"Pool named: {poolName.Trim().ToLower()} not found");
        }
    }
}
