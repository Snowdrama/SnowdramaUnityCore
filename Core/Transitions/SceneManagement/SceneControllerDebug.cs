using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.Transition
{
    public class SceneControllerDebug : MonoBehaviour
    {
        public TransitionState transitionState;
        public float transitionValue;
        public float transitionSpeed;
        public SceneTransition sceneTransition;
        public bool transitioning;
        [Header("Loaded Scenes")]
        public List<string> loadedScenes;
        public List<string> scenesNotToUnload;
        [Header("Debug")]
        public List<string> unload;
        public List<string> load;

        public List<string> unloadDontDestroy;
        public List<string> loadDontDestroy;
        private void Update()
        {
            transitionState = SceneController.transitionState;
            transitionValue = SceneController.transitionValue;
            sceneTransition = SceneController.targetSceneTransition;
            transitionSpeed = SceneController.transitionSpeed;


            loadedScenes = SceneController.loadedScenes_Normal;
            scenesNotToUnload = SceneController.loadedScenes_DontDestroyOnLoad;


            unload = SceneController.calculatedScenes_ToUnload;
            load = SceneController.calculatedScenes_ToLoad;

            unloadDontDestroy = SceneController.calculatedScenes_ToUnload_DontDestroyOnLoad;
            loadDontDestroy = SceneController.calculatedScenes_ToLoad_DontDestroyOnLoad;
        }
    }
}