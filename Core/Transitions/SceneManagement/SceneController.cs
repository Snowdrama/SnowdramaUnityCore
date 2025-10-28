using Codice.Client.BaseCommands.WkStatus.Printers;
using Snowdrama.Transition;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using static Codice.Client.BaseCommands.Import.Commit;

public class SceneController : MonoBehaviour
{
    //transition related
    private static string TransitionHubName = "TransitionDriver"; //this is the default name
    private static MessageHub TransitionMessageHub;
    private static StartHideTransitionMessage StartHideTransitionMessage;
    private static StartShowTransitionMessage StartShowTransitionMessage;

    public static List<string> RequiredScenes = new List<string>();
    public static Dictionary<string, WrapperSceneData> WrapperScenes = new Dictionary<string, WrapperSceneData>();
    public static Dictionary<string, SceneData> Scenes = new Dictionary<string, SceneData>();

    // Settings load
    public static SceneManagementData sceneManagementData;

    //Current state
    public static List<string> loadedScenes_Required = new List<string>();
    public static List<string> loadedScenes_Normal = new List<string>();
    public static List<string> loadedScenes_Wrappers = new List<string>();

    //Target Set
    public static List<string> targetScenes_Normal = new List<string>();
    public static List<string> targetScenes_Wrappers = new List<string>();

    //calculated set
    public static List<string> calculatedScenes_ToLoad = new List<string>();
    public static List<string> calculatedScenes_ToUnload = new List<string>();
    public static List<string> calculatedScenes_ToLoad_Wrappers = new List<string>();
    public static List<string> calculatedScenes_ToUnload_Wrappers = new List<string>();

    //transitions
    public static List<string> allowedTransitionList = new List<string>();

    //used only for callbacks
    public static List<SceneTransitionAsync_LoadData> asyncLoadData = new List<SceneTransitionAsync_LoadData>();
    public static List<SceneTransitionAsync_LoadData> asyncUnloadData = new List<SceneTransitionAsync_LoadData>();

    public static List<string> WaitingToLoad = new List<string>();
    public static void WaitAFuckingMinute(string name)
    {
        if (WaitingToLoad.Contains(name))
        {
            Debug.LogError($"Object {name} asked scene controller to wait more than once! Ignoring...");
            return;
        }

        WaitingToLoad.Add(name);
    }

    public static void OkayImGoodNow(string name)
    {
        if (!WaitingToLoad.Contains(name))
        {
            Debug.LogError($"Object {name} was never asked to wait for the scene to load");
            return;
        }
        WaitingToLoad.Remove(name);
    }

    #region Bootstrap
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        var jsonDoc = Resources.Load<TextAsset>("SceneLayoutJSON");
        sceneManagementData = JsonUtility.FromJson<SceneManagementData>(jsonDoc.text);
        RequiredScenes.Clear();
        WrapperScenes.Clear();
        Scenes.Clear();

        RequiredScenes = sceneManagementData.RequiredScenes;

        foreach (var item in sceneManagementData.WrapperScenes)
        {
            WrapperScenes.Add(item.Name, item);
        }
        foreach (var item in sceneManagementData.Scenes)
        {
            Scenes.Add(item.Name, item);
        }

        //add all scenes from the editor as loaded
        //loading scenes this way lets us launch from editor
        //without starting fromt the main scene
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            //first check if it's a required scene
            DebugLog($"[SceneControllerDebug] Loading into scene: {scene.name}");
            if (RequiredScenes.Contains(scene.name))
            {
                //if it is add it here so we don't load it again
                loadedScenes_Required.Add(scene.name);
            }
            else if (WrapperScenes.ContainsKey(scene.name))
            {
                //if it is add it here so we don't load it again
                loadedScenes_Wrappers.Add(scene.name);
            }
            else if (Scenes.ContainsKey(scene.name))
            {
                if (loadedScenes_Normal.Count <= 0)
                {
                    //if it is add it here so we don't load it again
                    loadedScenes_Normal.Add(scene.name);
                }
                else
                {
                    //if it's the second normal scene
                    //we're going to force unload it so we don't break things
                    SceneManager.UnloadSceneAsync(scene.name);
                }
            }
        }

        //then we load all the required scenes since they need to load and never get unloaded
        foreach (var sceneName in RequiredScenes)
        {
            //only load it if we didn't start the editor with the scene
            if (!loadedScenes_Required.Contains(sceneName))
            {
                LoadScene_Required(sceneName);
            }
        }

        //we'll assume if a normal scene is loaded it's the main scene so calculate any missing scenes
        for (int i = 0; i < loadedScenes_Normal.Count; i++)
        {
            if (Scenes.ContainsKey(loadedScenes_Normal[i]))
            {
                CalculateTargetState(loadedScenes_Normal[i]);
            }
        }
        //now that we've gotten our target scene layout
        //we should calculate the changes needed
        CalculateSceneChanges(true);

        //finally we will use the calculated states to load the scenes
        //this is in case the current loaded scene has dependencies

        LoadCalculatedScenes();
    }
    #endregion

    #region Game Functions
    public static void GoToScene(string sceneName)
    {
        CalculateTargetState(sceneName);
        CalculateSceneChanges(false);
        StartHideTransitionMessage?.Dispatch(1.0f, 1.0f, allowedTransitionList, SceneHideComplete, FakeLoadComplete);
    }

    public static void SceneHideComplete()
    {
        //load the calculated scenes
        LoadCalculatedScenes();
    }

    public static async void FakeLoadComplete()
    {
        //wait until we have loaded/unloaded all scenes
        while (asyncUnloadData.Count > 0 && asyncLoadData.Count > 0)
        {
            await Awaitable.NextFrameAsync();
        }

        while (WaitingToLoad.Count > 0)
        {
            await Awaitable.NextFrameAsync();
        }

        //then start showing
        StartShowTransitionMessage?.Dispatch(1.0f, SceneShowComplete);
    }

    public static void SceneShowComplete()
    {
        //We're done!
        //TODO: Add any cleanup stuff here...
    }

    private void OnEnable()
    {

        TransitionMessageHub = Messages.GetHub(TransitionHubName);
        StartHideTransitionMessage = TransitionMessageHub.Get<StartHideTransitionMessage>();
        StartShowTransitionMessage = TransitionMessageHub.Get<StartShowTransitionMessage>();
    }

    private void OnDisable()
    {
        TransitionMessageHub.Return<StartHideTransitionMessage>();
        TransitionMessageHub.Return<StartShowTransitionMessage>();
        Messages.ReturnHub(TransitionHubName);
    }


    #endregion

    #region Scene State Calculation
    private static void CalculateTargetState(string requestedSceneToLoad)
    {
        //clear the target state
        targetScenes_Normal.Clear();
        targetScenes_Wrappers.Clear();

        DebugLog($"Calculating Target State for {requestedSceneToLoad}");
        if (!Scenes.ContainsKey(requestedSceneToLoad))
        {
            DebugLogError($"Scene {requestedSceneToLoad} Does not exist, cannot load wrapper");
            var output = "Scenes that exist: ";
            foreach (var scene in Scenes)
            {
                output += $" {scene.Key} | ";
            }
            DebugLogError($"{output}");
            return;
        }

        var targetScene = Scenes.Where(x => x.Value.Name == requestedSceneToLoad).FirstOrDefault();

        allowedTransitionList = targetScene.Value.AllowedTransitions;


        DebugLog($"Getting target scene: {targetScene}");

        //the normal scene we want is this one
        targetScenes_Normal.Add(targetScene.Value.Name);


        DebugLog($"Starting to get dependencies:");
        //then we add in all the target dependencies
        foreach (var wrapper in targetScene.Value.Dependencies)
        {
            DebugLog($"Loading Dependency: {wrapper}");
            var wrapperData = WrapperScenes[wrapper];
            if (!targetScenes_Wrappers.Contains(wrapper))
            {
                targetScenes_Wrappers.Add(wrapper);
            }
            if (wrapperData.Dependencies.Count > 0)
            {
                DebugLog($"Dependency {wrapper} has {wrapperData.Dependencies.Count} more Dependencies");
                LoadDependencies(wrapperData, ref targetScenes_Wrappers);
            }
        }
    }

    private static void CalculateSceneChanges(bool startup = false)
    {
        //now we're going to compare the target scenes to the existing ones
        calculatedScenes_ToLoad.Clear();
        calculatedScenes_ToLoad_Wrappers.Clear();
        calculatedScenes_ToUnload.Clear();
        calculatedScenes_ToUnload_Wrappers.Clear();

        if (!startup)
        {
            //first we assume we want all our scenes to be loaded
            foreach (var targetScene in targetScenes_Normal)
            {
                if (!calculatedScenes_ToLoad.Contains(targetScene))
                {
                    calculatedScenes_ToLoad.Add(targetScene);
                }
            }

            foreach (var loadedSceneName in loadedScenes_Normal)
            {
                var loadedSceneData = Scenes[loadedSceneName];

                //is this scene already loaded? And are we trying to load it again
                if (calculatedScenes_ToLoad.Contains(loadedSceneName))
                {
                    DebugLogWarning($"Trying to load scene {loadedSceneName} but it's already loaded. " +
                        $"Do we want to Reload? {loadedSceneData.ReloadIfSceneExists}");

                    //check if we want to reload the scene
                    if (loadedSceneData.ReloadIfSceneExists)
                    {
                        DebugLogWarning($"We DO want to reload");
                        //unload and then also reload the scene
                        calculatedScenes_ToUnload.Add(loadedSceneName);
                        calculatedScenes_ToLoad.Add(loadedSceneName);
                    }
                    else
                    {
                        //we are already loaded so do nothing
                        DebugLogWarning($"Nope so remove us from the load list");
                        calculatedScenes_ToLoad.Remove(loadedSceneName);
                    }
                }
                else
                {
                    //we aren't in target scenes so we're unloading
                    DebugLogWarning($"Scene {loadedSceneName} is loaded but we don't want it anymore, Unloading.");
                    calculatedScenes_ToUnload.Add(loadedSceneName);
                }
            }
        }

        foreach (var targetScene in targetScenes_Wrappers)
        {
            if (!calculatedScenes_ToLoad_Wrappers.Contains(targetScene))
            {
                calculatedScenes_ToLoad_Wrappers.Add(targetScene);
            }
        }

        //then check our existing wrappers
        foreach (var loadedSceneName_Wrapper in loadedScenes_Wrappers)
        {
            var loadedSceneData_Wrapper = WrapperScenes[loadedSceneName_Wrapper];
            //first are we trying to load this scene?
            if (calculatedScenes_ToLoad_Wrappers.Contains(loadedSceneName_Wrapper))
            {
                DebugLogWarning($"Trying to load scene {loadedSceneName_Wrapper} but it's already loaded. " +
                    $"Do we want to Reload? {loadedSceneData_Wrapper.ReloadIfSceneExists}");
                //check if we want to reload the scene
                if (loadedSceneData_Wrapper.ReloadIfSceneExists)
                {
                    DebugLogWarning($"We DO want to reload");
                    //unload and then also reload the scene
                    calculatedScenes_ToUnload_Wrappers.Add(loadedSceneName_Wrapper);
                    calculatedScenes_ToLoad_Wrappers.Add(loadedSceneName_Wrapper);
                }
                else
                {
                    //we are already loaded so do nothing
                    DebugLogWarning($"Nope so remove us from the load list");
                    calculatedScenes_ToLoad_Wrappers.Remove(loadedSceneName_Wrapper);
                }
            }
            else
            {
                //we aren't in target scenes so we're unloading
                calculatedScenes_ToUnload_Wrappers.Add(loadedSceneName_Wrapper);
            }
        }
    }

    private static void LoadDependencies(WrapperSceneData wrapperData, ref List<string> dependencyList)
    {
        foreach (var dependency in wrapperData.Dependencies)
        {
            if (!dependencyList.Contains(dependency))
            {
                dependencyList.Add(dependency);
            }
            var nestedWrapper = WrapperScenes[dependency];

            if (nestedWrapper.Dependencies.Count > 0)
            {
                LoadDependencies(nestedWrapper, ref dependencyList);
            }
        }
    }

    private static void LoadCalculatedScenes()
    {
        //first let's unload any scenes that need to be unloaded
        UnloadScenes_Normal(calculatedScenes_ToUnload);
        UnloadScenes_Wrappers(calculatedScenes_ToUnload_Wrappers);

        //then load the scenes that we need to load
        LoadScenes_Normal(calculatedScenes_ToLoad);
        LoadScenes_Wrappers(calculatedScenes_ToLoad_Wrappers);
    }
    #endregion

    #region Load Functions
    private static void LoadScenes_Normal(List<string> scenesToLoad)
    {
        for (int i = 0; i < scenesToLoad.Count; i++)
        {
            LoadScene_Normal(scenesToLoad[i]);
        }
    }
    private static void LoadScenes_Wrappers(List<string> scenesToLoad)
    {
        for (int i = 0; i < scenesToLoad.Count; i++)
        {
            LoadScene_Wrapper(scenesToLoad[i]);
        }
    }

    private static void LoadScene_Normal(string sceneToLoad)
    {
        DebugLogWarning($"LoadScene_Normal {sceneToLoad}");
        var asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        asyncOperation.completed += LoadScene_Normal_Complete;
        asyncLoadData.Add(new SceneTransitionAsync_LoadData()
        {
            sceneName = sceneToLoad,
            asyncOperation = asyncOperation,
            complete = false,
        });
    }
    private static void LoadScene_Wrapper(string sceneToLoad)
    {
        DebugLogWarning($"LoadScene_Wrapper {sceneToLoad}");
        var asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        asyncOperation.completed += LoadScene_Wrappers_Complete;
        asyncLoadData.Add(new SceneTransitionAsync_LoadData()
        {
            sceneName = sceneToLoad,
            asyncOperation = asyncOperation,
            complete = false,
        });
    }
    private static void LoadScene_Required(string sceneToLoad)
    {
        DebugLogWarning($"LoadScene_Required {sceneToLoad}");
        var asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        asyncOperation.completed += LoadScene_Required_Complete;
        asyncLoadData.Add(new SceneTransitionAsync_LoadData()
        {
            sceneName = sceneToLoad,
            asyncOperation = asyncOperation,
            complete = false,
        });
    }
    #endregion

    #region Unload Functions

    private static void UnloadScenes_Normal(List<string> scenesToUnload)
    {
        for (int i = 0; i < scenesToUnload.Count; i++)
        {
            UnloadScene_Normal(scenesToUnload[i]);
        }
    }
    private static void UnloadScenes_Wrappers(List<string> scenesToUnload)
    {
        for (int i = 0; i < scenesToUnload.Count; i++)
        {
            UnloadScene_Wrappers(scenesToUnload[i]);
        }
    }

    //private static void UnloadScenes_Required(List<string> scenesToUnload)
    //{
    //    for (int i = 0; i < scenesToUnload.Count; i++)
    //    {
    //        UnloadScene_Required(scenesToUnload[i]);
    //    }
    //}

    private static void UnloadScene_Normal(string sceneToUnload)
    {
        DebugLog($"Unloading Scene {sceneToUnload}");
        var asyncOperation = SceneManager.UnloadSceneAsync(sceneToUnload);
        if (asyncOperation == null)
        {
            DebugLogError($"Tried to load scene {sceneToUnload} but the asyncOperation is null, likely because scene isn't loaded");
            return;
        }
        asyncOperation.completed += UnloadSceneComplete;
        asyncUnloadData.Add(new SceneTransitionAsync_LoadData()
        {
            sceneName = sceneToUnload,
            asyncOperation = asyncOperation,
            complete = false,
        });
    }
    private static void UnloadScene_Wrappers(string sceneToUnload)
    {
        DebugLog($"Unloading Scene {sceneToUnload}");
        var asyncOperation = SceneManager.UnloadSceneAsync(sceneToUnload);
        asyncOperation.completed += UnloadScene_Wrapper_Complete;
        asyncUnloadData.Add(new SceneTransitionAsync_LoadData()
        {
            sceneName = sceneToUnload,
            asyncOperation = asyncOperation,
            complete = false,
        });
    }
    //private static void UnloadScene_Required(string sceneToUnload)
    //{
    //    DebugLog($"Unloading Scene {sceneToUnload}");
    //    var asyncOperation = SceneManager.UnloadSceneAsync(sceneToUnload);
    //    asyncOperation.completed += UnloadScene_Wrapper_Complete;
    //    asyncUnloadData.Add(new SceneTransitionAsync_LoadData()
    //    {
    //        sceneName = sceneToUnload,
    //        asyncOperation = asyncOperation,
    //        complete = false,
    //    });
    //}
    #endregion

    #region Load Complete Callbacks
    private static void LoadScene_Normal_Complete(AsyncOperation obj)
    {
        for (int i = 0; i < asyncLoadData.Count; i++)
        {
            //PLEASE let this work
            if (asyncLoadData[i].asyncOperation == obj)
            {
                DebugLogWarning($"LoadScene_Normal_Complete for Scene {asyncLoadData[i].sceneName}");
                asyncLoadData[i].complete = true;
                if (!loadedScenes_Normal.Contains(asyncLoadData[i].sceneName))
                {
                    loadedScenes_Normal.Add(asyncLoadData[i].sceneName);
                }
            }
        }
        asyncLoadData = asyncLoadData.Where(x => x.complete == false).ToList();
    }
    private static void LoadScene_Wrappers_Complete(AsyncOperation obj)
    {
        DebugLogWarning($"LoadScene_Wrappers_Complete! asyncLoadData.Count = {asyncLoadData.Count}");
        for (int i = 0; i < asyncLoadData.Count; i++)
        {
            DebugLogWarning($"LoadScene_Wrappers_Complete Comparing {asyncLoadData[i].sceneName} to obj");
            if (asyncLoadData[i].asyncOperation == obj)
            {
                DebugLogWarning($"LoadScene_Wrappers_Complete for Scene {asyncLoadData[i].sceneName}");
                asyncLoadData[i].complete = true;
                if (!loadedScenes_Wrappers.Contains(asyncLoadData[i].sceneName))
                {
                    loadedScenes_Wrappers.Add(asyncLoadData[i].sceneName);
                }
            }
        }
        asyncLoadData = asyncLoadData.Where(x => x.complete == false).ToList();
    }
    private static void LoadScene_Required_Complete(AsyncOperation obj)
    {
        for (int i = 0; i < asyncLoadData.Count; i++)
        {
            if (asyncLoadData[i].asyncOperation == obj)
            {
                DebugLogWarning($"LoadScene_Required_Complete for Scene {asyncLoadData[i].sceneName}");
                asyncLoadData[i].complete = true;
                if (!loadedScenes_Required.Contains(asyncLoadData[i].sceneName))
                {
                    loadedScenes_Required.Add(asyncLoadData[i].sceneName);
                }
            }
        }
        //remove completed scenes from list
        asyncLoadData = asyncLoadData.Where(x => x.complete == false).ToList();
    }
    #endregion

    #region Unload Complete Callbacks
    private static void UnloadSceneComplete(AsyncOperation obj)
    {
        for (int i = 0; i < asyncUnloadData.Count; i++)
        {
            if (asyncUnloadData[i].asyncOperation == obj)
            {
                DebugLogWarning($"Unload Complete for Scene {asyncUnloadData[i].sceneName}");
                asyncUnloadData[i].complete = true;

                if (loadedScenes_Normal.Contains(asyncUnloadData[i].sceneName))
                {
                    loadedScenes_Normal.Remove(asyncUnloadData[i].sceneName);
                }
            }
        }
        asyncUnloadData = asyncUnloadData.Where(x => x.complete == false).ToList();
    }
    private static void UnloadScene_Wrapper_Complete(AsyncOperation obj)
    {
        for (int i = 0; i < asyncUnloadData.Count; i++)
        {
            if (asyncUnloadData[i].asyncOperation == obj)
            {
                DebugLogWarning($"Unload Complete for Scene {asyncUnloadData[i].sceneName}");
                asyncUnloadData[i].complete = true;

                if (loadedScenes_Wrappers.Contains(asyncUnloadData[i].sceneName))
                {
                    loadedScenes_Wrappers.Remove(asyncUnloadData[i].sceneName);
                }
            }
        }
        asyncUnloadData = asyncUnloadData.Where(x => x.complete == false).ToList();
    }
    //private static void UnloadScene_Required_Complete(AsyncOperation obj)
    //{
    //    for (int i = 0; i < asyncUnloadData.Count; i++)
    //    {
    //        if (asyncUnloadData[i].asyncOperation == obj)
    //        {
    //            DebugLogWarning($"Unload Complete for Scene {asyncUnloadData[i].sceneName}");
    //            asyncUnloadData[i].complete = true;

    //            if (loadedScenes_Required.Contains(asyncUnloadData[i].sceneName))
    //            {
    //                loadedScenes_Required.Remove(asyncUnloadData[i].sceneName);
    //            }
    //        }
    //    }
    //    asyncUnloadData = asyncUnloadData.Where(x => x.complete == false).ToList();
    //}
    #endregion

    #region Debug
    private static void DebugLog(string log, GameObject target = null)
    {
        if (sceneManagementData.showConsoleMessages)
        {
            Debug.Log(log, target);
        }
    }
    private static void DebugLogWarning(string log, GameObject target = null)
    {
        if (sceneManagementData.showConsoleMessages)
        {
            Debug.LogWarning(log, target);
        }
    }
    private static void DebugLogError(string log, GameObject target = null)
    {
        if (sceneManagementData.showConsoleMessages)
        {
            Debug.LogError(log, target);
        }
    }
    #endregion

#if UNITY_EDITOR

    [MenuItem("Assets/Create/Snowdrama/Transitions/Create Scene Controller JSON")]
    public static void CreateSceneJSON()
    {
        SceneManagementData defaultData = new SceneManagementData()
        {
            showConsoleMessages = true,
            DefaultSceneName = "MainMenuScene",
            RequiredScenes = new List<string>()
            {
                "RequiredScene",
            },
            WrapperScenes = new List<WrapperSceneData>()
            {
                new WrapperSceneData()
                {
                    Name = "SaveGameScene",
                    ReloadIfSceneExists = false,
                    Dependencies = new List<string>(){}
                },
                new WrapperSceneData()
                {
                    Name = "GameDataScene",
                    ReloadIfSceneExists = false,
                    Dependencies = new List<string>()
                    {
                        "SaveGameScene",
                    }
                }
            },
            Scenes = new List<SceneData>()
            {
                new SceneData()
                {
                    Name = "MainMenuScene",
                    ReloadIfSceneExists = true,
                    Dependencies = new List<string>()
                    {
                        "SaveGameScene",
                    },
                    AllowedTransitions = new List<string>(),
                    transitionTime = 0.5f,
                    transitionFakeLoadTime = 0.5f,

                },
                new SceneData()
                {
                    Name = "Game",
                    ReloadIfSceneExists = true,
                    Dependencies = new List<string>()
                    {
                        "SaveGameScene",
                        "GameDataScene",
                    },
                    AllowedTransitions = new List<string>(),
                    transitionTime = 0.5f,
                    transitionFakeLoadTime = 0.5f,
                },
            },
        };

        var dataString = JsonUtility.ToJson(defaultData, true);

        var selected = Selection.activeObject;
        Debug.Log(selected);
        var path = AssetDatabase.GetAssetPath(selected);
        Debug.Log(path);

        File.WriteAllText($"{path}/SceneLayoutJSON.json", dataString);
        AssetDatabase.Refresh();
    }
#endif
}

[System.Serializable]
public struct SceneManagementData
{
    public bool showConsoleMessages;
    public string DefaultSceneName;
    public List<string> RequiredScenes;
    public List<WrapperSceneData> WrapperScenes;
    public List<SceneData> Scenes;
}

[System.Serializable]
public struct SceneData
{
    public string Name;
    public bool ReloadIfSceneExists;
    public List<string> Dependencies;
    public List<string> AllowedTransitions;
    public float transitionTime;
    public float transitionFakeLoadTime;
}

[System.Serializable]
public struct WrapperSceneData
{
    public string Name;
    public bool ReloadIfSceneExists;
    public List<string> Dependencies;
}