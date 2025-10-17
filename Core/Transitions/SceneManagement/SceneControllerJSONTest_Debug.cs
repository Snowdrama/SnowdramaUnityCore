using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Snowdrama.Transition
{
    public class SceneControllerJSONTest_Debug : MonoBehaviour
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
            RequiredScenes = SceneControllerJSONTest.RequiredScenes;
            WrapperScenes = SceneControllerJSONTest.WrapperScenes.Values.ToList();
            Scenes = SceneControllerJSONTest.Scenes.Values.ToList();

            //transitionState = SceneControllerJSONTest.transitionState;
            //transitionValue = SceneControllerJSONTest.transitionValue;
            //sceneTransition = SceneControllerJSONTest.targetSceneTransition;
            //transitionSpeed = SceneControllerJSONTest.transitionSpeed;

            loadedScenes_Normal = SceneControllerJSONTest.loadedScenes_Normal;
            loadedScenes_Wrapper = SceneControllerJSONTest.loadedScenes_Wrappers;
            loadedScenes_Required = SceneControllerJSONTest.loadedScenes_Required;

            targetScenes_Normal = SceneControllerJSONTest.targetScenes_Normal;
            targetScenes_Wrapper = SceneControllerJSONTest.targetScenes_Wrappers;

            calculatedScenes_ToLoad = SceneControllerJSONTest.calculatedScenes_ToLoad;
            calculatedScenes_ToLoad_Wrappers = SceneControllerJSONTest.calculatedScenes_ToLoad_Wrappers;

            calculatedScenes_ToUnload = SceneControllerJSONTest.calculatedScenes_ToUnload;
            calculatedScenes_ToUnload_Wrappers = SceneControllerJSONTest.calculatedScenes_ToUnload_Wrappers;
        }
    }
}