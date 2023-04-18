using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Snowdrama.Transition
{
    public class SceneController
    {
        public static List<string> loadedScenes;
        public const string TRANSITION_SCENE = "TransitionScene";
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


        private static void RequiredComponentsLoaded(AsyncOperation obj)
        {
            sceneNotToUnload.Add(TRANSITION_SCENE);
        }


        public static int loadSceneInstructionCount = 0;
        public static Action completeCallback;
        public static void LoadScene(string newSceneName, Action setCompleteCallback)
        {
            completeCallback = setCompleteCallback;
            foreach (var scene in loadedScenes)
            {
                Debug.Log($"Unloading Scene: {scene}");
                loadSceneInstructionCount++;
                SceneManager.UnloadSceneAsync(scene).completed += LoadSceneInstructionComplete;
            }
            loadedScenes.Clear();
            loadSceneInstructionCount++;
            SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive).completed += LoadSceneInstructionComplete;
            loadedScenes.Add(newSceneName);
        }

        private static void LoadSceneInstructionComplete(AsyncOperation obj)
        {
            loadSceneInstructionCount--;
            if (loadSceneInstructionCount <= 0)
            {
                completeCallback?.Invoke();
                completeCallback = null;
            }
        }
    }
}