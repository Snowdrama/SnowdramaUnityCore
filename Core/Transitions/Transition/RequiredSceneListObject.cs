using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.Transition
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "RequiredSceneList", menuName = "Snowdrama/Transitions/Required Scene List")]
    public class RequiredSceneListObject : ScriptableObject
    {
        public List<RequiredScene> listOfRequiredSceneNames;
    }

    [System.Serializable]
    public class RequiredScene
    {
        public bool dontDestroyOnLoad;
        public string sceneName;
    }

}