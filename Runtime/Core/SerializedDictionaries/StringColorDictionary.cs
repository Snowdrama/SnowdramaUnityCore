using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Snowdrama.GameData
{
    [System.Serializable]
    public class StringColorDictionary : UnitySerializedDictionary<string, Color>
    {
        public void Write(string key, Color value)
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

        public Color GetOrDefault(string key, Color defaultValue = default)
        {
            if (Has(key))
            {
                return this[key];
            }
            return defaultValue;
        }
    }
}