using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.Transition
{
    public class SceneControllerDebug : MonoBehaviour
    {
        [SerializeField]
        private List<string> openScenes;
        [SerializeField]
        private List<string> scenesNotToUnload;
        [SerializeField]
        private int loadSceneInstructionCount = 0;
        void Update()
        {
            openScenes = SceneController.loadedScenes;
            scenesNotToUnload = SceneController.sceneNotToUnload;
            loadSceneInstructionCount = SceneController.loadSceneInstructionCount;
        }
    }
}