using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public bool isAutoSave;
}

public class SaveGameListChangedMessage : AMessage { }
public class SaveGameLoadedMessage : AMessage<GameDataStruct> { }

//this gets triggered right before the game serializes
//good to listen for this and have all objects write their data here
public class SaveGameStartingSaveMessage : AMessage { }

//this is called right after a game saved, useful for triggering things like "Save and Quit"
public class SaveGameSavingCompletedMessage : AMessage { }

public class SaveManager : MonoBehaviour
{
    private static GameDataStruct loadedSave = new();
    private static SaveDataStruct saveDataInfo = new();

    private static readonly JsonSerializerSettings settings = new()
    {
        Formatting = Formatting.Indented,
    };
    private void Awake()
    {
        ValidateDirectories();
        NewGame();
        var saveDataInfoPath = $"{Application.persistentDataPath}/save_data_info.json";
        if (File.Exists(saveDataInfoPath))
        {
            Debug.Log($"Loading SaveDataInfo");
            var saveDataInfoJson = File.ReadAllText(saveDataInfoPath);
            Debug.Log(saveDataInfoJson);
            saveDataInfo = JsonConvert.DeserializeObject<SaveDataStruct>(saveDataInfoJson, settings);
        }
        else
        {
            Debug.Log($"Creating New SaveDataInfo");
            saveDataInfo = new SaveDataStruct();
        }

        foreach (SaveGameInfo file in saveDataInfo.saveLocations.Values)
        {
            Debug.Log($"Found Load: {file.name} {file.filePath}");
        }

        foreach (SaveGameInfo file in saveDataInfo.autoSaveLocations.Values)
        {
            Debug.Log($"Found Load: {file.name} {file.filePath}");
        }

#if UNITY_EDITOR
        Debug.Log("Loading Load 0");
        //load save 0 by default in case we're testing
        //then we can load into any scene with the save 0 data
        LoadSave(saveDataInfo.currentSaveIndex, false, false);
#endif
    }

    public void NewGame()
    {
        Debug.LogWarning($"Starting new game, loading default data!");
        //load the default save from resources:
        var defaultSaveJson = Resources.Load<TextAsset>("DefaultSave.jsonc");
        if (defaultSaveJson != null)
        {
            Debug.LogWarning($"Loaded Default Save!: {defaultSaveJson.text}");
        }
        else
        {
            Debug.LogError($"No Default save! " +
                $"Make sure you have a DefaultSave.jsonc in the Resources folder!");
        }
    }

    public static SaveDataStruct GetSaveList()
    {
        return saveDataInfo;
    }

    #region Load Game
    public static bool CanLoadSave(int saveSlot, bool autoSave)
    {
        Debug.Log($"Loading an Auto Save: {autoSave}");
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
        Debug.Log($"Loading an Save: {isAutoSave}");
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

        string expectedPath;
        if (isAutoSave)
        {
            expectedPath = saveDataInfo.autoSaveLocations[saveSlot].filePath;
        }
        else
        {
            expectedPath = saveDataInfo.saveLocations[saveSlot].filePath;
        }
        Debug.Log($"Loading Save From: {expectedPath}");


        if (File.Exists(expectedPath))
        {
            var savePathToLoad = expectedPath;
            var fileContents = File.ReadAllText(savePathToLoad);

            Debug.Log($"Loading file from {savePathToLoad}");
            Debug.Log(fileContents);
            try
            {
                loadedSave = JsonConvert.DeserializeObject<GameDataStruct>(fileContents, settings);
            }
            catch (Exception)
            {
                Debug.LogError($"Failed to parse file contents of file at: {savePathToLoad}");
                throw;
            }

            //TODO: Maybe don't do this here? Let another thing handle this with SaveGameLoadedMessage?
            if (autoLoadScene == true && !string.IsNullOrEmpty(loadedSave.SceneToLoadOnLoad))
            {
                Debug.Log($"Let's load the scene! autoLoadScene: {autoLoadScene}: {loadedSave.SceneToLoadOnLoad}");
                SceneController.GoToScene(loadedSave.SceneToLoadOnLoad);
            }

            saveDataInfo.currentAutoSaveIndex = saveSlot;
            SaveInfoFile();
            Messages.GetOnce<SaveGameLoadedMessage>().Dispatch(loadedSave);
            Messages.GetOnce<SaveGameListChangedMessage>().Dispatch();
            return true;
        }

        return false;
    }
    #endregion

    public static int GetUnusedSaveSlot()
    {
        int index = 0;
        //This assumes that there's no way the player
        //has int.MaxValue number of saves...
        //we will EVENTUALLY find one they don't use
        while (saveDataInfo.saveLocations.ContainsKey(index))
        {
            index++;
        }
        return index;
    }

    public static int GetUnusedAutoSaveSlot()
    {
        int index = 0;
        //This assumes that there's no way the player
        //has int.MaxValue number of saves...
        //we will EVENTUALLY find one they don't use
        while (saveDataInfo.autoSaveLocations.ContainsKey(index))
        {
            index++;
        }
        return index;
    }

    public static bool SaveGameToCurrentSlot(GameDataStruct gameData, bool force = false, string saveName = null, string version = null)
    {
        return SaveGame(saveDataInfo.currentSaveIndex, gameData, force, saveName, version);
    }

    public static bool SaveGameToNewSlot(GameDataStruct gameData, bool force = false, string saveName = null, string version = null)
    {
        saveDataInfo.currentSaveIndex = GetUnusedSaveSlot();
        return SaveGame(saveDataInfo.currentSaveIndex, gameData, force, saveName, version);
    }

    public static bool SaveGame(int saveSlot, GameDataStruct gameData, bool force = false, string saveName = null, string version = null)
    {
        ValidateDirectories();

        var filePath = $"{Application.persistentDataPath}/Saves/Save{saveSlot}.json";

        //do we have a file there already?
        bool fileExists = File.Exists(filePath);

        Debug.Log($"File.Exists() = {fileExists} && force == {force}");

        if (fileExists && force == false)
        {
            Debug.LogWarning($"File.Exists() = {fileExists} && force == {force}");
            return false;
        }

        //Update save info
        Debug.Log("Creating new save info");
        var newSaveInfo = new SaveGameInfo()
        {
            saveSlot = saveSlot,
            name = (!string.IsNullOrEmpty(saveName)) ? saveName : $"Save {saveSlot}",
            dateModified = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"),
            filePath = filePath,
            version = (!string.IsNullOrEmpty(version)) ? version : $"0.0.1",
            isAutoSave = false,
        };
        Debug.Log(newSaveInfo);

        if (!saveDataInfo.saveLocations.ContainsKey(saveSlot))
        {
            saveDataInfo.saveLocations.Add(saveSlot, newSaveInfo);
        }
        else
        {
            saveDataInfo.saveLocations[saveSlot] = newSaveInfo;
        }

        Messages.GetOnce<SaveGameStartingSaveMessage>().Dispatch();

        gameData.SceneToLoadOnLoad = SceneController.GetCurrentMainScene();


        Debug.Log($"Serializing game data, writing to {filePath}");
        var fileContents = JsonConvert.SerializeObject(gameData, settings);
        File.WriteAllText(filePath, fileContents);

        SaveInfoFile();
        Messages.GetOnce<SaveGameListChangedMessage>().Dispatch();

        Messages.GetOnce<SaveGameSavingCompletedMessage>().Dispatch();
        return true;
    }

    public static void AutoSave(GameDataStruct gameData, string version = null)
    {
        ValidateDirectories();
        var unusedSlot = GetUnusedAutoSaveSlot();
        //we always want to use an unused slot if we can find one
        //in case the player manually deletes an auto save
        //say the player has 10 saves, and deletes 3
        //but the current index is 7. We don't want to overwrite 8
        //we want to go back and save to 3 first. 

        if (unusedSlot >= 10)
        {
            //we couldn't find a slot.
            var listOfAutoSaves = saveDataInfo.autoSaveLocations.OrderBy(x => x.Value.dateModified).ToArray();

            foreach (var item in listOfAutoSaves)
            {
                Debug.Log($"AutoSave[{item.Key}]: {item.Value.dateModified}");
            }
            Debug.Log($"Soooo.... >.> {listOfAutoSaves[0].Key}");

            //get the oldest key and use that auto save key:
            saveDataInfo.currentAutoSaveIndex = listOfAutoSaves[0].Key;
            //arbitrary loop on auto save 10.
            saveDataInfo.currentAutoSaveIndex %= 10;


            Debug.LogWarning($"Couldn't find an empty Auto Save, Overwriting the Oldest: " +
                $"{saveDataInfo.currentAutoSaveIndex} -> {listOfAutoSaves.First().Value.dateModified}");

        }
        else
        {
            saveDataInfo.currentAutoSaveIndex = unusedSlot;
        }


        gameData.SceneToLoadOnLoad = SceneController.GetCurrentMainScene();

        //create the file contents
        var fileContents = JsonConvert.SerializeObject(gameData, settings);
        var filePath = $"{Application.persistentDataPath}/AutoSaves/AutoSave{saveDataInfo.currentAutoSaveIndex}.json";
        File.WriteAllText(filePath, fileContents);

        Debug.Log($"Auto Save[{saveDataInfo.currentAutoSaveIndex}]: {filePath}");
        var autoSaveInfo = new SaveGameInfo()
        {
            saveSlot = saveDataInfo.currentAutoSaveIndex,
            name = $"Autosave {saveDataInfo.currentAutoSaveIndex}",
            dateModified = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"),
            filePath = filePath,
            version = (!string.IsNullOrEmpty(version)) ? version : $"0.0.1",
            isAutoSave = true,
        };

        if (!saveDataInfo.autoSaveLocations.ContainsKey(saveDataInfo.currentAutoSaveIndex))
        {
            saveDataInfo.autoSaveLocations.Add(saveDataInfo.currentAutoSaveIndex, autoSaveInfo);
        }
        else
        {
            saveDataInfo.autoSaveLocations[saveDataInfo.currentAutoSaveIndex] = autoSaveInfo;
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

        Debug.Log($"Deleting file:{expectedPath}");
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

        Debug.Log($"Saving Info File! {filePath}");

        File.WriteAllText(filePath, fileContents);
    }

    public static GameDataStruct GetGameData()
    {
        return loadedSave;
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
