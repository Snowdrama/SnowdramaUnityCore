using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

[System.Serializable]
public class SaveDataStruct
{
    public int currentSaveIndex = 0;
    public int currentAutoSaveIndex = 0;
    public Dictionary<int, SaveGameInfo> saveLocations = new();
    public Dictionary<int, SaveGameInfo> autoSaveLocations = new();
}

[System.Serializable]
public class SaveGameInfo
{
    public int saveSlot;
    public string name;
    public string version;
    public string dateModified;
    public string filePath;
    public string imagePath;
    public bool isAutoSave;
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
        public int autoSaveSlotCount = 4;
        public string AutoSaveNameFormat = "Auto Save [VALUE]";
    }
    private static SaveManagerSettings saveManagerSettings;

    private static int currentSaveSlot = -1;

    private static GameData loadedSave = new();
    //private static GameData activeSave = new();

    private static SaveDataStruct saveDataInfo = new();
    private static readonly JsonSerializerSettings settings = new()
    {
        Formatting = Formatting.Indented,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    };
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

        var saveDataInfoPath = $"{Application.persistentDataPath}/save_data_info.json";
        if (File.Exists(saveDataInfoPath))
        {
            //Debug.Log($"Loading SaveDataInfo");
            var saveDataInfoJson = File.ReadAllText(saveDataInfoPath);
            //Debug.Log(saveDataInfoJson);
            saveDataInfo = JsonConvert.DeserializeObject<SaveDataStruct>(saveDataInfoJson, settings);
        }
        else
        {
            //Debug.Log($"Creating New SaveDataInfo");
            saveDataInfo = new SaveDataStruct();
        }

#if UNITY_EDITOR
        //load save 0 by default in case we're testing
        //then we can load into any scene with the save 0 data
        if (saveDataInfo.saveLocations.Count == 0)
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
    public static SaveDataStruct GetSaveList()
    {
        return saveDataInfo;
    }
    #region Load Game
    public static bool CanLoadSave(int saveSlot, bool autoSave)
    {
        //Debug.Log($"Loading an Auto Save: {autoSave}");
        if ((!autoSave && saveDataInfo.saveLocations == null) ||
            (autoSave && saveDataInfo.autoSaveLocations == null))
        {
            Debug.LogError("Somehow save locations is null");
            return false;
        }
        if ((!autoSave && saveDataInfo.saveLocations.Values.Count == 0) ||
            (autoSave && saveDataInfo.autoSaveLocations.Values.Count == 0))
        {
            Debug.LogError($"No saves found, " +
                $"AutoSave.Count: {saveDataInfo.saveLocations.Values.Count} " +
                $"SaveCount: {saveDataInfo.saveLocations.Values.Count}");
            return false;
        }

        if ((!autoSave && !saveDataInfo.saveLocations.ContainsKey(saveSlot)) ||
            (autoSave && !saveDataInfo.autoSaveLocations.ContainsKey(saveSlot)))
        {
            Debug.LogError($"Failed to load, IsAutoSave: {autoSave} SaveSlot: {saveSlot}");
            return false;
        }

        string expectedPath;
        //can we find the save?
        if (autoSave)
        {
            expectedPath = saveDataInfo.autoSaveLocations[saveSlot].filePath;
        }
        else
        {
            expectedPath = saveDataInfo.saveLocations[saveSlot].filePath;
        }

        if (File.Exists(expectedPath))
        {
            return true;
        }
        return false;
    }
    public static bool LoadSave(int saveSlot, bool isAutoSave, bool autoLoadScene = true)
    {
        ValidateDirectories();
        //Debug.Log($"Loading an Save: {isAutoSave}");
        if ((!isAutoSave && saveDataInfo.saveLocations == null) ||
            (isAutoSave && saveDataInfo.autoSaveLocations == null))
        {
            Debug.LogError("Somehow save locations is null");
            return false;
        }
        if ((!isAutoSave && saveDataInfo.saveLocations.Values.Count == 0) ||
            (isAutoSave && saveDataInfo.autoSaveLocations.Values.Count == 0))
        {
            Debug.LogError("No saves found");
            return false;
        }

        if ((!isAutoSave && !saveDataInfo.saveLocations.ContainsKey(saveSlot)) ||
            (isAutoSave && !saveDataInfo.autoSaveLocations.ContainsKey(saveSlot)))
        {
            Debug.LogError("Tried to load an save slot that doesn't exist");
            return false;
        }

        //set the current save slot so we know which was the last uses
        currentSaveSlot = saveSlot;

        string expectedPath;
        if (isAutoSave)
        {
            expectedPath = saveDataInfo.autoSaveLocations[saveSlot].filePath;

            //if we're loading an auto save, we do this so it
            saveDataInfo.currentAutoSaveIndex = saveSlot;
        }
        else
        {
            expectedPath = saveDataInfo.saveLocations[saveSlot].filePath;
        }
        //Debug.Log($"Loading Save From: {expectedPath}");


        if (File.Exists(expectedPath))
        {
            var savePathToLoad = expectedPath;
            var fileContents = File.ReadAllText(savePathToLoad);

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
    #endregion
    public static int GetUnusedSaveSlot()
    {
        var index = 0;
        //This assumes that there's no way the player
        //has int.MaxValue number of saves...
        //we will EVENTUALLY find one they don't use
        while (saveDataInfo.saveLocations.ContainsKey(index))
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
        while (saveDataInfo.autoSaveLocations.ContainsKey(index))
        {
            index++;

            //if we reach the max count we didn't find any empty slot
            if (index == saveManagerSettings.autoSaveSlotCount)
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
        if (index >= saveManagerSettings.autoSaveSlotCount)
        {
            //we couldn't find a slot.

            //sort the list by the date modified so we always overwrite the oldest save.
            var listOfAutoSaves = saveDataInfo.autoSaveLocations.OrderBy(x => x.Value.dateModified).ToArray();

            //foreach (var item in listOfAutoSaves)
            //{
            //    Debug.Log($"AutoSave[{item.Key}]: {item.Value.dateModified}");
            //}
            //Debug.Log($"Soooo.... >.> {listOfAutoSaves[0].Key}");

            //get the oldest key and use that auto save key, this can be out of order so you need to get the key
            saveDataInfo.currentAutoSaveIndex = listOfAutoSaves[0].Key;

            //make sure we never go over the auto save count so if the auto save index == it should be 0
            saveDataInfo.currentAutoSaveIndex %= saveManagerSettings.autoSaveSlotCount;

            //Debug.Log($"Couldn't find an empty Auto Save, Overwriting the Oldest: " +
            //$"{saveDataInfo.currentAutoSaveIndex} -> {listOfAutoSaves.First().Value.dateModified}");

        }
        else
        {
            saveDataInfo.currentAutoSaveIndex = index;
        }

        return index;
    }
    public static bool SaveGameToCurrentSlot(GameData gameData, bool force = false, string saveName = null, string version = null)
    {
        return SaveGame(saveDataInfo.currentSaveIndex, gameData, force, saveName, version);
    }
    public static bool SaveGameToNewSlot(GameData gameData, bool force = false, string saveName = null, string version = null)
    {
        saveDataInfo.currentSaveIndex = GetUnusedSaveSlot();
        return SaveGame(saveDataInfo.currentSaveIndex, gameData, force, saveName, version);
    }
    public static bool CanOverwriteSave(int saveSlot, bool autoSave)
    {
        ValidateDirectories();
        if (!autoSave)
        {
            var filePath = $"{Application.persistentDataPath}/Saves/Save{saveSlot}.json";

            //do we have a file there already?
            //if true we can overwrite it lol
            return File.Exists(filePath);
        }

        //auto saves can't be overridden
        return false;
    }

    public static bool SaveGame(int saveSlot, GameData gameData, bool force = false, string saveName = null, string version = null, Texture2D image = null)
    {
        ValidateDirectories();

        var filePath = $"{Application.persistentDataPath}/Saves/Save{saveSlot}.json";

        //fail the save if it exists but we're not forcing it
        var fileExists = File.Exists(filePath);
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
            File.WriteAllBytes(imagePath, png);
        }

        //Update save info
        //Debug.Log("Creating new save info");
        var newSaveInfo = new SaveGameInfo()
        {
            saveSlot = saveSlot,
            name = (!string.IsNullOrEmpty(saveName)) ? saveName : $"Save {saveSlot}",
            dateModified = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"),
            filePath = filePath,
            imagePath = imagePath,
            version = (!string.IsNullOrEmpty(version)) ? version : $"0.0.1",
            isAutoSave = false,
        };
        //Debug.Log(newSaveInfo);

        if (!saveDataInfo.saveLocations.ContainsKey(saveSlot))
        {
            saveDataInfo.saveLocations.Add(saveSlot, newSaveInfo);
        }
        else
        {
            saveDataInfo.saveLocations[saveSlot] = newSaveInfo;
        }

        Messages.GetOnce<SaveGameStartingSaveMessage>().Dispatch();
        //Debug.Log($"Serializing game data, writing to {filePath}");
        var fileContents = JsonConvert.SerializeObject(gameData, settings);
        File.WriteAllText(filePath, fileContents);

        SaveInfoFile();
        Messages.GetOnce<SaveGameListChangedMessage>().Dispatch();

        Messages.GetOnce<SaveGameSavingCompletedMessage>().Dispatch();
        return true;
    }
    public static void AutoSave(GameData gameData, string version = null)
    {
        var unusedSlot = GetAutoSaveToUse();

        AutoSaveToSlot(unusedSlot, gameData, version);
    }
    public static void AutoSaveToSlot(int saveSlot, GameData gameData, string version = null)
    {
        ValidateDirectories();
        //get the current loaded scene so we reload that scene on load
        gameData.SceneToLoadOnLoad = SceneController.GetCurrentMainScene();

        //create the file contents
        var fileContents = JsonConvert.SerializeObject(gameData, settings);
        var filePath = $"{Application.persistentDataPath}/AutoSaves/AutoSave{saveDataInfo.currentAutoSaveIndex}.json";
        File.WriteAllText(filePath, fileContents);

        //update the save info
        var autoSaveInfo = new SaveGameInfo()
        {
            saveSlot = saveSlot,
            name = saveManagerSettings.AutoSaveNameFormat.Replace("[VALUE]", $"{saveSlot}"),
            dateModified = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"),
            filePath = filePath,
            version = (!string.IsNullOrEmpty(version)) ? version : $"0.0.1",
            isAutoSave = true,
        };

        //write to the save slot
        if (!saveDataInfo.autoSaveLocations.ContainsKey(saveSlot))
        {
            saveDataInfo.autoSaveLocations.Add(saveSlot, autoSaveInfo);
        }
        else
        {
            saveDataInfo.autoSaveLocations[saveSlot] = autoSaveInfo;
        }

        SaveInfoFile();
        Messages.GetOnce<SaveGameListChangedMessage>().Dispatch();
    }
    public static bool DeleteSaveGame(int saveSlot, bool isAutoSave, bool force = false)
    {
        if ((!isAutoSave && saveDataInfo.saveLocations == null) ||
            (isAutoSave && saveDataInfo.autoSaveLocations == null))
        {
            Debug.LogError("Somehow save locations is null");
            return false;
        }
        if ((!isAutoSave && saveDataInfo.saveLocations.Values.Count == 0) ||
            (isAutoSave && saveDataInfo.autoSaveLocations.Values.Count == 0))
        {
            Debug.LogError("No saves found");
            return false;
        }

        if ((!isAutoSave && !saveDataInfo.saveLocations.ContainsKey(saveSlot)) ||
            (isAutoSave && !saveDataInfo.autoSaveLocations.ContainsKey(saveSlot)))
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
            expectedPath = saveDataInfo.autoSaveLocations[saveSlot].filePath;
        }
        else
        {
            expectedPath = saveDataInfo.saveLocations[saveSlot].filePath;
        }

        //Debug.Log($"Deleting file:{expectedPath}");
        if (File.Exists(expectedPath))
        {
            File.Delete(expectedPath);
        }
        if (isAutoSave)
        {
            saveDataInfo.autoSaveLocations.Remove(saveSlot);
        }
        else
        {
            saveDataInfo.saveLocations.Remove(saveSlot);
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

        File.WriteAllText(filePath, fileContents);
    }
    public static GameData GetGameData()
    {
        return loadedSave;
    }
    public static bool HasSaveGame(int saveSlot, bool isAutoSave)
    {
        if (!isAutoSave)
        {
            if (saveDataInfo.saveLocations.ContainsKey(saveSlot))
            {
                return true;
            }
        }
        else if (isAutoSave)
        {
            if (saveDataInfo.autoSaveLocations.ContainsKey(saveSlot))
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
            if (saveDataInfo.saveLocations.ContainsKey(saveSlot))
            {
                return saveDataInfo.saveLocations[saveSlot];
            }
        }
        else if (isAutoSave)
        {
            if (saveDataInfo.autoSaveLocations.ContainsKey(saveSlot))
            {
                return saveDataInfo.autoSaveLocations[saveSlot];
            }
        }
        return null;
    }
    private static void ValidateDirectories()
    {
        if (!Directory.Exists($"{Application.persistentDataPath}/Saves"))
        {
            Directory.CreateDirectory($"{Application.persistentDataPath}/Saves");
        }

        if (!Directory.Exists($"{Application.persistentDataPath}/AutoSaves"))
        {
            Directory.CreateDirectory($"{Application.persistentDataPath}/AutoSaves");
        }
    }
}
