using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Snowdrama.Transition
{
    [ExecuteAlways]
    public class SceneControllerDebug : MonoBehaviour
    {
        [SerializeField] private List<string> RequiredScenes;
        [SerializeField] private List<WrapperSceneData> WrapperScenes;
        [SerializeField] private List<SceneData> Scenes;

        [Header("Transition Info")]
        [SerializeField] private TransitionState transitionState;
        [SerializeField] private float transitionValue;
        [SerializeField] private float transitionSpeed;
        [SerializeField] private SceneTransition sceneTransition;
        [SerializeField] private bool transitioning;

        [Header("Currently Loaded Scenes")]
        [SerializeField] private List<string> loadedScenes_Normal;
        [SerializeField] private List<string> loadedScenes_Wrapper;
        [SerializeField] private List<string> loadedScenes_Required;

        [Header("Target Scenes")]
        [SerializeField] private List<string> targetScenes_Normal;
        [SerializeField] private List<string> targetScenes_Wrapper;

        [Header("Loading Next")]
        [SerializeField] private List<string> calculatedScenes_ToLoad;
        [SerializeField] private List<string> calculatedScenes_ToLoad_Wrappers;

        [Header("Unloading Next")]
        [SerializeField] private List<string> calculatedScenes_ToUnload;
        [SerializeField] private List<string> calculatedScenes_ToUnload_Wrappers;

        [Header("Async Data")]
        [SerializeField] private List<SceneTransitionAsync_LoadData> asyncLoadData = new List<SceneTransitionAsync_LoadData>();
        [SerializeField] private List<SceneTransitionAsync_LoadData> asyncUnloadData = new List<SceneTransitionAsync_LoadData>();

        [Header("Waiting To Load")]
        [SerializeField] private List<string> WaitingToLoad = new List<string>();
        //basically take all static things and make them debugable by
        //displaying them in the editor at runtime;
        private void Update()
        {
            RequiredScenes = SceneController.RequiredScenes;
            WrapperScenes = SceneController.WrapperScenes.Values.ToList();
            Scenes = SceneController.Scenes.Values.ToList();

            //transitionState = SceneControllerJSONTest.transitionState;
            //transitionValue = SceneControllerJSONTest.transitionValue;
            //sceneTransition = SceneControllerJSONTest.targetSceneTransition;
            //transitionSpeed = SceneControllerJSONTest.transitionSpeed;

            loadedScenes_Normal = SceneController.loadedScenes_Normal;
            loadedScenes_Wrapper = SceneController.loadedScenes_Wrappers;
            loadedScenes_Required = SceneController.loadedScenes_Required;

            targetScenes_Normal = SceneController.targetScenes_Normal;
            targetScenes_Wrapper = SceneController.targetScenes_Wrappers;

            calculatedScenes_ToLoad = SceneController.calculatedScenes_ToLoad;
            calculatedScenes_ToLoad_Wrappers = SceneController.calculatedScenes_ToLoad_Wrappers;

            calculatedScenes_ToUnload = SceneController.calculatedScenes_ToUnload;
            calculatedScenes_ToUnload_Wrappers = SceneController.calculatedScenes_ToUnload_Wrappers;

            asyncLoadData = SceneController.asyncLoadData;
            asyncLoadData = SceneController.asyncUnloadData;
            WaitingToLoad = SceneController.WaitingToLoad;
        }
    }
}