using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.Core.GameData
{
    [CreateAssetMenu(menuName = "Snowdrama/Data Objects/String to String Dictionary")]
    public class StringStringDictionaryObject : ScriptableObject
    {
        public StringStringDictionary data = new StringStringDictionary();
    }
}