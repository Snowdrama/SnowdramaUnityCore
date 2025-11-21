using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

[System.Serializable]
public class SaveDataStruct
{
    public int currentSaveIndex = 0;
    public int currentAutoSaveIndex = 0;
    public Dictionary<int, SaveGameInfo> saveLocations = new Dictionary<int, SaveGameInfo>();
    public Dictionary<int, SaveGameInfo> autoSaveLocations = new Dictionary<int, SaveGameInfo>();
}

[System.Serializable]
public class SaveGameInfo
{
    public string name;
    public string version;
    public string dateModified;
    public string filePath;
}

public class SaveGameListChanged : AMessage { }

public class SaveManager : MonoBehaviour
{
    private static GameDataStruct loadedSave = new GameDataStruct();
    private static SaveDataStruct saveDataInfo = new SaveDataStruct();

    private static readonly JsonSerializerSettings settings = new JsonSerializerSettings()
    {
        Formatting = Formatting.Indented,
    };
    private void Awake()
    {
        ValidateDirectories();
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

        Debug.Log("Loading Load 0");
        //load save 0 by default in case we're testing
        LoadSave(saveDataInfo.currentSaveIndex);
    }

    public static SaveDataStruct GetSaveList()
    {
        return saveDataInfo;
    }

    public static bool LoadSave(int saveSlot)
    {
        ValidateDirectories();
        if (saveDataInfo.saveLocations == null)
        {
            Debug.LogError("Somehow saveLocations is null");
            return false;
        }
        if (saveDataInfo.saveLocations.Values.Count == 0)
        {
            Debug.LogError("No saves found");
            return false;
        }

        if (!saveDataInfo.saveLocations.ContainsKey(saveSlot))
        {
            Debug.LogError("Tried to load a save slot that doesn't exist");
            return false;
        }

        //can we find the save?
        Debug.Log($"Loading Load From: {saveDataInfo.saveLocations[saveSlot].filePath}");
        if (File.Exists(saveDataInfo.saveLocations[saveSlot].filePath))
        {
            var saveToLoad = saveDataInfo.saveLocations[saveSlot].filePath;
            var fileContents = File.ReadAllText(saveToLoad);

            Debug.Log($"Loading file from {saveToLoad}");
            Debug.Log(fileContents);
            loadedSave = JsonConvert.DeserializeObject<GameDataStruct>(fileContents, settings);

            saveDataInfo.currentSaveIndex = saveSlot;
            SaveInfoFile();
            Messages.GetOnce<SaveGameListChanged>().Dispatch();
            return true;
        }

        return false;
    }

    public static bool CanLoadSave(int saveSlot)
    {
        if (saveDataInfo.saveLocations == null)
        {
            Debug.LogError("Somehow saveLocations is null");
            return false;
        }
        if (saveDataInfo.saveLocations.Values.Count == 0)
        {
            Debug.LogError("No saves found");
            return false;
        }

        if (!saveDataInfo.saveLocations.ContainsKey(saveSlot))
        {
            Debug.LogError("Tried to load a save slot that doesn't exist");
            return false;
        }

        if (File.Exists(saveDataInfo.saveLocations[saveSlot].filePath))
        {
            return true;
        }
        return false;
    }

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

    public static bool SaveGame(GameDataStruct gameData, bool force = false, string saveName = null, string version = null)
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
            name = (!string.IsNullOrEmpty(saveName)) ? saveName : $"Save {saveSlot}",
            dateModified = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"),
            filePath = filePath,
            version = (!string.IsNullOrEmpty(version)) ? version : $"0.0.1",
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
        Debug.Log($"Serializing game data, writing to {filePath}");
        var fileContents = JsonConvert.SerializeObject(gameData, settings);
        File.WriteAllText(filePath, fileContents);

        SaveInfoFile();
        Messages.GetOnce<SaveGameListChanged>().Dispatch();

        return true;
    }

    public static void AutoSave(GameDataStruct gameData)
    {
        ValidateDirectories();
        var unusedSlot = GetUnusedAutoSaveSlot();

        //we always want to use an unused slot if we can find one
        //in case the player manually deletes an auto save
        //say the player has 10 saves, and deletes 3
        //but the current index is 7. We don't want to overwrite 8
        //we want to go back and save to 3 first. 

        if (unusedSlot > 10)
        {
            //we couldn't find a slot.

            //TODO: Sort the auto save slots by the save date/time

            //increment the current auto save slot
            saveDataInfo.currentAutoSaveIndex++;
            //arbitrary loop on auto save 10.
            saveDataInfo.currentAutoSaveIndex = saveDataInfo.currentAutoSaveIndex % 10;
        }
        else
        {
            saveDataInfo.currentAutoSaveIndex = unusedSlot;
        }

        //create the file contents
        var fileContents = JsonConvert.SerializeObject(gameData, settings);
        var filePath = $"{Application.persistentDataPath}/AutoSaves/AutoSave{saveDataInfo.currentAutoSaveIndex}.json";
        File.WriteAllText(filePath, fileContents);
        SaveInfoFile();
        Messages.GetOnce<SaveGameListChanged>().Dispatch();
    }

    public static bool DeleteSaveGame(int saveSlot, bool force = false)
    {
        if (saveDataInfo.saveLocations == null)
        {
            Debug.LogError("Somehow saveLocations is null");
            return false;
        }
        if (saveDataInfo.saveLocations.Values.Count == 0)
        {
            Debug.LogError("No saves found");
            return false;
        }

        if (!saveDataInfo.saveLocations.ContainsKey(saveSlot))
        {
            Debug.LogError("Tried to load a save slot that doesn't exist");
            return false;
        }

        if (force == false)
        {
            return false;
        }

        Debug.Log($"Deleting file:{Application.persistentDataPath}/Saves/Save{saveSlot}.json");
        if (File.Exists($"{Application.persistentDataPath}/Saves/Save{saveSlot}.json"))
        {
            File.Delete($"{Application.persistentDataPath}/Saves/Save{saveSlot}.json");
        }
        saveDataInfo.saveLocations.Remove(saveSlot);
        SaveInfoFile();
        Messages.GetOnce<SaveGameListChanged>().Dispatch();
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
