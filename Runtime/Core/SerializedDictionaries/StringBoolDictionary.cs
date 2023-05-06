using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Snowdrama.GameData
{
    [System.Serializable]
    public class StringBoolDictionary : UnitySerializedDictionary<string, bool>
    {
        public void Write(string key, bool value)
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

        public bool GetOrDefault(string key, bool defaultValue = false)
        {
            if (Has(key))
            {
                return this[key];
            }
            return defaultValue;
        }
    }

}