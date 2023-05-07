using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.GameData
{
    [CreateAssetMenu(menuName = "Snowdrama/Data Objects/String Bool Dictionary")]
    public class StringBoolDictionaryObject : ScriptableObject
    {
        public StringBoolDictionary stringBoolDictionary = new StringBoolDictionary();
    }
}