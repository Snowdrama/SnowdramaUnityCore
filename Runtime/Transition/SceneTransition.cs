using UnityEngine;

namespace Snowdrama.Transition
{
    [CreateAssetMenu]
    public class SceneTransition : ScriptableObject
    {
        public string sceneName;

        public void TransitionToThis()
        {
            Debug.Log("Going To Scene");
            TransitionController.GoToScene(sceneName);
        }
    }
}