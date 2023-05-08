using UnityEngine;

namespace Snowdrama.Core.GameData
{
    [CreateAssetMenu(menuName = "Snowdrama/Data Objects/String Float Dictionary")]
    public class StringFloatDictionaryObject : ScriptableObject
    {
        public StringFloatDictionary data = new StringFloatDictionary();
    }
}