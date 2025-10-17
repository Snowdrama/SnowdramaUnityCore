using System;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.Transition
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "SceneTransitionObject", menuName = "Snowdrama/Transitions/Scene Transition")]
    public class SceneTransitionObject : ScriptableObject
    {
        [SerializeField] private SceneTransition sceneTransition;

        public void TransitionToThis()
        {
            Debug.Log($"[{this.name}]: Going To Scene");
            SceneController.StartTransition(sceneTransition);
        }
    }
    public enum SceneTransitionMode
    {

        [Tooltip("Normal")]
        Normal,
        [Tooltip("Adds the listed scenes without removing any existing senes")]
        Additively,
    }


    [System.Serializable]
    public class SceneTransition
    {

        [Tooltip("How to manage the existing scnees")]
        [SerializeField] public SceneTransitionMode transitionMode;

        [Tooltip("A list of scenes to calculatedScenes_ToLoad_Normal")]
        [SerializeField] public List<SceneTransitionData> scenes = new List<SceneTransitionData>();

        [Header("Time")]
        [SerializeField] public float hideSceneDuration = 1.0f;

        [Tooltip("Add a fake calculatedScenes_ToLoad_Normal time to make sure the transition doesn't look ugly when the scene loads too fast")]
        [SerializeField] public float fakeLoadBufferTime = 1.0f;
        [SerializeField] public float showSceneDuration = 1.0f;

        [Tooltip("This is a list of the transition names allowed to be used" +
            "if this is empty or if none of them are found a random transition will be chosen")]
        [SerializeField] public List<string> allowedTransitionNames = new List<string>();

        [Header("Force Unload DontDestroy Scene")]
        [Tooltip("A list of DontDestroy scenes to force calculatedScenes_ToUnload_Normal")]
        [SerializeField] public List<string> doNotDestroyScenesToUnload = new List<string>();
    }

    [System.Serializable]
    public class SceneTransitionData
    {
        public string SceneName;
        public bool dontDestroyOnLoad;
        public bool allowLoadingMultiple;
        public bool reloadIfAlreadyExists;
    }


    [System.Serializable]

    public class SceneTransitionAsync_LoadData
    {
        public string sceneName;
        public AsyncOperation asyncOperation;
        public bool complete = false;
    }
}
