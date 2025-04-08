using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.Transition
{
    [CreateAssetMenu]
    public class LoadingScreenTextObject : ScriptableObject
    {
        [SerializeField] List<string> loadingScreenText = new List<string>();
        [SerializeField] int index = 0;

        public string GetLoadingScreenText()
        {
            index++;
            index %= loadingScreenText.Count;
            return loadingScreenText[index];
        }
        public string GetRandomString()
        {
            if(loadingScreenText.Count > 0)
            {
                return loadingScreenText[Random.Range(0, loadingScreenText.Count)];
            }
            return "No Loading Screen Text Today! Sorry!";
        }
    }
}

