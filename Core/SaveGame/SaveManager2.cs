using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor;


//using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Save Manager 2 has one main change. 
/// 
/// The big thing I wanted to do is also support save slots with multiple saves
/// 
/// Example: KOTOR Allows you to select a character and see all the saves for that character. 
/// So a character is a save slot and a save slot contains multiple saves.
/// 
/// This also includes autosaves, so autosaves for a save slot don't overwrite autosaves from other save slots.
/// </summary>

namespace Snowdrama
{

    #region SaveDataManager2
    public class SaveDataManager2
    {
        public static readonly string SAVE_PATH = $"{Application.persistentDataPath}/Saves/";
        public static readonly string SAVE_MANIFEST_PATH = $"{Application.persistentDataPath}/Saves/manifest.json";
        public static readonly string DEFAULT_SAVE_NAME = "DefaultSave";

        private static SaveInfoManifest saveManifest;
        private static SaveData currentSaveData;
        #region Bootstrap
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Bootstrap()
        {
            //load the save manifest
            if (FileHelper.Exists(SAVE_MANIFEST_PATH))
            {
                var json = FileHelper.ReadAllText(SAVE_MANIFEST_PATH);
                saveManifest = JsonConvert.DeserializeObject<SaveInfoManifest>(json);
            }
            else
            {
                //make a new save manifest
                saveManifest = new SaveInfoManifest();
                FileHelper.WriteAllText(SAVE_MANIFEST_PATH, JsonConvert.SerializeObject(saveManifest));
            }
        }
        #endregion
        #region Load
        public static void LoadSave(
            int saveSlotIndex = 0,
            int saveIndex = 0,
            bool isAutoSave = false,
            bool loadSceneOnStart = true)
        {
            if (!saveManifest.TryGetSaveSlot(saveSlotIndex, out var slotInfo))
            {
                Debug.LogError($"Save slot {saveSlotIndex} does not exist.");
                return;
            }

            // Use saveIndex here, not saveSlotIndex.
            if (!slotInfo.TryGetSave(saveIndex, out var saveInfo, isAutoSave))
            {
                Debug.LogError(
                    $"Save {(isAutoSave ? "auto-save" : "regular save")} " +
                    $"{saveIndex} does not exist in slot {saveSlotIndex}.");
                return;
            }

            if (string.IsNullOrWhiteSpace(saveInfo.SavePath) || !FileHelper.Exists(saveInfo.SavePath))
            {
                Debug.LogError($"Save file is missing: {saveInfo.SavePath}");
                return;
            }

            var json = FileHelper.ReadAllText(saveInfo.SavePath);
            var saveData = JsonConvert.DeserializeObject<SaveData>(json);

            if (saveData?.DataStorage == null)
            {
                Debug.LogError($"Save file is invalid: {saveInfo.SavePath}");
                return;
            }

            currentSaveData = saveData;
            GameDataManager2.SetGameData(saveData.DataStorage);

            if (loadSceneOnStart)
            {
                SceneController.GoToScene(saveData.SceneToLoad);
            }
            else
            {
                Debug.LogWarning($"Save loaded, but loading scene '{saveData.SceneToLoad}' was prevented.");
            }
        }
        #endregion
        #region NewGame
        public static void NewGame()
        {
            //Get default save data
            var saveDataJson = Resources.Load<TextAsset>(DEFAULT_SAVE_NAME).text;
            var saveData = JsonConvert.DeserializeObject<SaveData>(saveDataJson);
            //Send DataStorage to GameDataManager
            GameDataManager2.SetGameData(saveData.DataStorage);

            //now that the save is loaded into the GameDataManager load the scene
            SceneController.GoToScene(saveData.SceneToLoad);
        }
        #endregion
        #region  SaveGame

        public static void SaveGame(
            int saveSlotIndex = 0,
            int saveIndex = 0,
            bool isAutoSave = false,
            string saveName = null,
            string extraData = null,
            string sceneToLoad = null)
        {
            SaveGame(GameDataManager2.GetGameData(), saveSlotIndex, saveIndex, isAutoSave, saveName, extraData, sceneToLoad);
        }
        public static void SaveGame(
            DataStorage dataToStore,
            int saveSlotIndex = 0,
            int saveIndex = 0,
            bool isAutoSave = false,
            string saveName = null,
            string extraData = null,
            string sceneToLoad = null)
        {

            if (dataToStore == null)
            {
                Debug.LogError("Cannot save: dataToStore is null.");
                return;
            }

            var slotDirectory = Path.Combine(SAVE_PATH, $"SaveSlot{saveSlotIndex}");

            Directory.CreateDirectory(slotDirectory);

            // Keep autosaves separate so they cannot overwrite regular saves.
            var fileName = isAutoSave
                ? $"AutoSave{saveIndex}.json"
                : $"Save{saveIndex}.json";

            var savePath = Path.Combine(slotDirectory, fileName);

            var saveInfo = new SaveInfo
            {
                Name = !string.IsNullOrWhiteSpace(saveName) ? saveName : $"{(isAutoSave ? "Auto Save " : "Save Game ")} {saveIndex}",
                Extra = !string.IsNullOrWhiteSpace(extraData) ? extraData : $"{(isAutoSave ? "Auto Save" : "Save Game ")} {saveIndex}",
                SaveDate = DateTime.Now,
                SavePath = savePath,
                SaveIndex = saveIndex,
                SaveSlotIndex = saveSlotIndex,
                IsAutoSave = isAutoSave,
            };

            var saveData = new SaveData
            {
                SaveInfo = saveInfo,
                DataStorage = dataToStore,
                SceneToLoad = !string.IsNullOrWhiteSpace(sceneToLoad) ? sceneToLoad : SceneController.GetCurrentMainScene()
            };

            File.WriteAllText(savePath, JsonConvert.SerializeObject(saveData));

            saveManifest.GetOrCreateSaveSlot(saveSlotIndex, out var slot);
            slot.SetSave(saveIndex, saveInfo, isAutoSave);
            SaveManifest();
        }
        #endregion
        #region  DeleteSave
        public static bool DeleteSave(
            int saveSlotIndex,
            int saveIndex,
            bool isAutoSave = false)
        {

            if (!saveManifest.TryGetSaveSlot(saveSlotIndex, out var slot) ||
                !slot.TryGetSave(saveIndex, out var saveInfo, isAutoSave))
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(saveInfo.SavePath) &&
                FileHelper.Exists(saveInfo.SavePath))
            {
                FileHelper.Delete(saveInfo.SavePath);
            }

            if (isAutoSave)
            {
                slot.AutoSaves.Remove(saveIndex);
            }
            else
            {
                slot.Saves.Remove(saveIndex);
            }

            // Remove an empty slot from the manifest.
            if (slot.Saves.Count == 0 && slot.AutoSaves.Count == 0)
            {
                saveManifest.SaveSlots.Remove(saveSlotIndex);
            }

            SaveManifest();
            return true;
        }
        #endregion
        #region SaveManifest
        private static void SaveManifest()
        {
            DirectoryHelper.CreateDirectory(SAVE_PATH);
            FileHelper.WriteAllText(SAVE_MANIFEST_PATH, JsonConvert.SerializeObject(saveManifest));
        }
        #endregion

        #region Editor
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
        #endregion

    }
    #endregion

    #region SaveManagerSettings
    public class SaveManagerSettings
    {
        public bool ShowConsoleMessages = true;
        public int AutoSaveSlotCount = 4;
        public string AutoSaveNameFormat = "Auto Save [VALUE]";
    }
    #endregion

    #region GameDataManager2
    public class GameDataManager2
    {
        private static DataStorage loadedGameData;
        public static DataStorage GetGameData() { return loadedGameData; }
        public static void SetGameData(DataStorage setStorage) { loadedGameData = setStorage; }
        public static void SaveGame(int saveSlotIndex = 0, int saveIndex = 0, bool isAutoSave = false)
        {
            SaveDataManager2.SaveGame(loadedGameData, saveSlotIndex, saveIndex, isAutoSave);
        }
    }
    #endregion

    #region SaveData
    public class SaveData
    {
        public SaveInfo SaveInfo { get; set; }
        public string SceneToLoad { get; set; }
        /// <summary>
        /// The storage container that has the game data in it
        /// </summary>
        public DataStorage DataStorage { get; set; }
    }
    #endregion

    #region SaveInfo
    public class SaveInfo
    {
        /// <summary>
        /// The save slot index
        /// </summary>
        public int SaveSlotIndex { get; set; } = 0;
        /// <summary>
        /// The save index
        /// </summary>
        public int SaveIndex { get; set; } = 0;
        /// <summary>
        /// Is the save an auto save
        /// </summary>
        public bool IsAutoSave { get; set; } = false;
        /// <summary>
        /// The name ofthe save
        /// </summary>
        public string Name { get; set; } = "Save 1";
        /// <summary>
        /// The date/time the game was saved
        /// </summary>
        public DateTime SaveDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Used for things like a save description or some other per-game identifier
        /// </summary>
        public string Extra { get; set; } = "This is save 1.";

        /// <summary>
        /// This is the relative path to the thing that contains the SaveData
        /// 
        /// Example:
        /// Save1.json
        /// 
        /// when combined with a save slot this gets a full path
        /// Application.persistentDataPath/Saves/Slot1/ + Save1.json
        /// 
        /// When used not with a save slot this can be a full path like:
        /// Application.persistentDataPath/Saves/Save1.json
        /// </summary>
        public string SavePath { get; set; } = "Save1.json";
    }
    #endregion

    #region SaveSlotInfo
    public class SaveSlotInfo
    {
        //The save slot has it's own info 
        public SaveInfo SaveInfo { get; set; } = new();

        /// <summary>
        /// This is the list of the save info for the save slot
        /// </summary>
        public Dictionary<int, SaveInfo> Saves = new();

        /// <summary>
        /// This is the list of the auto save info for the slot
        /// </summary>
        public Dictionary<int, SaveInfo> AutoSaves = new();

        /// <summary>
        /// This is the relative path of all the save slots
        /// 
        /// Example:
        /// Application.persistentDataPath/Saves/Slot1/
        /// Application.persistentDataPath/Saves/Slot2/
        /// Application.persistentDataPath/Saves/Slot3/
        /// </summary>
        public string SaveSlotPath { get; set; } = $"{Application.persistentDataPath}/Saves/Slot1/";

        public bool TryGetSave(
            int saveIndex,
            out SaveInfo saveInfo,
            bool isAutoSave = false)
        {
            var saves = isAutoSave ? AutoSaves : Saves;
            return saves.TryGetValue(saveIndex, out saveInfo);
        }

        public void GetOrCreateSave(int saveIndex, out SaveInfo saveInfo, bool isAutoSave = false)
        {
            var saves = isAutoSave ? AutoSaves : Saves;

            if (!saves.TryGetValue(saveIndex, out saveInfo))
            {
                saveInfo = new SaveInfo();
                saves.Add(saveIndex, saveInfo);
            }
        }

        public void SetSave(int saveIndex, SaveInfo saveInfo, bool isAutoSave = false)
        {
            var saves = isAutoSave ? AutoSaves : Saves;
            saves[saveIndex] = saveInfo;
        }
    }
    #endregion

    #region SaveInfoManifest
    public class SaveInfoManifest
    {
        public Dictionary<int, SaveSlotInfo> SaveSlots = new Dictionary<int, SaveSlotInfo>();
        public bool TryGetSaveSlot(int slotIndex, out SaveSlotInfo saveInfo)
        {
            return SaveSlots.TryGetValue(slotIndex, out saveInfo);
        }

        public void GetOrCreateSaveSlot(int slotIndex, out SaveSlotInfo saveInfo)
        {
            if (!SaveSlots.TryGetValue(slotIndex, out saveInfo))
            {
                saveInfo = new SaveSlotInfo();
                SaveSlots.Add(slotIndex, saveInfo);
            }
        }

        public void SetSaveSlot(int slotIndex, SaveSlotInfo saveInfo)
        {
            SaveSlots[slotIndex] = saveInfo;
        }
    }
    #endregion


    public class SaveGameMessage : AMessage<(int saveSlotIndex, int saveIndex, string saveName, bool isAutoSave)> { }

    [RequireComponent(typeof(Button))]
    public class SaveButton : MonoBehaviour
    {
        private Button button;
        private int saveSlotIndex;
        private int saveIndex;
        private string saveName;
        private void Start()
        {
            button = this.GetComponent<Button>();
            button.onClick.AddListener(this.OnClick);
        }
        public void SetSaveInfo(int saveSlotIndex, int saveIndex, string saveName)
        {
            this.saveSlotIndex = saveSlotIndex;
            this.saveIndex = saveIndex;
            this.saveName = saveName;
        }
        private void OnClick()
        {
            Messages.Get<SaveGameMessage>().Dispatch((saveSlotIndex, saveIndex, saveName, false));
        }

        private void Save()
        {
        }
    }


    [RequireComponent(typeof(CanvasModal))]
    public class SaveModal : MonoBehaviour
    {
        [SerializeField] private TMP_InputField saveNameInput;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button cancelButton;
        private CanvasModal canvasModal;
        private SaveGameMessage SaveGameMessage;
        private int saveSlotIndex;
        private int saveIndex;
        private string saveName;
        private bool isAutoSave;
        private void Start()
        {
            saveButton.onClick.AddListener(this.OnComplete);
            cancelButton.onClick.AddListener(this.OnCancel);
            canvasModal = this.GetComponent<CanvasModal>();
        }
        private void OnEnable()
        {
            SaveGameMessage = Messages.Get<SaveGameMessage>();
            SaveGameMessage.AddListener(this.OnSaveGame);
        }

        private void OnDisable()
        {
            SaveGameMessage.RemoveListener(this.OnSaveGame);
            Messages.Return<SaveGameMessage>();
        }

        public void OnSaveGame((int saveSlotIndex, int saveIndex, string saveName, bool isAutoSave) args)
        {
            saveSlotIndex = args.saveSlotIndex;
            saveIndex = args.saveIndex;
            saveName = args.saveName;
            isAutoSave = args.isAutoSave;
            saveNameInput.text = args.saveName;
            canvasModal.ShowCanvasModal();
        }

        private void OnComplete()
        {
            saveName = saveNameInput.text;
            SaveDataManager2.SaveGame(saveSlotIndex, saveIndex, isAutoSave, saveName);
            canvasModal.HideCanvasModal();
        }

        private void OnCancel()
        {
            canvasModal.HideCanvasModal();
        }
    }

    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasModal : MonoBehaviour
    {
        private CanvasGroup group;
        [SerializeField] private float _fadeTime = 0.5f;
        private float _fadeSpeed => 1.0f / _fadeTime.Clamp(0.01f, float.MaxValue);
        private float _alpha = 0;
        private float Alpha
        {
            get => _alpha;
            set
            {
                if (_alpha != value)
                {
                    _alpha = value;
                    _alpha = _alpha.Clamp(0, 1);
                    group.alpha = _alpha;
                    if (_alpha > 0)
                    {
                        group.blocksRaycasts = true;
                        group.interactable = true;
                        group.ignoreParentGroups = true;
                    }
                    else
                    {
                        group.blocksRaycasts = true;
                        group.interactable = true;
                        group.ignoreParentGroups = true;
                    }
                }
            }
        }
        private float targetAlpha;

        private void Update()
        {
            this.Alpha = Mathf.MoveTowards(this.Alpha, targetAlpha, Time.deltaTime * this._fadeSpeed);
        }

        public void ShowCanvasModal()
        {
            targetAlpha = 1.0f;
        }
        public void HideCanvasModal()
        {
            targetAlpha = 1.0f;
        }
    }
}
