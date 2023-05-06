using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Snowdrama.GameData
{
    [System.Serializable]
    public class StringIntDictionary : UnitySerializedDictionary<string, int>
    {
        public void Write(string key, int value)
        {
            if (Has(key))
            {
                this[key] = value;
            }
            else
            {
                Add(key, value);
            }
        }

        public bool Has(string key)
        {
            if (ContainsKey(key))
            {
                return true;
            }
            return false;
        }

        public int GetOrDefault(string key, int defaultValue = 0)
        {
            if (Has(key))
            {
                return this[key];
            }
            return defaultValue;
        }
    }
}