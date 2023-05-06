using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Snowdrama.GameData
{
    [System.Serializable]
    public class StringVector2Dictionary : UnitySerializedDictionary<string, Vector2>
    {
        public void Write(string key, Vector2 value)
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

        public Vector2 GetOrDefault(string key, Vector2 defaultValue = default)
        {
            if (Has(key))
            {
                return this[key];
            }
            return defaultValue;
        }
    }
}