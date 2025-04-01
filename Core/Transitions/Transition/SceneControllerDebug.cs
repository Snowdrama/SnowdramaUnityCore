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
        [Header("Debug")]
        public List<string> unload;
        public List<string> load;

        public List<string> unloadDontDestroy;
        public List<string> loadDontDestroy;
        void Update()
        {
            transitionState = SceneController.transitionState;
            transitionValue = SceneController.transitionValue;
            sceneTransition = SceneController.targetSceneTransition;
            transitionSpeed = SceneController.transitionSpeed;


            loadedScenes = SceneController.loadedScenes;



            unload = SceneController.unload;
            load = SceneController.load;

            unloadDontDestroy = SceneController.unloadDontDestroy;
            loadDontDestroy = SceneController.loadDontDestroy;
        }
    }
}