using System;
using UnityEngine;

namespace Snowdrama.Transition
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "SceneTransition", menuName = "Snowdrama/Transitions/Scene Transition")]
    public class SceneTransition : ScriptableObject
    {
        public string sceneName;

        public void TransitionToThis()
        {
            Debug.Log("Going To Scene");
            SceneController.StartTransition(sceneName, 1.0f, null);
        }
    }
}