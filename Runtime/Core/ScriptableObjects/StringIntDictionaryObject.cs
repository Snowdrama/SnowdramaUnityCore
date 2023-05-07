using UnityEngine;

namespace Snowdrama.GameData
{
    [CreateAssetMenu(menuName = "Snowdrama/Data Objects/String Int Dictionary")]
    public class StringIntDictionaryObject : ScriptableObject
    {
        public StringIntDictionary data = new StringIntDictionary();
    }
}