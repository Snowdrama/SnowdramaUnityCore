using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Snowdrama.GameData
{
    [System.Serializable]
    public class StringVector3Dictionary : UnitySerializedDictionary<string, Vector3>
    {
        public void Write(string key, Vector3 value)
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

        public Vector3 GetOrDefault(string key, Vector3 defaultValue = default)
        {
            if (Has(key))
            {
                return this[key];
            }
            return defaultValue;
        }
    }
}