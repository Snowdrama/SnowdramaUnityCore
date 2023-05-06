using UnityEngine;

namespace Snowdrama.GameData
{
    [CreateAssetMenu(menuName = "Snowdrama/Data Objects/String Vector3 Dictionary")]
    public class StringVector3DictionaryObject : ScriptableObject
    {
        public StringVector3Dictionary data = new StringVector3Dictionary();
    }
}