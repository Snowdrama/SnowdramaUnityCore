using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.PlayerLoop;
using Snowdrama.Core;

namespace Snowdrama.Transition
{
    public class SceneController : MonoBehaviour
    {
        public static List<string> requiredScenesLoaded = new List<string>();

        public static List<string> loadedScenes_Normal = new List<string>();
        public static List<string> loadedScenes_DontDestroyOnLoad = new List<string>();

        private static SceneControllerOptions sceneControllerOptions;

        public static SceneTransition targetSceneTransition;

        public static float transitionValue;
        public static float transitionSpeed;

        public static float fakeBufferTime;
        //public static float transitionHideDuration;
        //public static float transitionShowDuration;
        //public static bool transitioning;

        public static int scenesLoaded;
        public static int scenesLoadingCount;
        public static List<SceneTransitionAsync_LoadData> asyncLoadData;
        public static List<SceneTransitionAsync_LoadData> asyncUnloadData;

        public static TransitionState transitionState;

        public struct TransitionCallbacks
        {
            public Action onTransitionStarted;
            public Action onHideStarted;
            public Action onHideCompleted;
            public Action onScenesLoaded;
            public Action onShowStarted;
            public Action onShowCompleted;
            public Action onTransitionCompltete;
        }

        public static TransitionCallbacks transitionCallbacks;

        public static bool isTransitioning;

        public static List<string> calculatedScenes_ToUnload = new List<string>();
        public static List<string> calculatedScenes_ToLoad = new List<string>();
        public static List<string> calculatedScenes_ToUnload_DontDestroyOnLoad = new List<string>();
        public static List<string> calculatedScenes_ToLoad_DontDestroyOnLoad = new List<string>();
        public static List<string> allowedTransitions;

        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Bootstrap()
        {
            //add all scenes from the editor as loaded.
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (!loadedScenes_Normal.Contains(scene.name))
                {
                    loadedScenes_Normal.Add(scene.name);
                }
            }

            //load required scenes
            LoadRequiredScenes();


            //TODO: We need to check into this it seems to be causing issues
            //TODO: For now this needs to be added to the required scene and put manually into the loop
            //not using the inserter...
            //var loopInserter = UnityPlayerLoopInserter.GetCurrent();            
            //loopInserter.InsertAfter(typeof(Update), typeof(SceneController), UpdateTransition);
            //loopInserter.Flush();
        }

        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void AfterSceneLoad()
        {
            if (loadedScenes_Normal.Count > 1)
            {
                //unload each scene except scene index 0
                for (int i = 0; i < loadedScenes_Normal.Count; i++)
                {
                    if (i != 0)
                    {
                        DebugLog($"Unloading Excess Scene {loadedScenes_Normal[i]}", sceneControllerOptions.showConsoleMessages);
                        SceneManager.UnloadSceneAsync(loadedScenes_Normal[i]);
                    }
                }
            }
        }



        public void Update()
        {
            SceneController.UpdateTransition();
        }

        public static void UpdateTransition()
        {
            switch (transitionState)
            {
                case TransitionState.None:
                    break;

                case TransitionState.Start:
                    transitionCallbacks.onHideStarted?.Invoke();
                    transitionState = TransitionState.HidingScene;
                    break;
                case TransitionState.HidingScene:
                    if (targetSceneTransition.hideSceneDuration > 0)
                    {
                        transitionSpeed = 1.0f / targetSceneTransition.hideSceneDuration;
                        transitionValue += Time.unscaledDeltaTime * transitionSpeed;
                        if (transitionValue >= 1.0f)
                        {
                            transitionValue = 1.0f;
                            transitionCallbacks.onHideCompleted?.Invoke();
                            transitionState = TransitionState.SceneHidden;
                        }
                    }
                    else
                    {
                        //if it's 0 it should be instant
                        transitionValue = 1.0f;
                        transitionCallbacks.onHideCompleted?.Invoke();
                        transitionState = TransitionState.SceneHidden;
                    }
                    break;

                case TransitionState.SceneHidden:
                    CalculateScenesToChange();
                    transitionState = TransitionState.StartUnload;
                    break;
                case TransitionState.StartUnload:
                    UnloadScenes(calculatedScenes_ToUnload);
                    UnloadDoNotDestroyScenes(calculatedScenes_ToUnload_DontDestroyOnLoad);
                    transitionState = TransitionState.WaitingforUnload;
                    break;
                case TransitionState.WaitingforUnload:
                    var incomplteteUnloads = asyncUnloadData.Where(x => x.complete == false).ToList();
                    if (incomplteteUnloads.Count == 0)
                    {
                        transitionState = TransitionState.StartLoad;
                    }
                    break;
                case TransitionState.StartLoad:
                    Debug.Log("StartLoad");
                    LoadScenes(calculatedScenes_ToLoad);
                    LoadScenesDontDestroy(calculatedScenes_ToLoad_DontDestroyOnLoad);
                    transitionState = TransitionState.WaitingForLoad;
                    break;

                case TransitionState.WaitingForLoad:
                    var incomplteteLoads = asyncLoadData.Where(x => x.complete == false).ToList();
                    if (incomplteteLoads.Count == 0)
                    {
                        transitionCallbacks.onScenesLoaded?.Invoke();
                        transitionState = TransitionState.FakeTimeBuffer;
                    }
                    break;
                case TransitionState.FakeTimeBuffer:
                    if (targetSceneTransition.fakeLoadBufferTime > 0)
                    {
                        transitionSpeed = 1.0f / targetSceneTransition.fakeLoadBufferTime;
                        fakeBufferTime += Time.unscaledDeltaTime * transitionSpeed;
                        if (fakeBufferTime >= 1.0f)
                        {
                            fakeBufferTime = 0;
                            transitionState = TransitionState.RevealingScene;
                            transitionCallbacks.onShowStarted?.Invoke();
                        }
                    }
                    else
                    {
                        //if it's 0 it should be instant
                        fakeBufferTime = 0;
                        transitionState = TransitionState.RevealingScene;
                        transitionCallbacks.onShowStarted?.Invoke();
                    }
                    break;
                case TransitionState.RevealingScene:
                    if (targetSceneTransition.showSceneDuration > 0)
                    {
                        transitionSpeed = 1.0f / targetSceneTransition.showSceneDuration;
                        transitionValue -= Time.unscaledDeltaTime * transitionSpeed;
                        if (transitionValue <= 0.0f)
                        {
                            transitionValue = 0.0f;
                            transitionCallbacks.onShowCompleted?.Invoke();
                            transitionState = TransitionState.End;
                        }
                    }
                    else
                    {
                        //if it's 0 it should be instant
                        transitionValue = 0.0f;
                        transitionCallbacks.onShowCompleted?.Invoke();
                        transitionState = TransitionState.End;
                    }
                    break;

                case TransitionState.End:
                    transitionCallbacks.onTransitionCompltete?.Invoke();
                    transitionState = TransitionState.None;
                    isTransitioning = false;
                    break;
            }
        }

        public static void StartTransition(SceneTransition setSceneTransition)
        {
            if (!isTransitioning)
            {
                isTransitioning = true;
                transitionValue = 0;
                allowedTransitions = setSceneTransition.allowedTransitionNames;
                targetSceneTransition = setSceneTransition;
                transitionCallbacks.onTransitionStarted?.Invoke();
                transitionState = TransitionState.Start;
            }
            else
            {
                Debug.LogError("Currently in a transition! Can't transition again!");
            }
        }

        public static void CalculateScenesToChange()
        {
            asyncLoadData = new List<SceneTransitionAsync_LoadData>();
            asyncUnloadData = new List<SceneTransitionAsync_LoadData>();

            switch (targetSceneTransition.transitionMode)
            {
                case SceneTransitionMode.Normal:
                    calculatedScenes_ToUnload = new List<string>(loadedScenes_Normal);
                    calculatedScenes_ToLoad = new List<string>();
                    calculatedScenes_ToUnload_DontDestroyOnLoad = new List<string>();
                    calculatedScenes_ToLoad_DontDestroyOnLoad = new List<string>();

                    //loop over each scene in the force unload.
                    for (int i = 0; i < targetSceneTransition.doNotDestroyScenesToUnload.Count; i++)
                    {
                        //if we've marked it as don't destroy on load
                        if (loadedScenes_DontDestroyOnLoad.Contains(targetSceneTransition.doNotDestroyScenesToUnload[i]))
                        {
                            //add it to this list to make sure to force destroy it. 
                            calculatedScenes_ToUnload_DontDestroyOnLoad.Add(targetSceneTransition.doNotDestroyScenesToUnload[i]);
                        }
                    }

                    for (int i = 0; i < targetSceneTransition.scenes.Count; i++)
                    {
                        var newScene = targetSceneTransition.scenes[i];

                        //if we have a scene loaded and it's marked to be loaded again
                        if (loadedScenes_Normal.Contains(newScene.SceneName) && calculatedScenes_ToUnload.Contains(newScene.SceneName))
                        {
                            //check if we want to reload
                            if (!newScene.reloadIfAlreadyExists)
                            {
                                //we don't want to reload, stop the unload
                                DebugLogWarning($"Preventing Unload of Scene {newScene.SceneName}", sceneControllerOptions.showConsoleMessages);
                                calculatedScenes_ToUnload.Remove(newScene.SceneName);
                                continue;
                            }
                            else
                            {
                                //And we DO want to reload. don't stop the unload AND add to reload.
                                DebugLogWarning($"Reloading Scene {newScene.SceneName}", sceneControllerOptions.showConsoleMessages);
                                if (newScene.dontDestroyOnLoad)
                                {
                                    calculatedScenes_ToLoad_DontDestroyOnLoad.Add(newScene.SceneName);
                                }
                                else
                                {
                                    calculatedScenes_ToLoad.Add(newScene.SceneName);
                                }
                            }
                        }
                        else if (loadedScenes_DontDestroyOnLoad.Contains(newScene.SceneName))
                        {
                            //the scene is already loaded as a don't destroy on load scene
                            if (!newScene.reloadIfAlreadyExists)
                            {
                                //we want to reload so we want to unload it as well as load it again.
                                calculatedScenes_ToUnload_DontDestroyOnLoad.Add(newScene.SceneName);
                                calculatedScenes_ToLoad_DontDestroyOnLoad.Add(newScene.SceneName);
                            }
                            //otherwise it's already loaded... so don't load it again.
                        }
                        else
                        {
                            //scene isn't already loaded so load it.
                            DebugLogWarning($"Loading New Scene {newScene.SceneName}", sceneControllerOptions.showConsoleMessages);
                            if (newScene.dontDestroyOnLoad)
                            {
                                calculatedScenes_ToLoad_DontDestroyOnLoad.Add(newScene.SceneName);
                            }
                            else
                            {
                                calculatedScenes_ToLoad.Add(newScene.SceneName);
                            }
                        }
                    }

                    break;
                case SceneTransitionMode.Additively:
                    calculatedScenes_ToUnload = new List<string>();
                    calculatedScenes_ToLoad = new List<string>();
                    calculatedScenes_ToUnload_DontDestroyOnLoad = new List<string>();
                    calculatedScenes_ToLoad_DontDestroyOnLoad = new List<string>();

                    //loop over each scene in the force unload.
                    for (int i = 0; i < targetSceneTransition.doNotDestroyScenesToUnload.Count; i++)
                    {
                        //if we've marked it as don't destroy on load
                        if (loadedScenes_DontDestroyOnLoad.Contains(targetSceneTransition.doNotDestroyScenesToUnload[i]))
                        {
                            //add it to this list to make sure to force destroy it. 
                            calculatedScenes_ToUnload_DontDestroyOnLoad.Add(targetSceneTransition.doNotDestroyScenesToUnload[i]);
                        }
                    }

                    for (int i = 0; i < targetSceneTransition.scenes.Count; i++)
                    {
                        var newScene = targetSceneTransition.scenes[i];
                        if (loadedScenes_Normal.Contains(newScene.SceneName))
                        {
                            //if the scene is already loaded
                            if (newScene.reloadIfAlreadyExists)
                            {
                                //during addative we can choose to reload
                                //if we do than we need to explicityly unload AND reload
                                DebugLogWarning($"Reloading Scene {newScene.SceneName}", sceneControllerOptions.showConsoleMessages);
                                calculatedScenes_ToUnload.Add(newScene.SceneName);
                                if (newScene.dontDestroyOnLoad)
                                {
                                    calculatedScenes_ToLoad_DontDestroyOnLoad.Add(newScene.SceneName);
                                }
                                else
                                {
                                    calculatedScenes_ToLoad.Add(newScene.SceneName);
                                }
                            }
                            else
                            {
                                DebugLogWarning($"Doing Nothing for Scene {newScene.SceneName} since it's already loaded and reloading is not set", sceneControllerOptions.showConsoleMessages);
                            }
                        }
                        else
                        {
                            //scene isn't already loaded so load it.
                            DebugLogWarning($"Loading New Scene {newScene.SceneName}", sceneControllerOptions.showConsoleMessages);
                            if (newScene.dontDestroyOnLoad)
                            {
                                calculatedScenes_ToLoad_DontDestroyOnLoad.Add(newScene.SceneName);
                            }
                            else
                            {
                                calculatedScenes_ToLoad.Add(newScene.SceneName);
                            }
                        }
                    }
                    break;
            }
        }


        public static bool IsSceneLoaded(string findSceneName)
        {
            if (loadedScenes_Normal.Contains(findSceneName))
            {
                return true;
            }
            return false;
        }

        public static bool IsDontDestroySceneIsLoaded(string findSceneName)
        {
            if (loadedScenes_DontDestroyOnLoad.Contains(findSceneName))
            {
                return true;
            }
            return false;
        }


        #region Load Functions
        public static void LoadScenes(List<string> scenesToLoad)
        {
            for (int i = 0; i < scenesToLoad.Count; i++)
            {
                Debug.LogWarning($"Loading Scene {scenesToLoad[i]}");
                var asyncOperation = SceneManager.LoadSceneAsync(scenesToLoad[i], LoadSceneMode.Additive);
                asyncOperation.completed += LoadSceneComplete;
                asyncLoadData.Add(new SceneTransitionAsync_LoadData()
                {
                    sceneName = scenesToLoad[i],
                    asyncOperation = asyncOperation,
                    complete = false,
                });
            }
        }
        public static void LoadScenesDontDestroy(List<string> scenesToLoad)
        {
            for (int i = 0; i < scenesToLoad.Count; i++)
            {
                Debug.LogWarning($"Loading Scene {scenesToLoad[i]}");
                var asyncOperation = SceneManager.LoadSceneAsync(scenesToLoad[i], LoadSceneMode.Additive);
                asyncOperation.completed += LoadSceneDontDestroyComplete;
                asyncLoadData.Add(new SceneTransitionAsync_LoadData()
                {
                    sceneName = scenesToLoad[i],
                    asyncOperation = asyncOperation,
                    complete = false,
                });
            }
        }
        #endregion

        #region Unload Functions

        public static void UnloadScenes(List<string> scenesToUnload)
        {
            for (int i = 0; i < scenesToUnload.Count; i++)
            {
                DebugLog($"Unloading Scene {scenesToUnload[i]}", sceneControllerOptions.showConsoleMessages);
                var asyncOperation = SceneManager.UnloadSceneAsync(scenesToUnload[i]);
                asyncOperation.completed += UnloadSceneComplete;
                asyncUnloadData.Add(new SceneTransitionAsync_LoadData()
                {
                    sceneName = scenesToUnload[i],
                    asyncOperation = asyncOperation,
                    complete = false,
                });
            }
        }
        public static void UnloadDoNotDestroyScenes(List<string> scenesToUnload)
        {
            for (int i = 0; i < scenesToUnload.Count; i++)
            {
                DebugLog($"Unloading Scene {scenesToUnload[i]}", sceneControllerOptions.showConsoleMessages);
                var asyncOperation = SceneManager.UnloadSceneAsync(scenesToUnload[i]);
                asyncOperation.completed += UnloadSceneDoNotDestroyComplete;
                asyncUnloadData.Add(new SceneTransitionAsync_LoadData()
                {
                    sceneName = scenesToUnload[i],
                    asyncOperation = asyncOperation,
                    complete = false,
                });
            }
        }
        #endregion

        #region Load Complete Callbacks
        private static void LoadSceneComplete(AsyncOperation obj)
        {
            for (int i = 0; i < asyncLoadData.Count; i++)
            {
                //PLEASE let this work
                if (asyncLoadData[i].asyncOperation == obj)
                {
                    DebugLogWarning($"Load Complete for Scene {asyncLoadData[i].sceneName}", sceneControllerOptions.showConsoleMessages);
                    asyncLoadData[i].complete = true;
                    if (!loadedScenes_Normal.Contains(asyncLoadData[i].sceneName))
                    {
                        loadedScenes_Normal.Add(asyncLoadData[i].sceneName);
                    }
                }
            }
        }
        private static void LoadSceneDontDestroyComplete(AsyncOperation obj)
        {
            for (int i = 0; i < asyncLoadData.Count; i++)
            {
                if (asyncLoadData[i].asyncOperation == obj)
                {
                    DebugLogWarning($"Load Dont Destroy Complete for Scene {asyncLoadData[i].sceneName}", sceneControllerOptions.showConsoleMessages);
                    asyncLoadData[i].complete = true;
                    if (!loadedScenes_DontDestroyOnLoad.Contains(asyncLoadData[i].sceneName))
                    {
                        loadedScenes_DontDestroyOnLoad.Add(asyncLoadData[i].sceneName);
                    }
                }
            }
        }
        #endregion

        #region Unload Complete Callbacks
        private static void UnloadSceneComplete(AsyncOperation obj)
        {
            for (int i = 0; i < asyncUnloadData.Count; i++)
            {
                if (asyncUnloadData[i].asyncOperation == obj)
                {
                    DebugLogWarning($"Unload Complete for Scene {asyncUnloadData[i].sceneName}", sceneControllerOptions.showConsoleMessages);
                    asyncUnloadData[i].complete = true;

                    if (loadedScenes_Normal.Contains(asyncUnloadData[i].sceneName))
                    {
                        loadedScenes_Normal.Remove(asyncUnloadData[i].sceneName);
                    }
                }
            }
        }
        private static void UnloadSceneDoNotDestroyComplete(AsyncOperation obj)
        {
            for (int i = 0; i < asyncUnloadData.Count; i++)
            {
                if (asyncUnloadData[i].asyncOperation == obj)
                {
                    DebugLogWarning($"Unload Complete for Scene {asyncUnloadData[i].sceneName}", sceneControllerOptions.showConsoleMessages);
                    asyncUnloadData[i].complete = true;

                    if (loadedScenes_DontDestroyOnLoad.Contains(asyncUnloadData[i].sceneName))
                    {
                        loadedScenes_DontDestroyOnLoad.Remove(asyncUnloadData[i].sceneName);
                    }
                }
            }
        }
        #endregion



        #region RequiredScenes
        public static void LoadRequiredScenes()
        {

            var requredSceneListObject = Resources.Load<RequiredSceneListObject>("RequiredSceneList");
            sceneControllerOptions = Resources.Load<SceneControllerOptions>("SceneControllerOptions");

            if (sceneControllerOptions == null)
            {
                sceneControllerOptions = ScriptableObject.CreateInstance<SceneControllerOptions>();
            }

            if (requredSceneListObject != null)
            {
                for (int i = 0; i < requredSceneListObject.listOfRequiredSceneNames.Count; i++)
                {
                    var requiredScene = requredSceneListObject.listOfRequiredSceneNames[i];

                    if (requiredScene.dontDestroyOnLoad)
                    {
                        if (!loadedScenes_Normal.Contains(requiredScene.sceneName) && !loadedScenes_DontDestroyOnLoad.Contains(requiredScene.sceneName))
                        {
                            //add it if it's not already in the lists. 
                            SceneManager.LoadSceneAsync(requiredScene.sceneName, LoadSceneMode.Additive);
                        }

                        //we don't want the don't destroy scene in the regular loaded scenes list
                        if (loadedScenes_Normal.Contains(requiredScene.sceneName))
                        {
                            loadedScenes_Normal.Remove(requiredScene.sceneName);
                        }
                        if (!loadedScenes_DontDestroyOnLoad.Contains(requiredScene.sceneName))
                        {
                            loadedScenes_DontDestroyOnLoad.Add(requiredScene.sceneName);
                        }
                    }
                    else
                    {
                        if (!loadedScenes_Normal.Contains(requiredScene.sceneName))
                        {
                            loadedScenes_Normal.Add(requiredScene.sceneName);
                        }
                    }
                }
            }
            else
            {
                DebugLogWarning("No Required Scene List asset named 'RequiredSceneList' is in" +
                    " the Resources folder. If there are no Required scenes please place a " +
                    "SceneControllerOptions object in the Resources folder and check Hide Warnings",
                    !sceneControllerOptions.hideRequiredSceneWarning);
            }

        }
        #endregion

        #region Debug
        public static void DebugLog(string log, bool enableFlag = false, GameObject target = null)
        {
            if (enableFlag)
            {
                Debug.Log(log, target);
            }
        }
        public static void DebugLogWarning(string log, bool enableFlag = false, GameObject target = null)
        {
            if (enableFlag)
            {
                Debug.LogWarning(log, target);
            }
        }
        public static void DebugLogError(string log, bool enableFlag = false, GameObject target = null)
        {
            if (enableFlag)
            {
                Debug.LogError(log, target);
            }
        }
        #endregion

    }
}