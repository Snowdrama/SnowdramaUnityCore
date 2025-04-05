using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.PlayerLoop;
using Snowdrama.Core;

namespace Snowdrama.Transition
{
    public enum TransitionState
    {
        None,
        Start,
        HidingScene,
        SceneHidden,
        StartUnload,
        WaitingforUnload,
        StartLoad,
        WaitingForLoad,
        FakeTimeBuffer,
        RevealingScene,
        End,
    }

    [CreateAssetMenu(fileName = "SceneControllerOptions", menuName = "Snowdrama/Transitions/Scene Controller Options")]
    public class SceneControllerOptions : ScriptableObject
    {
        public bool showConsoleMessages = false;
        public bool hideRequiredSceneWarning = false;
    }
    public class SceneController
    {
        public static List<string> loadedScenes;
        public static List<string> sceneNotToUnload;

        private static SceneControllerOptions sceneControllerOptions;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Bootstrap()
        {
            loadedScenes = new List<string>();
            sceneNotToUnload = new List<string>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (!loadedScenes.Contains(scene.name))
                {
                    loadedScenes.Add(scene.name);
                }
            }

            var requredSceneListObject = Resources.Load<RequiredSceneListObject>("RequiredSceneList");
            sceneControllerOptions = Resources.Load<SceneControllerOptions>("SceneControllerOptions");

            if(sceneControllerOptions == null)
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
                        if (!loadedScenes.Contains(requiredScene.sceneName) && !sceneNotToUnload.Contains(requiredScene.sceneName))
                        {
                            //add it if it's not already in the lists. 
                            SceneManager.LoadSceneAsync(requiredScene.sceneName, LoadSceneMode.Additive);
                        }

                        //we don't want the don't destroy scene in the regular loaded scenes list
                        if (loadedScenes.Contains(requiredScene.sceneName))
                        {
                            loadedScenes.Remove(requiredScene.sceneName);
                        }
                        if (!sceneNotToUnload.Contains(requiredScene.sceneName))
                        {
                            sceneNotToUnload.Add(requiredScene.sceneName);
                        }
                    }
                    else
                    {
                        if (!loadedScenes.Contains(requiredScene.sceneName))
                        {
                            loadedScenes.Add(requiredScene.sceneName);
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


            var loopInserter = UnityPlayerLoopInserter.GetCurrent();
            loopInserter.InsertAfter(typeof(Update), typeof(SceneController), UpdateTransition);
            loopInserter.Flush();
        }


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

        public static List<string> unload = new List<string>();
        public static List<string> load = new List<string>();
        public static List<string> unloadDontDestroy = new List<string>();
        public static List<string> loadDontDestroy = new List<string>();

        public static List<string> allowedTransitions;
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
                    if(targetSceneTransition.hideSceneDuration > 0)
                    {
                        transitionSpeed = 1.0f / targetSceneTransition.hideSceneDuration;
                        transitionValue += Time.unscaledDeltaTime * transitionSpeed;
                        if (transitionValue >= 1.0f)
                        {
                            transitionValue = 1.0f;
                            transitionCallbacks.onHideCompleted?.Invoke();
                            transitionState = TransitionState.SceneHidden;
                        }
                    } else {
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
                    UnloadScenes(unload);
                    UnloadDoNotDestroyScenes(unloadDontDestroy);
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
                    LoadScenes(load);
                    LoadScenesDontDestroy(loadDontDestroy);
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
                    if(targetSceneTransition.fakeLoadBufferTime > 0)
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
                    } else {
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

        public static void LoadScenes(List<string> scenesToLoad)
        {
            for (int i = 0; i < scenesToLoad.Count; i++)
            {
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

        public static void CalculateScenesToChange()
        {
            asyncLoadData = new List<SceneTransitionAsync_LoadData>();
            asyncUnloadData = new List<SceneTransitionAsync_LoadData>();


            switch (targetSceneTransition.transitionMode)
            {
                case SceneTransitionMode.Normal:
                    unload = new List<string>(loadedScenes);
                    load = new List<string>();
                    unloadDontDestroy = new List<string>(targetSceneTransition.doNotDestroyScenesToUnload);
                    loadDontDestroy = new List<string>();

                    for (int i = 0; i < targetSceneTransition.doNotDestroyScenesToUnload.Count; i++)
                    {
                        if (sceneNotToUnload.Contains(targetSceneTransition.doNotDestroyScenesToUnload[i]))
                        {
                            unloadDontDestroy.Add(targetSceneTransition.doNotDestroyScenesToUnload[i]);
                        }
                    }

                    for (int i = 0; i < targetSceneTransition.scenes.Count; i++)
                    {
                        var newScene = targetSceneTransition.scenes[i];
                        if (unload.Contains(newScene.SceneName))
                        {
                            //if the scene is potentially being unloaded
                            if (!newScene.reloadIfAlreadyExists)
                            {
                                //And we don't want to reload, stop the unload

                                DebugLogWarning($"Preventing Unload of Scene {newScene.SceneName}", sceneControllerOptions.showConsoleMessages);
                                unload.Remove(newScene.SceneName);
                                continue;
                            }
                            else
                            {
                                //And we DO want to reload. don't stop the unload AND add to reload.
                                DebugLogWarning($"Reloading Scene {newScene.SceneName}", sceneControllerOptions.showConsoleMessages);
                                if (newScene.dontDestroyOnLoad)
                                {
                                    loadDontDestroy.Add(newScene.SceneName);
                                }
                                else
                                {
                                    load.Add(newScene.SceneName);
                                }
                            }
                        }
                        else
                        {
                            //scene isn't already loaded so load it.
                            DebugLogWarning($"Loading New Scene {newScene.SceneName}", sceneControllerOptions.showConsoleMessages);
                            if (newScene.dontDestroyOnLoad)
                            {
                                loadDontDestroy.Add(newScene.SceneName);
                            }
                            else
                            {
                                load.Add(newScene.SceneName);
                            }
                        }
                    }

                    break;
                case SceneTransitionMode.Additively:
                    unload = new List<string>();
                    load = new List<string>();
                    unloadDontDestroy = new List<string>();
                    loadDontDestroy = new List<string>();

                    
                    for (int i = 0; i < targetSceneTransition.doNotDestroyScenesToUnload.Count; i++)
                    {
                        if (sceneNotToUnload.Contains(targetSceneTransition.doNotDestroyScenesToUnload[i]))
                        {
                            unloadDontDestroy.Add(targetSceneTransition.doNotDestroyScenesToUnload[i]);
                        }
                    }

                    for (int i = 0; i < targetSceneTransition.scenes.Count; i++)
                    {
                        var newScene = targetSceneTransition.scenes[i];
                        if (loadedScenes.Contains(newScene.SceneName))
                        {
                            //if the scene is already loaded
                            if (newScene.reloadIfAlreadyExists)
                            {
                                //during addative we can choose to reload
                                //if we do than we need to explicityly unload AND reload
                                DebugLogWarning($"Reloading Scene {newScene.SceneName}", sceneControllerOptions.showConsoleMessages);
                                unload.Add(newScene.SceneName);
                                if (newScene.dontDestroyOnLoad)
                                {
                                    loadDontDestroy.Add(newScene.SceneName);
                                }
                                else
                                {
                                    load.Add(newScene.SceneName);
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
                                loadDontDestroy.Add(newScene.SceneName);
                            }
                            else
                            {
                                load.Add(newScene.SceneName);
                            }
                        }
                    }
                    break;
            }

        }



        private static void LoadSceneComplete(AsyncOperation obj)
        {
            for (int i = 0; i < asyncLoadData.Count; i++)
            {
                //PLEASE let this work
                if (asyncLoadData[i].asyncOperation == obj)
                {
                    DebugLogWarning($"Load Complete for Scene {asyncLoadData[i].sceneName}", sceneControllerOptions.showConsoleMessages);
                    asyncLoadData[i].complete = true;
                    if (!loadedScenes.Contains(asyncLoadData[i].sceneName))
                    {
                        loadedScenes.Add(asyncLoadData[i].sceneName);
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
                    if (!sceneNotToUnload.Contains(asyncLoadData[i].sceneName))
                    {
                        sceneNotToUnload.Add(asyncLoadData[i].sceneName);
                    }
                }
            }
        }

        private static void UnloadSceneComplete(AsyncOperation obj)
        {
            for (int i = 0; i < asyncUnloadData.Count; i++)
            {
                if (asyncUnloadData[i].asyncOperation == obj)
                {
                    DebugLogWarning($"Unload Complete for Scene {asyncUnloadData[i].sceneName}", sceneControllerOptions.showConsoleMessages);
                    asyncUnloadData[i].complete = true;

                    if (loadedScenes.Contains(asyncUnloadData[i].sceneName))
                    {
                        loadedScenes.Remove(asyncUnloadData[i].sceneName);
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

                    if (sceneNotToUnload.Contains(asyncUnloadData[i].sceneName))
                    {
                        sceneNotToUnload.Remove(asyncUnloadData[i].sceneName);
                    }
                }
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
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void AfterSceneLoad()
        {
            if (loadedScenes.Count > 1)
            {
                //unload each scene except scene index 0
                for (int i = 0; i < loadedScenes.Count; i++)
                {
                    if (i != 0)
                    {
                        DebugLog($"Unloading Excess Scene {loadedScenes[i]}", sceneControllerOptions.showConsoleMessages);
                        SceneManager.UnloadSceneAsync(loadedScenes[i]);
                    }
                }
            }
        }


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
    }
}