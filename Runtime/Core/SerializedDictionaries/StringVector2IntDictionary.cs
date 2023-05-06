using UnityEngine;

namespace Snowdrama.GameData
{
    [System.Serializable]
    public class StringVector2IntDictionary : UnitySerializedDictionary<string, Vector2Int>
    {
        public void Write(string key, Vector2Int value)
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

        public Vector2Int GetOrDefault(string key, Vector2Int defaultValue = default)
        {
            if (Has(key))
            {
                return this[key];
            }
            return defaultValue;
        }
    }
}