using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
//using System.IO;
using System.Linq;
using UnityEngine;
using QFSW.QC;

//using QFSW.QC;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class SaveDataStruct
{
    public int CurrentSaveIndex = 0;
    public int CurrentAutoSaveIndex = 0;
    public Dictionary<int, SaveGameInfo> SaveLocations = new();
    public Dictionary<int, SaveGameInfo> AutoSaveLocations = new();
}

[System.Serializable]
public class SaveGameInfo
{
    public int SaveSlot;
    public string Name;
    public string Version;
    public string DateModified;
    public string FilePath;
    public string ImagePath;
    public bool IsAutoSave;
}

//This is used for updating the save list
//when you save or delete a game save
public class SaveGameListChangedMessage : AMessage { }

//this is triggered when the save is loaded to tell the game to trigger
//something in response to the save
public class SaveGameLoadedMessage : AMessage<GameData> { }

//this gets triggered right before the game serializes
//good to listen for this and have all objects write their data here
public class SaveGameStartingSaveMessage : AMessage { }

//this is called right after a game saved, useful for triggering things like "Save and Quit"
public class SaveGameSavingCompletedMessage : AMessage { }


public class SaveManager : MonoBehaviour
{
    private class SaveManagerSettings
    {
        public bool ShowConsoleMessages = true;
        public int AutoSaveSlotCount = 4;
        public string AutoSaveNameFormat = "Auto Save [VALUE]";
    }
    private static SaveManagerSettings saveManagerSettings;
    private static int currentSaveSlot = -1;
    private static GameData loadedSave = new();
    private static SaveDataStruct saveDataInfo = new();
    /// <summary>
    /// Formating should be intented to make it easier to read
    /// 
    /// Reference loops... Ignore by default because in THEORY you should't do it
    /// </summary>
    private static readonly JsonSerializerSettings settings = new() { Formatting = Formatting.Indented, ReferenceLoopHandling = ReferenceLoopHandling.Ignore, };
    private static string saveDataInfoPath = $"{Application.persistentDataPath}/save_data_info.json";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Bootstrap()
    {
        ValidateDirectories();
        NewGame();

        var SaveManagerSettingsTextAsset = Resources.Load<TextAsset>("SaveManagerSettings");
        if (SaveManagerSettingsTextAsset == null)
        {
            Debug.LogError($"Ensure you create a SaveManagerSettings.jsonc file in the Resources folder! Using default which may be broken!");
            saveManagerSettings = new SaveManagerSettings();
        }
        else
        {
            saveManagerSettings = JsonConvert.DeserializeObject<SaveManagerSettings>(SaveManagerSettingsTextAsset.text);
        }

        InitSaveDataInfo();

#if UNITY_EDITOR
        //load save 0 by default in case we're testing
        //then we can load into any scene with the save 0 data
        if (saveDataInfo.SaveLocations.Count == 0)
        {
            //force saving a new game if there's no slot 0
            NewGame();
            //Debug.Log($"Force Saving Save 0, Modify the save there for testing");
            //Debug.Log($"SavePath: {Application.persistentDataPath}");
            SaveGame(0, loadedSave, true);
        }
        else
        {
            //Debug.Log($"Loading save 0 in editor for testing: {Application.persistentDataPath}");
            LoadSave(0, false, false);
        }
#endif
    }
    public static void NewGame(bool autoLoadScene = true)
    {
        currentSaveSlot = -1;
        //Debug.Log($"Starting new game, loading default data!");
        //load the default save from resources:
        var defaultSaveJson = Resources.Load<TextAsset>("DefaultSave");
        if (defaultSaveJson != null)
        {
            //Debug.Log($"Loaded Default Save!: {defaultSaveJson.text}");
            loadedSave = JsonConvert.DeserializeObject<GameData>(defaultSaveJson.text, settings);
            GameDataManager.SetLoadedSave(loadedSave);
            if (autoLoadScene == true && !string.IsNullOrEmpty(loadedSave.SceneToLoadOnLoad))
            {
                //Debug.Log($"Let's load the scene! autoLoadScene: {autoLoadScene}: {loadedSave.SceneToLoadOnLoad}");
                SceneController.GoToScene(loadedSave.SceneToLoadOnLoad);
            }
        }
        else
        {
            Debug.LogError($"No Default save! " +
                $"Make sure you have a DefaultSave.jsonc in the Resources folder!");
        }
    }
    public static void LoadSaveScene()
    {
        if (loadedSave != null)
        {
            SceneController.GoToScene(loadedSave.SceneToLoadOnLoad);
        }
    }

    private static void InitSaveDataInfo()
    {
        if (FileHelper.Exists(saveDataInfoPath))
        {
            var saveDataInfoJson = FileHelper.ReadAllText(saveDataInfoPath);
            if (string.IsNullOrWhiteSpace(saveDataInfoJson))
            {
                Debug.Log($"Creating New SaveDataInfo");
                saveDataInfo = new SaveDataStruct();
                return;
            }
            saveDataInfo = JsonConvert.DeserializeObject<SaveDataStruct>(saveDataInfoJson, settings);

            if (saveDataInfo == null)
            {
                Debug.Log($"Creating New SaveDataInfo");
                saveDataInfo = new SaveDataStruct();
                return;
            }
        }
        else
        {
            Debug.Log($"Creating New SaveDataInfo");
            saveDataInfo = new SaveDataStruct();
            return;
        }
    }
    public static SaveDataStruct GetSaveList()
    {
        if (saveDataInfo == null)
        {
            InitSaveDataInfo();
        }

        return saveDataInfo;
    }
    #region Load Game
    public static bool CanLoadSave(int saveSlot, bool autoSave)
    {
        //Debug.Log($"Loading an Auto Save: {autoSave}");
        if ((!autoSave && saveDataInfo.SaveLocations == null) ||
            (autoSave && saveDataInfo.AutoSaveLocations == null))
        {
            Debug.LogError("Somehow save locations is null");
            return false;
        }
        if ((!autoSave && saveDataInfo.SaveLocations.Values.Count == 0) ||
            (autoSave && saveDataInfo.AutoSaveLocations.Values.Count == 0))
        {
            Debug.LogError($"No saves found, " +
                $"AutoSave.Count: {saveDataInfo.SaveLocations.Values.Count} " +
                $"SaveCount: {saveDataInfo.SaveLocations.Values.Count}");
            return false;
        }

        if ((!autoSave && !saveDataInfo.SaveLocations.ContainsKey(saveSlot)) ||
            (autoSave && !saveDataInfo.AutoSaveLocations.ContainsKey(saveSlot)))
        {
            Debug.LogError($"Failed to load, IsAutoSave: {autoSave} SaveSlot: {saveSlot}");
            return false;
        }

        string expectedPath;
        //can we find the save?
        if (autoSave)
        {
            expectedPath = saveDataInfo.AutoSaveLocations[saveSlot].FilePath;
        }
        else
        {
            expectedPath = saveDataInfo.SaveLocations[saveSlot].FilePath;
        }

        if (FileHelper.Exists(expectedPath))
        {
            return true;
        }
        return false;
    }
    public static bool LoadSave(int saveSlot, bool isAutoSave, bool autoLoadScene = true)
    {
        ValidateDirectories();
        //Debug.Log($"Loading an Save: {isAutoSave}");
        if ((!isAutoSave && saveDataInfo.SaveLocations == null) ||
            (isAutoSave && saveDataInfo.AutoSaveLocations == null))
        {
            Debug.LogError("Somehow save locations is null");
            return false;
        }
        if ((!isAutoSave && saveDataInfo.SaveLocations.Values.Count == 0) ||
            (isAutoSave && saveDataInfo.AutoSaveLocations.Values.Count == 0))
        {
            Debug.LogError("No saves found");
            return false;
        }

        if ((!isAutoSave && !saveDataInfo.SaveLocations.ContainsKey(saveSlot)) ||
            (isAutoSave && !saveDataInfo.AutoSaveLocations.ContainsKey(saveSlot)))
        {
            Debug.LogError("Tried to load an save slot that doesn't exist");
            return false;
        }

        //set the current save slot so we know which was the last uses
        currentSaveSlot = saveSlot;

        string expectedPath;
        if (isAutoSave)
        {
            expectedPath = saveDataInfo.AutoSaveLocations[saveSlot].FilePath;

            //if we're loading an auto save, we do this so it
            saveDataInfo.CurrentAutoSaveIndex = saveSlot;
        }
        else
        {
            expectedPath = saveDataInfo.SaveLocations[saveSlot].FilePath;
        }
        //Debug.Log($"Loading Save From: {expectedPath}");


        if (FileHelper.Exists(expectedPath))
        {
            var savePathToLoad = expectedPath;
            var fileContents = FileHelper.ReadAllText(savePathToLoad);

            //Debug.Log($"Loading file from {savePathToLoad}");
            //Debug.Log(fileContents);
            loadedSave = JsonConvert.DeserializeObject<GameData>(fileContents, settings);
            GameDataManager.SetLoadedSave(loadedSave);
            Debug.Log($"<color=#0FF>Dispatching the SaveGameLoadedMessage!!!</color>");
            Messages.GetOnce<SaveGameLoadedMessage>().Dispatch(loadedSave);
            Debug.Log($"<color=#0FF>Post SaveGameLoaded Message!!!</color>");

            if (autoLoadScene == true && !string.IsNullOrEmpty(loadedSave.SceneToLoadOnLoad))
            {
                Debug.Log($"Let's load the scene! autoLoadScene: {autoLoadScene}: {loadedSave.SceneToLoadOnLoad}");
                SceneController.GoToScene(loadedSave.SceneToLoadOnLoad);
            }

            SaveInfoFile();
            Messages.GetOnce<SaveGameListChangedMessage>().Dispatch();
            return true;
        }

        return false;
    }

    public static void LoadSave(int slot)
    {
        LoadSave(slot, false);
    }

    public static void LoadAutoSave(int slot)
    {
        LoadSave(slot, true);
    }


    #endregion
    public static int GetUnusedSaveSlot()
    {
        var index = 0;
        //This assumes that there's no way the player
        //has int.MaxValue number of saves...
        //we will EVENTUALLY find one they don't use
        while (saveDataInfo.SaveLocations.ContainsKey(index))
        {
            index++;
        }
        return index;
    }

    public static int GetAutoSaveToUse()
    {
        var index = 0;
        //This assumes that there's no way the player
        //has int.MaxValue number of saves...
        //we will EVENTUALLY find one they don't use
        while (saveDataInfo.AutoSaveLocations.ContainsKey(index))
        {
            index++;

            //if we reach the max count we didn't find any empty slot
            if (index == saveManagerSettings.AutoSaveSlotCount)
            {
                break;
            }

            //hard cap to prevent this stalling if somehow the player
            //makes 1024 saves... the cap SHOULD stop this... but...
            //this will save my ass some day I bet
            if (index == 1024)
            {
                break;
            }
        }

        //we want to use an unused slot if we can find one
        //in case the player manually deletes an auto save
        //say the player has 10 saves, and deletes 3
        //but the current index is 7. We don't want to overwrite 8
        //we want to go back and save to 3 first. 
        if (index >= saveManagerSettings.AutoSaveSlotCount)
        {
            //we couldn't find a slot.

            //sort the list by the date modified so we always overwrite the oldest save.
            var listOfAutoSaves = saveDataInfo.AutoSaveLocations.OrderBy(x => x.Value.DateModified).ToArray();

            //foreach (var item in listOfAutoSaves)
            //{
            //    Debug.Log($"AutoSave[{item.Key}]: {item.Value.dateModified}");
            //}
            //Debug.Log($"Soooo.... >.> {listOfAutoSaves[0].Key}");

            //get the oldest key and use that auto save key, this can be out of order so you need to get the key
            saveDataInfo.CurrentAutoSaveIndex = listOfAutoSaves[0].Key;

            //make sure we never go over the auto save count so if the auto save index == it should be 0
            saveDataInfo.CurrentAutoSaveIndex %= saveManagerSettings.AutoSaveSlotCount;

            //Debug.Log($"Couldn't find an empty Auto Save, Overwriting the Oldest: " +
            //$"{saveDataInfo.currentAutoSaveIndex} -> {listOfAutoSaves.First().Value.dateModified}");

        }
        else
        {
            saveDataInfo.CurrentAutoSaveIndex = index;
        }

        return index;
    }
    public static bool SaveGameToCurrentSlot(GameData gameData, bool force = false, string saveName = null, string version = null)
    {
        return SaveGame(saveDataInfo.CurrentSaveIndex, gameData, force, saveName, version);
    }
    public static bool SaveGameToNewSlot(GameData gameData, bool force = false, string saveName = null, string version = null)
    {
        saveDataInfo.CurrentSaveIndex = GetUnusedSaveSlot();
        return SaveGame(saveDataInfo.CurrentSaveIndex, gameData, force, saveName, version);
    }
    public static bool CanOverwriteSave(int saveSlot, bool autoSave)
    {
        ValidateDirectories();
        if (!autoSave)
        {
            var filePath = $"{Application.persistentDataPath}/Saves/Save{saveSlot}.json";

            //do we have a file there already?
            //if true we can overwrite it lol
            return FileHelper.Exists(filePath);
        }

        //auto saves can't be overridden
        return false;
    }

    public static bool SaveGame(int saveSlot, GameData gameData, bool force = false, string saveName = null, string version = null, Texture2D image = null, bool saveSceneToLoadOnLoad = true, string overrideSceneToLoadOnLoad = null)
    {
        ValidateDirectories();

        if (!string.IsNullOrEmpty(overrideSceneToLoadOnLoad))
        {
            gameData.SceneToLoadOnLoad = overrideSceneToLoadOnLoad;
        }
        else if (saveSceneToLoadOnLoad)
        {
            gameData.SceneToLoadOnLoad = SceneController.GetCurrentMainScene();
        }

        var filePath = $"{Application.persistentDataPath}/Saves/Save{saveSlot}.json";

        //fail the save if it exists but we're not forcing it
        var fileExists = FileHelper.Exists(filePath);
        if (fileExists && force == false)
        {
            return false;
        }
        string imagePath = null;
        if (image != null)
        {
            //Serialize the image to disk
            var png = ImageConversion.EncodeToJPG(image, 75);

            imagePath = $"{Application.persistentDataPath}/Saves/Save{saveSlot}.jpg";
            FileHelper.WriteAllBytes(imagePath, png);
        }

        //Update save info
        //Debug.Log("Creating new save info");
        var newSaveInfo = new SaveGameInfo()
        {
            SaveSlot = saveSlot,
            Name = (!string.IsNullOrEmpty(saveName)) ? saveName : $"Save {saveSlot}",
            DateModified = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"),
            FilePath = filePath,
            ImagePath = imagePath,
            Version = (!string.IsNullOrEmpty(version)) ? version : $"0.0.1",
            IsAutoSave = false,
        };
        //Debug.Log(newSaveInfo);

        if (!saveDataInfo.SaveLocations.ContainsKey(saveSlot))
        {
            saveDataInfo.SaveLocations.Add(saveSlot, newSaveInfo);
        }
        else
        {
            saveDataInfo.SaveLocations[saveSlot] = newSaveInfo;
        }

        Messages.GetOnce<SaveGameStartingSaveMessage>().Dispatch();
        //Debug.Log($"Serializing game data, writing to {filePath}");
        var fileContents = JsonConvert.SerializeObject(gameData, settings);
        FileHelper.WriteAllText(filePath, fileContents);

        SaveInfoFile();
        Messages.GetOnce<SaveGameListChangedMessage>().Dispatch();

        Messages.GetOnce<SaveGameSavingCompletedMessage>().Dispatch();
        return true;
    }

    public static void AutoSave(GameData gameData, string version = null)
    {
        var unusedSlot = GetAutoSaveToUse();
        DebugLog($"<color=#F0F>Using Auto Save Slot {unusedSlot}");
        AutoSaveToSlot(unusedSlot, gameData, version);
    }
    public static void AutoSaveToSlot(int saveSlot, GameData gameData, string version = null)
    {
        ValidateDirectories();
        //get the current loaded scene so we reload that scene on load
        gameData.SceneToLoadOnLoad = SceneController.GetCurrentMainScene();

        //create the file contents
        var fileContents = JsonConvert.SerializeObject(gameData, settings);
        var filePath = $"{Application.persistentDataPath}/AutoSaves/AutoSave{saveDataInfo.CurrentAutoSaveIndex}.json";
        FileHelper.WriteAllText(filePath, fileContents);

        //update the save info
        var autoSaveInfo = new SaveGameInfo()
        {
            SaveSlot = saveSlot,
            Name = saveManagerSettings.AutoSaveNameFormat.Replace("[VALUE]", $"{saveSlot}"),
            DateModified = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"),
            FilePath = filePath,
            Version = (!string.IsNullOrEmpty(version)) ? version : $"0.0.1",
            IsAutoSave = true,
        };

        //write to the save slot
        if (!saveDataInfo.AutoSaveLocations.ContainsKey(saveSlot))
        {
            saveDataInfo.AutoSaveLocations.Add(saveSlot, autoSaveInfo);
        }
        else
        {
            saveDataInfo.AutoSaveLocations[saveSlot] = autoSaveInfo;
        }

        SaveInfoFile();
        Messages.GetOnce<SaveGameListChangedMessage>().Dispatch();
    }
    public static bool DeleteSaveGame(int saveSlot, bool isAutoSave, bool force = false)
    {
        if ((!isAutoSave && saveDataInfo.SaveLocations == null) ||
            (isAutoSave && saveDataInfo.AutoSaveLocations == null))
        {
            Debug.LogError("Somehow save locations is null");
            return false;
        }
        if ((!isAutoSave && saveDataInfo.SaveLocations.Values.Count == 0) ||
            (isAutoSave && saveDataInfo.AutoSaveLocations.Values.Count == 0))
        {
            Debug.LogError("No saves found");
            return false;
        }

        if ((!isAutoSave && !saveDataInfo.SaveLocations.ContainsKey(saveSlot)) ||
            (isAutoSave && !saveDataInfo.AutoSaveLocations.ContainsKey(saveSlot)))
        {
            Debug.LogError("Tried to load an save slot that doesn't exist");
            return false;
        }

        if (force == false)
        {
            return false;
        }

        string expectedPath;
        if (isAutoSave)
        {
            expectedPath = saveDataInfo.AutoSaveLocations[saveSlot].FilePath;
        }
        else
        {
            expectedPath = saveDataInfo.SaveLocations[saveSlot].FilePath;
        }

        //Debug.Log($"Deleting file:{expectedPath}");
        if (FileHelper.Exists(expectedPath))
        {
            FileHelper.Delete(expectedPath);
        }
        if (isAutoSave)
        {
            saveDataInfo.AutoSaveLocations.Remove(saveSlot);
        }
        else
        {
            saveDataInfo.SaveLocations.Remove(saveSlot);
        }
        SaveInfoFile();
        Messages.GetOnce<SaveGameListChangedMessage>().Dispatch();
        return true;
    }
    private static void SaveInfoFile()
    {
        ValidateDirectories();

        var filePath = $"{Application.persistentDataPath}/save_data_info.json";
        var fileContents = JsonConvert.SerializeObject(saveDataInfo, settings);

        //Debug.Log($"Saving Info File! {filePath}");

        FileHelper.WriteAllText(filePath, fileContents);
    }
    public static GameData GetGameData()
    {
        return loadedSave;
    }
    public static bool HasSaveGame(int saveSlot, bool isAutoSave)
    {
        if (!isAutoSave)
        {
            if (saveDataInfo.SaveLocations.ContainsKey(saveSlot))
            {
                return true;
            }
        }
        else if (isAutoSave)
        {
            if (saveDataInfo.AutoSaveLocations.ContainsKey(saveSlot))
            {
                return true;
            }
        }
        return false;
    }
    public static SaveGameInfo GetSaveGameInfo(int saveSlot, bool isAutoSave)
    {
        if (!isAutoSave)
        {
            if (saveDataInfo.SaveLocations.ContainsKey(saveSlot))
            {
                return saveDataInfo.SaveLocations[saveSlot];
            }
        }
        else if (isAutoSave)
        {
            if (saveDataInfo.AutoSaveLocations.ContainsKey(saveSlot))
            {
                return saveDataInfo.AutoSaveLocations[saveSlot];
            }
        }
        return null;
    }
    private static void ValidateDirectories()
    {
        if (!DirectoryHelper.Exists($"{Application.persistentDataPath}/Saves"))
        {
            DirectoryHelper.CreateDirectory($"{Application.persistentDataPath}/Saves");
        }

        if (!DirectoryHelper.Exists($"{Application.persistentDataPath}/AutoSaves"))
        {
            DirectoryHelper.CreateDirectory($"{Application.persistentDataPath}/AutoSaves");
        }
    }




    #region Debug
    private static void DebugLog(string log, GameObject target = null)
    {
        if (saveManagerSettings.ShowConsoleMessages)
        {
            Debug.Log(log, target);
        }
    }
    private static void DebugLogWarning(string log, GameObject target = null)
    {
        if (saveManagerSettings.ShowConsoleMessages)
        {
            Debug.LogWarning(log, target);
        }
    }
    private static void DebugLogError(string log, GameObject target = null)
    {
        if (saveManagerSettings.ShowConsoleMessages)
        {
            Debug.LogError(log, target);
        }
    }
    #endregion

#if UNITY_EDITOR

    [MenuItem("Snowdrama/Required/Create Save Manager Settings JSON")]
    public static void CreateSaveManagerSettingsJSON()
    {
        SaveManagerSettings defaultData = new()
        {
            ShowConsoleMessages = true,
            AutoSaveSlotCount = 4,
            AutoSaveNameFormat = "Auto Save [VALUE]",
        };

        var dataString = JsonConvert.SerializeObject(defaultData, new JsonSerializerSettings() { Formatting = Formatting.Indented });
        if (!FileHelper.Exists($"Assets/Resources/SaveManagerSettings.jsonc"))
        {
            FileHelper.WriteAllText($"Assets/Resources/SaveManagerSettings.jsonc", dataString);
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError("DANGER! ENSURE YOU ACTUALLY WANT TO DO THIS!!! " +
                "Can't overwrite SaveManagerSettings.jsonc because it already exists! " +
                "Overwriting this would delete any scene configuration you have! " +
                "Check the SaveManagerSettings.jsonc file and ensure you actually want to delete it! " +
                "If this ACTUALLY intended please manually delete the SaveManagerSettings.jsonc and run again. ");
        }
    }
#endif

}
