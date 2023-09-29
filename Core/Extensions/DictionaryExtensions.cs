using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class DictionaryExtensions
{
    public static K GetRandomKey<K, V>(this Dictionary<K, V> dictionary)
    {
        return dictionary.Keys.GetRandom();
    }
    public static V GetRandomValue<K, V>(this Dictionary<K, V> dictionary)
    {
        return dictionary.Values.GetRandom();
    }
}
