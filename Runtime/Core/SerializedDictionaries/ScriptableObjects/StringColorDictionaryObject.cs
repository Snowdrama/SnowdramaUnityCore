using UnityEngine;

namespace Snowdrama.Core.GameData
{
    [CreateAssetMenu(menuName = "Snowdrama/Data Objects/String Color Dictionary")]
    public class StringColorDictionaryObject : ScriptableObject
    {
        public StringColorDictionary data = new StringColorDictionary();
    }
}