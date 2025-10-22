using UnityEngine;

namespace Snowdrama.Transition
{
    [CreateAssetMenu(fileName = "SceneControllerOptions", menuName = "Snowdrama/Transitions/Scene Controller Options")]
    public class SceneControllerOptions : ScriptableObject
    {
        public bool showConsoleMessages = false;
        public bool hideRequiredSceneWarning = false;
    }
}