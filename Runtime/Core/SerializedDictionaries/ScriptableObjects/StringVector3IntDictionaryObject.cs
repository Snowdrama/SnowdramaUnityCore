using UnityEngine;

namespace Snowdrama.GameData
{
    [CreateAssetMenu(menuName = "Snowdrama/Data Objects/String Vector3Int Dictionary")]
    public class StringVector3IntDictionaryObject : ScriptableObject
    {
        public StringVector3IntDictionary data = new StringVector3IntDictionary();
    }
}