using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Snowdrama.Core.GameData
{
    public abstract class UnitySerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<KeyValueData> keyValueData = new List<KeyValueData>();

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Clear();
            foreach (var item in keyValueData)
            {
                this[item.key] = item.value;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            keyValueData.Clear();

            foreach (var item in this)
            {
                keyValueData.Add(new KeyValueData() { key = item.Key, value = item.Value });
            }
        }

        [System.Serializable]
        private struct KeyValueData
        {
            public TKey key;
            public TValue value;
        }
    }
}