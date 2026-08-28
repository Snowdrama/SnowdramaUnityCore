using Newtonsoft.Json;
using Snowdrama.Transition;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    //transition related
    private const string TransitionHubName = "TransitionDriver"; //this is the default name
    private static MessageHub TransitionMessageHub;
    private static TransitionReqestedMessage TransitionReqestedMessage;
    private static StartHideTransitionMessage StartHideTransitionMessage;
    private static StartShowTransitionMessage StartShowTransitionMessage;
    private static TransitionCompleteMessage TransitionCompleteMessage;

    public static List<string> RequiredScenes = new();
    public static Dictionary<string, WrapperSceneData> WrapperScenes = new();
    public static Dictionary<string, SceneData> Scenes = new();

    // Settings load
    public static SceneManagementData sceneManagementData;

    //Current state
    public static List<string> loadedScenes_Required = new();
    public static List<string> loadedScenes_Wrappers = new();
    public static string loadedScene_Current = "";

    //Target Set
    public static List<string> targetScenes_Wrappers = new();
    public static string targetScene_Next = "";

    //calculated set
    public static List<string> calculatedScenes_ToLoad = new();
    public static List<string> calculatedScenes_ToUnload = new();
    public static List<string> calculatedScenes_ToLoad_Wrappers = new();
    public static List<string> calculatedScenes_ToUnload_Wrappers = new();

    //transitions
    public static List<string> allowedTransitionList = new();

    //used only for callbacks
    public static List<SceneTransitionAsync_LoadData> asyncLoadData = new();
    public static List<SceneTransitionAsync_LoadData> asyncUnloadData = new();

    public static List<string> WaitingToLoad = new();
    public static void WaitAFuckingMinute(string name)
    {
        if (WaitingToLoad.Contains(name))
        {
            DebugLogError($"Object {name} asked scene controller to wait more than once! Ignoring...");
            return;
        }

        WaitingToLoad.Add(name);
    }

    public static void OkayImGoodNow(string name)
    {
        if (!WaitingToLoad.Contains(name))
        {
            DebugLogError($"Object {name} was never asked to wait for the scene to load");
            return;
        }
        WaitingToLoad.Remove(name);
    }

    #region Bootstrap
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        var jsonDoc = Resources.Load<TextAsset>("SceneLayoutJSON");

        if (jsonDoc == null)
        {
            Debug.LogError("Can't find SceneControllerJSON. " +
                "Please use Create -> Snowdrama -> Transitions -> Create Scene ControllerJson " +
                "to create one in the Resources folder");
            return;
        }

        sceneManagementData = JsonConvert.DeserializeObject<SceneManagementData>(jsonDoc.text);

        DebugLog($"Loading Scene Management Data, ShowConsoleMessages? -> {sceneManagementData.ShowConsoleMessages}");

        DebugLog(jsonDoc.text);

        RequiredScenes.Clear();
        WrapperScenes.Clear();
        Scenes.Clear();

        loadedScenes_Required.Clear();
        loadedScenes_Wrappers.Clear();
        loadedScene_Current = null;

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
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            DebugLog($"[SceneControllerDebug] Grabbing Editor Scene: {scene.name}");

            //first check if it's a required scene
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
                if (string.IsNullOrEmpty(loadedScene_Current))
                {
                    //if it is add it here so we don't load it again
                    loadedScene_Current = scene.name;
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
        if (!string.IsNullOrEmpty(loadedScene_Current) && Scenes.ContainsKey(loadedScene_Current))
        {
            CalculateTargetState(loadedScene_Current);
        }
        //now that we've gotten our target scene layout
        //we should calculate the changes needed
        CalculateSceneChanges(true);

        //finally we will use the calculated states to load the scenes
        //this is in case the current loaded scene has dependencies
        LoadCalculatedScenes();
    }
    #endregion

    private static bool IsTransitioning = false;
    #region Game Functions
    public static void GoToScene(string sceneName)
    {
        if (!Scenes.ContainsKey(sceneName))
        {
            DebugLogError($"Tried to load a scene named {sceneName} that's not in the Scenes List. Check the SceneLayout JSON");
            return;
        }

        //don't do something while transitioning
        if (IsTransitioning) { return; }

        TransitionReqestedMessage?.Dispatch(sceneName);

        //now we're starting the transition
        IsTransitioning = true;

        CalculateTargetState(sceneName);
        CalculateSceneChanges(false);
        StartHideTransitionMessage?.Dispatch(1.0f, 1.0f, allowedTransitionList, SceneHideComplete, FakeLoadComplete);
    }

    public static string GetCurrentMainScene()
    {
        return loadedScene_Current;
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
        IsTransitioning = false;
        TransitionCompleteMessage?.Dispatch();
    }

    private void OnEnable()
    {

        TransitionMessageHub = Messages.GetHub(TransitionHubName);
        TransitionReqestedMessage = TransitionMessageHub.Get<TransitionReqestedMessage>();
        StartHideTransitionMessage = TransitionMessageHub.Get<StartHideTransitionMessage>();
        StartShowTransitionMessage = TransitionMessageHub.Get<StartShowTransitionMessage>();
        TransitionCompleteMessage = TransitionMessageHub.Get<TransitionCompleteMessage>();
    }

    private void OnDisable()
    {
        TransitionMessageHub.Return<TransitionReqestedMessage>();
        TransitionMessageHub.Return<StartHideTransitionMessage>();
        TransitionMessageHub.Return<StartShowTransitionMessage>();
        TransitionMessageHub.Return<TransitionCompleteMessage>();
        Messages.ReturnHub(TransitionHubName);
    }


    #endregion

    #region Scene State Calculation
    private static void CalculateTargetState(string requestedSceneToLoad)
    {
        //clear the target state
        targetScene_Next = null;
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
        targetScene_Next = targetScene.Value.Name;


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
            if (!calculatedScenes_ToLoad.Contains(targetScene_Next))
            {
                calculatedScenes_ToLoad.Add(targetScene_Next);
            }

            DebugLog($"Scenes: {Scenes} ");
            DebugLog($"Scenes Count: {Scenes.Count}");
            DebugLog($"Scenes Count: {loadedScene_Current}");

            var loadedSceneData = Scenes[loadedScene_Current];

            //is this scene already loaded? And are we trying to load it again
            if (calculatedScenes_ToLoad.Contains(loadedSceneData.Name))
            {
                DebugLog($"Trying to load scene {loadedSceneData.Name} but it's already loaded. " +
                    $"Do we want to Reload? {loadedSceneData.ReloadIfSceneExists}");

                //check if we want to reload the scene
                if (loadedSceneData.ReloadIfSceneExists)
                {
                    DebugLog($"We DO want to reload");
                    //unload because we also want to unload and reload
                    //and we're already in the calculatedScenes_ToLoad
                    calculatedScenes_ToUnload.Add(loadedSceneData.Name);

                    //DON'T DO THIS: it will mean loading a second one!
                    //DON'T DO THIS: it will mean loading a second one!
                    //calculatedScenes_ToLoad.Add(loadedSceneData.Name);
                }
                else
                {
                    //we are already loaded so do nothing
                    DebugLog($"Nope so remove us from the load list");
                    calculatedScenes_ToLoad.Remove(loadedSceneData.Name);
                }
            }
            else
            {
                //we aren't in target scenes so we're unloading
                DebugLog($"Scene {loadedSceneData.Name} is loaded but we don't want it anymore, Unloading.");
                calculatedScenes_ToUnload.Add(loadedSceneData.Name);
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
                DebugLog($"Trying to load scene {loadedSceneName_Wrapper} but it's already loaded. " +
                    $"Do we want to Reload? {loadedSceneData_Wrapper.ReloadIfSceneExists}");
                //check if we want to reload the scene
                if (loadedSceneData_Wrapper.ReloadIfSceneExists)
                {
                    DebugLog($"We DO want to reload");
                    //unload and then also reload the scene
                    calculatedScenes_ToUnload_Wrappers.Add(loadedSceneName_Wrapper);
                    calculatedScenes_ToLoad_Wrappers.Add(loadedSceneName_Wrapper);
                }
                else
                {
                    //we are already loaded so do nothing
                    DebugLog($"Nope so remove us from the load list");
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
        foreach (var item in calculatedScenes_ToUnload)
        {
            DebugLog($"<color=orange>Unloading: {item}");
        }
        foreach (var item in calculatedScenes_ToUnload_Wrappers)
        {
            DebugLog($"<color=orange>Unloading Wrapper: {item}");
        }
        foreach (var item in calculatedScenes_ToLoad)
        {
            DebugLog($"<color=green>Loading: {item}");
        }
        foreach (var item in calculatedScenes_ToLoad_Wrappers)
        {
            DebugLog($"<color=green>Loading Wrapper: {item}");
        }

        //first let's unload any scenes that need to be 
        UnloadScenes_Normal(calculatedScenes_ToUnload);
        UnloadScenes_Wrappers(calculatedScenes_ToUnload_Wrappers);

        //then load the scenes that we need to load
        LoadScenes_Wrappers(calculatedScenes_ToLoad_Wrappers);
        LoadScenes_Normal(calculatedScenes_ToLoad);
    }
    #endregion

    #region Load Functions
    private static void LoadScenes_Normal(List<string> scenesToLoad)
    {
        DebugLog($"<color=cyan>Number of Scenes to Load: {scenesToLoad.Count}");

        foreach (var scene in scenesToLoad) { DebugLog($"<color=orange>{scene}</color>"); }


        for (var i = 0; i < scenesToLoad.Count; i++)
        {
            DebugLog($"Loading Scene: {scenesToLoad[i]}");
            LoadScene_Normal(scenesToLoad[i]);
        }
    }
    private static void LoadScenes_Wrappers(List<string> scenesToLoad)
    {
        DebugLog($"<color=cyan>Number of Wrapper Scenes to Load: {scenesToLoad.Count}");

        foreach (var scene in scenesToLoad) { DebugLog($"<color=orange>{scene}</color>"); }

        for (var i = 0; i < scenesToLoad.Count; i++)
        {
            LoadScene_Wrapper(scenesToLoad[i]);
        }
    }

    private static void LoadScene_Normal(string sceneToLoad)
    {
        DebugLog($"<color=cyan>LoadScene_Normal {sceneToLoad}");
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
        DebugLog($"<color=cyan>LoadScene_Wrapper {sceneToLoad}");
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
        DebugLog($"<color=cyan>LoadScene_Required {sceneToLoad}");
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
        DebugLog($"<color=magenta>Number of Scenes to Unload: {scenesToUnload.Count}");

        foreach (var scene in scenesToUnload) { DebugLog($"<color=orange>{scene}</color>"); }

        for (var i = 0; i < scenesToUnload.Count; i++)
        {
            UnloadScene_Normal(scenesToUnload[i]);
        }
    }
    private static void UnloadScenes_Wrappers(List<string> wrappersToUnload)
    {
        DebugLog($"<color=magenta>Number of Wrapper Scenes to Unload: {wrappersToUnload.Count}");

        foreach (var scene in wrappersToUnload) { DebugLog($"<color=orange>{scene}</color>"); }

        for (var i = 0; i < wrappersToUnload.Count; i++)
        {
            UnloadScene_Wrappers(wrappersToUnload[i]);
        }
    }

    private static void UnloadScene_Normal(string sceneToUnload)
    {
        DebugLog($"Unloading Scene {sceneToUnload}");
        var asyncOperation = SceneManager.UnloadSceneAsync(sceneToUnload);
        if (asyncOperation == null)
        {
            DebugLogError($"Tried to unload scene {sceneToUnload} but the asyncOperation is null, " +
                $"likely because scene isn't loaded");
            return;
        }
        asyncOperation.completed += UnloadScene_Normal_Complete;
        asyncUnloadData.Add(new SceneTransitionAsync_LoadData()
        {
            sceneName = sceneToUnload,
            asyncOperation = asyncOperation,
            complete = false,
        });
    }
    private static void UnloadScene_Wrappers(string sceneToUnload)
    {
        var scene = SceneManager.GetSceneByName(sceneToUnload);
        var asyncOperation = SceneManager.UnloadSceneAsync(sceneToUnload);
        if (asyncOperation == null)
        {
            DebugLogError($"Tried to unload a wrapper scene {sceneToUnload} but the asyncOperation is null, " +
                $"likely because scene isn't loaded." +
                $"Note that you can not start the editor with 'Wrapper' scenes due to initial unloading." +
                $"Please remove any wrapper scenes from the hierarchy before starting the editor!");
            return;
        }
        asyncOperation.completed += UnloadScene_Wrapper_Complete;
        asyncUnloadData.Add(new SceneTransitionAsync_LoadData()
        {
            sceneName = sceneToUnload,
            asyncOperation = asyncOperation,
            complete = false,
        });
        DebugLog($"Starting unload of: {sceneToUnload} progress: {asyncOperation.progress}");
    }
    #endregion

    #region Load Complete Callbacks
    private static void LoadScene_Normal_Complete(AsyncOperation obj)
    {
        DebugLog($"<color=yellow>[LoadScene_Normal_Complete]: Async Load Complete Checking Status... Count:{asyncLoadData.Count}");
        for (var i = 0; i < asyncLoadData.Count; i++)
        {
            DebugLog($"<color=yellow>[LoadScene_Normal_Complete]: Checking complete status for: {asyncLoadData[i].sceneName}");
            if (asyncLoadData[i].asyncOperation == obj)
            {
                DebugLog($"<color=green>[LoadScene_Normal_Complete]: Load Compltete {asyncLoadData[i].sceneName}");
                asyncLoadData[i].complete = true;
                if (loadedScene_Current.Trim().ToLower() != asyncLoadData[i].sceneName.Trim().ToLower())
                {
                    loadedScene_Current = asyncLoadData[i].sceneName;
                }
            }
        }
        asyncLoadData = asyncLoadData.Where(x => x.complete == false).ToList();
    }
    private static void LoadScene_Wrappers_Complete(AsyncOperation obj)
    {
        DebugLog($"<color=yellow>[LoadScene_Wrappers_Complete]: Async Load Wrapper Complete Checking Status... Count:{asyncLoadData.Count}");
        for (var i = 0; i < asyncLoadData.Count; i++)
        {
            DebugLog($"<color=yellow>[LoadScene_Wrappers_Complete]: Checking complete status for: {asyncLoadData[i].sceneName}");
            if (asyncLoadData[i].asyncOperation == obj)
            {
                DebugLog($"<color=green>[LoadScene_Wrappers_Complete]: Load Wrapper Compltete: {asyncLoadData[i].sceneName}");
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
        DebugLog($"<color=yellow>[LoadScene_Required_Complete]: Async Load Required Complete Checking Status... Count:{asyncLoadData.Count}");
        for (var i = 0; i < asyncLoadData.Count; i++)
        {
            DebugLog($"<color=yellow>[LoadScene_Required_Complete]: Checking complete status for: {asyncLoadData[i].sceneName}");
            if (asyncLoadData[i].asyncOperation == obj)
            {
                DebugLog($"<color=green>[LoadScene_Required_Complete]: Load Complete: {asyncLoadData[i].sceneName}");
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
    private static void UnloadScene_Normal_Complete(AsyncOperation obj)
    {
        DebugLog($"<color=yellow>[UnloadScene_Normal_Complete]: Async Unload Complete Checking Status... Count:{asyncUnloadData.Count}");
        for (var i = 0; i < asyncUnloadData.Count; i++)
        {
            DebugLog($"<color=yellow>[UnloadScene_Normal_Complete]: Checking complete status for: {asyncUnloadData[i].sceneName}");
            if (asyncUnloadData[i].asyncOperation == obj)
            {
                DebugLog($"<color=green>[UnloadScene_Normal_Complete]Unload Complete for Scene {asyncUnloadData[i].sceneName}");
                asyncUnloadData[i].complete = true;

                if (loadedScene_Current == asyncUnloadData[i].sceneName)
                {
                    loadedScene_Current = asyncUnloadData[i].sceneName;
                }
            }
        }
        asyncUnloadData = asyncUnloadData.Where(x => x.complete == false).ToList();
    }
    private static void UnloadScene_Wrapper_Complete(AsyncOperation obj)
    {
        DebugLog($"<color=yellow>[UnloadScene_Wrapper_Complete]: Async Unload Complete Checking Status");
        for (var i = 0; i < asyncUnloadData.Count; i++)
        {
            DebugLog($"<color=yellow>[UnloadScene_Wrapper_Complete]: Checking complete status for: {asyncUnloadData[i].sceneName}");
            if (asyncUnloadData[i].asyncOperation == obj)
            {
                DebugLog($"<color=green>[UnloadScene_Wrapper_Complete]: Unload Complete! for Scene {asyncUnloadData[i].sceneName}");
                asyncUnloadData[i].complete = true;

                if (loadedScenes_Wrappers.Contains(asyncUnloadData[i].sceneName))
                {
                    loadedScenes_Wrappers.Remove(asyncUnloadData[i].sceneName);
                }
            }
        }
        asyncUnloadData = asyncUnloadData.Where(x => x.complete == false).ToList();
    }
    #endregion

    #region Debug
    private static void DebugLog(string log, GameObject target = null)
    {
        if (sceneManagementData.ShowConsoleMessages)
        {
            Debug.Log(log, target);
        }
    }
    private static void DebugLogWarning(string log, GameObject target = null)
    {
        if (sceneManagementData.ShowConsoleMessages)
        {
            Debug.LogWarning(log, target);
        }
    }
    private static void DebugLogError(string log, GameObject target = null)
    {
        if (sceneManagementData.ShowConsoleMessages)
        {
            Debug.LogError(log, target);
        }
    }
    #endregion

#if UNITY_EDITOR

    [MenuItem("Snowdrama/Required/Create Scene Controller JSON")]
    public static void CreateSceneJSON()
    {
        SceneManagementData defaultData = new()
        {
            ShowConsoleMessages = true,
            DefaultSceneName = "MainMenuScene",
            RequiredScenes = new()
            {
                "RequiredScene",
            },
            WrapperScenes = new()
            {
                new()
                {
                    Name = "PauseMenuScene",
                    ReloadIfSceneExists = false,
                    Dependencies = new(){}
                }
            },
            Scenes = new()
            {
                new()
                {
                    Name = "MainMenuScene",
                    ReloadIfSceneExists = true,
                    Dependencies = new(),
                    AllowedTransitions = new(),
                    transitionTime = 0.5f,
                    transitionFakeLoadTime = 0.5f,
                },
                new()
                {
                    Name = "Game",
                    ReloadIfSceneExists = true,
                    Dependencies = new()
                    {
                        "PauseMenuScene",
                    },
                    AllowedTransitions = new(),
                    transitionTime = 0.5f,
                    transitionFakeLoadTime = 0.5f,
                },
                new()
                {
                    Name = "WinScene",
                    ReloadIfSceneExists = true,
                    Dependencies = new(),
                    AllowedTransitions = new(),
                    transitionTime = 0.5f,
                    transitionFakeLoadTime = 0.5f,
                },
                new()
                {
                    Name = "LoseScene",
                    ReloadIfSceneExists = true,
                    Dependencies = new(),
                    AllowedTransitions = new(),
                    transitionTime = 0.5f,
                    transitionFakeLoadTime = 0.5f,
                },
            },
        };
        var dataString = JsonConvert.SerializeObject(defaultData, new JsonSerializerSettings() { Formatting = Formatting.Indented });
        if (!File.Exists($"Assets/Resources/SceneLayoutJSON.jsonc"))
        {
            File.WriteAllText($"Assets/Resources/SceneLayoutJSON.jsonc", dataString);
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError("DANGER! ENSURE YOU ACTUALLY WANT TO DO THIS!!! " +
                "Can't overwrite SceneLayoutJSON.jsonc because it already exists! " +
                "Overwriting this would delete any scene configuration you have! " +
                "Check the SceneLayoutJSON.json file and ensure you actually want to delete it! " +
                "If this ACTUALLY intended please manually delete the SceneLayoutJSON.jsonc and run again. ");
        }
    }
#endif
}

[System.Serializable]
public struct SceneManagementData
{
    public bool ShowConsoleMessages;
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