using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Snowdrama.GameData
{
    [System.Serializable]
    public class StringFloatDictionary : UnitySerializedDictionary<string, float>
    {
        public void Write(string key, float value)
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

        public float GetOrDefault(string key, float defaultValue = 0)
        {
            if (Has(key))
            {
                return this[key];
            }
            return defaultValue;
        }
    }
}