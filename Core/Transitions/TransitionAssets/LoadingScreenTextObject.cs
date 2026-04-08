using System;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.Transition
{
    [CreateAssetMenu]
    public class LoadingScreenTextObject : ScriptableObject
    {
        [SerializeField] private List<string> loadingScreenText = new List<string>();
        [SerializeField] private int index = 0;
        [SerializeField] private bool chooseRandom = true;

        public string GetLoadingScreenText()
        {
            if (chooseRandom)
            {
                return loadingScreenText.GetRandom();
            }

            index++;
            index %= loadingScreenText.Count;
            return loadingScreenText[index];
        }
    }
}

