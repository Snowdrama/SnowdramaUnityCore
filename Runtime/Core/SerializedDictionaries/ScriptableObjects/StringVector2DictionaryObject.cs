using UnityEngine;

namespace Snowdrama.Core.GameData
{
    [CreateAssetMenu(menuName = "Snowdrama/Data Objects/String Vector2 Dictionary")]
    public class StringVector2DictionaryObject : ScriptableObject
    {
        public StringVector2Dictionary data = new StringVector2Dictionary();
    }
}