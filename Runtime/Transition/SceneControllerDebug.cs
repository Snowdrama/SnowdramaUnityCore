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

        public string targetScene;
        public float transitionValue;
        public float transitionDuration;
        public float transitionSpeed;
        public bool transitioning;
        public TransitionState transitionState;
        void Update()
        {
            openScenes = SceneController.loadedScenes;
            scenesNotToUnload = SceneController.sceneNotToUnload;
            targetScene = SceneController.targetScene;
            transitionValue = SceneController.transitionValue;
            transitionDuration = SceneController.transitionDuration;
            transitionSpeed = SceneController.transitionSpeed;
            transitioning = SceneController.transitioning;
            transitionState = SceneController.transitionState;

        }
    }
}