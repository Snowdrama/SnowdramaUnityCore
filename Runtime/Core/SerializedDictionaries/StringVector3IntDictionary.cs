using UnityEngine;

namespace Snowdrama.GameData
{
    [System.Serializable]
    public class StringVector3IntDictionary : UnitySerializedDictionary<string, Vector3Int>
    {
        public void Write(string key, Vector3Int value)
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

        public Vector3Int GetOrDefault(string key, Vector3Int defaultValue = default)
        {
            if (Has(key))
            {
                return this[key];
            }
            return defaultValue;
        }
    }
}