using UnityEngine;

namespace Snowdrama.GameData
{
    [CreateAssetMenu(menuName = "Snowdrama/Data Objects/String Vector2Int Dictionary")]
    public class StringVector2IntDictionaryObject : ScriptableObject
    {
        public StringVector2IntDictionary data = new StringVector2IntDictionary();
    }
}