using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.PlayerLoop;


namespace Snowdrama.Transition
{
    public enum TransitionState
    {
        None,
        Start,
        HidingScene,
        SceneHidden,
        WaitingForLoad,
        RevealingScene,
        End,
    }
    public class SceneController
    {
        public static List<string> loadedScenes;
        public static List<string> sceneNotToUnload;

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
            if(requredSceneListObject != null)
            {
                for (int i = 0; i < requredSceneListObject.listOfRequiredSceneNames.Count; i++)
                {
                    var requiredScene = requredSceneListObject.listOfRequiredSceneNames[i];

                    SceneManager.LoadSceneAsync(requiredScene.sceneName, LoadSceneMode.Additive);
                    if (requiredScene.dontDestroyOnLoad)
                    {
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


            var loopInserter = UnityPlayerLoopInserter.GetCurrent();
            loopInserter.InsertInto(typeof(Update), typeof(SceneController), UpdateTransition);
            loopInserter.Flush();
        }

        public static string targetScene;
        public static float transitionValue;
        public static float transitionDuration;
        public static float transitionSpeed;
        public static bool transitioning;
        public static TransitionState transitionState;
        public static Action completeCallback;
        public static void UpdateTransition()
        {
            switch (transitionState)
            {
                case TransitionState.None:
                    break;

                case TransitionState.Start:
                    transitionState = TransitionState.HidingScene;
                    break;

                case TransitionState.HidingScene:
                    transitionSpeed = 1.0f / transitionDuration;
                    transitionValue += Time.deltaTime;
                    if (transitionValue >= 1.0f)
                    {
                        transitionState = TransitionState.SceneHidden;
                    }
                    break;

                case TransitionState.SceneHidden:

                    SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive).completed += LoadSceneComplete;
                    transitionState = TransitionState.WaitingForLoad;
                    break;

                case TransitionState.WaitingForLoad:
                    break;

                case TransitionState.RevealingScene:
                    transitionSpeed = 1.0f / transitionDuration;
                    transitionValue -= Time.deltaTime;
                    if (transitionValue <= 0.0f)
                    {
                        transitionState = TransitionState.End;
                        completeCallback?.Invoke();
                    }
                    break;

                case TransitionState.End:
                    transitionState = TransitionState.None;
                    break;
            }
        }

        private static void LoadSceneComplete(AsyncOperation obj)
        {
            if (loadedScenes.Count > 0)
            {
                //unload each scene except scene index 0
                for (int i = 0; i < loadedScenes.Count; i++)
                {
                    SceneManager.UnloadSceneAsync(loadedScenes[i]);
                }
            }
            loadedScenes.Clear();
            loadedScenes.Add(targetScene);
            transitionState = TransitionState.RevealingScene;
        }

        public static void StartTransition(string setTargetScene, float setDuration, Action setCompleteCallback)
        {
            transitionValue = 0;
            transitionDuration = setDuration;
            targetScene = setTargetScene;
            completeCallback = setCompleteCallback;
            transitionState = TransitionState.Start;
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
                        Debug.Log($"Unloading Excess Scene {loadedScenes[i]}");
                        SceneManager.UnloadSceneAsync(loadedScenes[i]);
                    }
                }
            }
        }
    }
}