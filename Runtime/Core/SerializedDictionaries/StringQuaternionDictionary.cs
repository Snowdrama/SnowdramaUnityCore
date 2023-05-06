using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Snowdrama.GameData
{
    [System.Serializable]
    public class StringQuaternionDictionary : UnitySerializedDictionary<string, Quaternion>
    {
        public void Write(string key, Quaternion value)
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

        public Quaternion GetOrDefault(string key, Quaternion defaultValue = default)
        {
            if (Has(key))
            {
                return this[key];
            }
            return defaultValue;
        }
    }
}