namespace Snowdrama.GameData
{
    [System.Serializable]
    public class StringStringDictionary : UnitySerializedDictionary<string, string>
    {
        public void Write(string key, string value)
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

        public string GetOrDefault(string key, string defaultValue = null)
        {
            if (Has(key))
            {
                return this[key];
            }
            return defaultValue;
        }
    }

}