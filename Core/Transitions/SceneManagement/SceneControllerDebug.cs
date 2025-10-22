using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Snowdrama.Transition
{
    public class SceneControllerDebug : MonoBehaviour
    {
        public List<string> RequiredScenes;
        public List<WrapperSceneData> WrapperScenes;
        public List<SceneData> Scenes;

        [Header("Transition Info")]
        public TransitionState transitionState;
        public float transitionValue;
        public float transitionSpeed;
        public SceneTransition sceneTransition;
        public bool transitioning;

        [Header("Currently Loaded Scenes")]
        public List<string> loadedScenes_Normal;
        public List<string> loadedScenes_Wrapper;
        public List<string> loadedScenes_Required;

        [Header("Target Scenes")]
        public List<string> targetScenes_Normal;
        public List<string> targetScenes_Wrapper;

        [Header("Loading Next")]
        public List<string> calculatedScenes_ToLoad;
        public List<string> calculatedScenes_ToLoad_Wrappers;

        [Header("Unloading Next")]
        public List<string> calculatedScenes_ToUnload;
        public List<string> calculatedScenes_ToUnload_Wrappers;

        //basically take all static things and make them debugable by
        //displaying them in the editor at runtime;
        private void Update()
        {
            RequiredScenes = global::SceneController.RequiredScenes;
            WrapperScenes = global::SceneController.WrapperScenes.Values.ToList();
            Scenes = global::SceneController.Scenes.Values.ToList();

            //transitionState = SceneControllerJSONTest.transitionState;
            //transitionValue = SceneControllerJSONTest.transitionValue;
            //sceneTransition = SceneControllerJSONTest.targetSceneTransition;
            //transitionSpeed = SceneControllerJSONTest.transitionSpeed;

            loadedScenes_Normal = global::SceneController.loadedScenes_Normal;
            loadedScenes_Wrapper = global::SceneController.loadedScenes_Wrappers;
            loadedScenes_Required = global::SceneController.loadedScenes_Required;

            targetScenes_Normal = global::SceneController.targetScenes_Normal;
            targetScenes_Wrapper = global::SceneController.targetScenes_Wrappers;

            calculatedScenes_ToLoad = global::SceneController.calculatedScenes_ToLoad;
            calculatedScenes_ToLoad_Wrappers = global::SceneController.calculatedScenes_ToLoad_Wrappers;

            calculatedScenes_ToUnload = global::SceneController.calculatedScenes_ToUnload;
            calculatedScenes_ToUnload_Wrappers = global::SceneController.calculatedScenes_ToUnload_Wrappers;
        }
    }
}