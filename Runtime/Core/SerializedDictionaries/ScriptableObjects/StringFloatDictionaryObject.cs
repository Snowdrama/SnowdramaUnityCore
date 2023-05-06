using UnityEngine;

namespace Snowdrama.GameData
{
    [CreateAssetMenu(menuName = "Snowdrama/Data Objects/String Float Dictionary")]
    public class StringFloatDictionaryObject : ScriptableObject
    {
        public StringFloatDictionary data = new StringFloatDictionary();
    }
}