using UnityEngine;

namespace Snowdrama.Core.GameData
{
    [CreateAssetMenu(menuName = "Snowdrama/Data Objects/String Quaternion Dictionary")]
    public class StringQuaternionDictionaryObject : ScriptableObject
    {
        public StringQuaternionDictionary data = new StringQuaternionDictionary();
    }
}