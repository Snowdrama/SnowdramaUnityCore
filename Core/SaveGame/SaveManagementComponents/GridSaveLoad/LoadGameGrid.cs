using Snowdrama.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadGameGrid : MonoBehaviour
{
    private SaveGameListChangedMessage gameListChanged;

    [Header("Prefab")]
    [SerializeField] private LoadGameGridButton LoadGameButtonPrefab;

    [Header("Settings")]
    [SerializeField] private bool UseNormalSaves = true;
    [SerializeField] private bool UseAutoSaves = false;
    [SerializeField] private bool SortByDate = true;
    [SerializeField] private bool MixAutoSaves = false;

    [Header("Empty Save Settings")]
    [SerializeField, Tooltip("Use this if you don't have a 'create new' option. for fixed count save games")]
    private bool fillWithEmptyAutoSaves = false;
    [SerializeField, Tooltip("Use this if you don't have a 'create new' option. for fixed count save games")]
    private bool fillWithEmptySaves = true;
    [SerializeField] private int autoSaveCount = 4;
    [SerializeField] private int saveCount = 12;

    [Header("UI")]
    [SerializeField] private SnowUI grid;

    [Header("Debug")]
    [SerializeField] private List<LoadGameGridButton> buttons = new List<LoadGameGridButton>();

    private void OnEnable()
    {
        gameListChanged = Messages.Get<SaveGameListChangedMessage>();
        gameListChanged.AddListener(this.GameListChanged);
        this.GameListChanged();
    }
    private void OnDisable()
    {
        gameListChanged.RemoveListener(this.GameListChanged);
        gameListChanged = null;
        Messages.Return<SaveGameListChangedMessage>();
    }

    private void GameListChanged()
    {
        if (buttons.Count > 0)
        {
            foreach (var button in buttons)
            {
                Destroy(button.gameObject);
            }
        }
        buttons.Clear();

        Debug.Log("Before GetSaveList");
        var saves = SaveManager.GetSaveList();
        Debug.Log($"After GetSaveList: saves null={saves == null}");

        Debug.Log($"autoSaveLocations null={saves?.AutoSaveLocations == null}, " +
                  $"count={saves?.AutoSaveLocations?.Count}");
        Debug.Log($"saveLocations null={saves?.SaveLocations == null}, " +
                  $"count={saves?.SaveLocations?.Count}");


        //Sorry future me but this is complex
        if (MixAutoSaves)
        {
            Debug.Log($"Mixing Auto Saves!");
            var saveList = new List<SaveGameInfo>();
            //merge into 1 list
            //it doesn't matter the order yet
            Debug.Log($"UseAutoSaves={UseAutoSaves}, UseNormalSaves={UseNormalSaves}");
            if (UseAutoSaves)
            {
                saveList.AddRange(saves.AutoSaveLocations.Values);
            }
            if (UseNormalSaves)
            {
                saveList.AddRange(saves.SaveLocations.Values);
            }

            if (SortByDate)
            {
                //now sort the combined list
                saveList = saveList.OrderByDescending(x => x.DateModified).ToList();
            }
            else
            {
                //sort by the index, weird but this would go AutoSave0 then Save0 then AutoSave1 etc.
                saveList = saveList.OrderByDescending(x => x.SaveSlot).ToList();
            }
            //we CANNOT fill empty auto saves so the fill with empty literally cannot work
            if (fillWithEmptySaves || fillWithEmptyAutoSaves)
            {
                Debug.LogError($"Because you are mixing the AutoSaves in with normal saves, " +
                    $"we CANNOT fill with empty auto save buttons! so even though `fillWithEmpty` is true," +
                    $"The creation of the save buttons is FORCED to be false, if this is understod disable the" +
                    $"fillWithEmptySaves and fillWithEmptyAutoSaves flags when using 'MixAutoSaves' to stop this error");
            }

            this.CreateSaveButtons(saveList, false, SortByDate, autoSaveCount + saveCount, false);
        }
        else
        {
            Debug.Log($"Not Mixing Auto Saves!");
            //we're keeping the lists separate so wait and merge them after sorting by date or not
            if (UseAutoSaves)
            {
                Debug.Log($"Before auto Values; null={saves.AutoSaveLocations == null}, count={saves.AutoSaveLocations?.Count}");
                var autoSaves = saves.AutoSaveLocations.Values.ToList();
                Debug.Log("After auto Values");
                Debug.Log($"Creating {autoSaves.Count} Auto Save Buttons! Max: {autoSaveCount}");
                if (SortByDate)
                {
                    autoSaves = autoSaves.OrderByDescending(x => x.DateModified).ToList();
                }
                else
                {
                    autoSaves = autoSaves.OrderBy(x => x.SaveSlot).ToList();
                }
                Debug.Log($"Before CreateSaveButtons UseAutoSaves: count={autoSaves.Count}");
                this.CreateSaveButtons(autoSaves, fillWithEmptyAutoSaves, SortByDate, autoSaveCount, true);
                Debug.Log("After CreateSaveButtons");
            }

            if (UseNormalSaves)
            {
                var normalSaves = saves.SaveLocations.Values.ToList();
                Debug.Log($"Creating {normalSaves.Count} Auto Save Buttons! Max: {saveCount}");
                if (SortByDate)
                {
                    normalSaves = normalSaves.OrderByDescending(x => x.DateModified).ToList();
                }
                else
                {
                    normalSaves = normalSaves.OrderBy(x => x.SaveSlot).ToList();
                }
                Debug.Log($"Before CreateSaveButtons: count={normalSaves.Count}");
                this.CreateSaveButtons(normalSaves, fillWithEmptySaves, SortByDate, saveCount, false);
                Debug.Log("After CreateSaveButtons");

            }
        }
        Debug.Log("Before MarkDirtyLayout");
        grid.MarkDirtyLayout();
        Debug.Log("After MarkDirtyLayout");
    }


    public void CreateSaveButtons(List<SaveGameInfo> saveList, bool fillEmpty, bool sortByDate, int buttonMaxCount, bool isAutoSave)
    {
        for (var i = 0; i < buttonMaxCount; i++)
        {
            //make sure you don't index into the list

            SaveGameInfo foundSave = null;

            if (i < saveList.Count)
            {
                foundSave = saveList[i];
            }

            if (foundSave == null && fillEmpty)
            {
                //add an empty button if there isn't a save in the slot
                var newButton = this.MakeSaveButton(i, null, isAutoSave);
                buttons.Add(newButton);
            }
            else if (foundSave != null)
            {
                //add a button for the found save
                var newButton = this.MakeSaveButton(i, foundSave, isAutoSave);
                buttons.Add(newButton);
            }
            //if we're not filling with empty
            //or we didn't find a save don't add a button. 
        }
    }

    private LoadGameGridButton MakeSaveButton(int index, SaveGameInfo foundSave, bool isAutoSave = false)
    {
        //create a new button then parent it to us
        var newButton = Instantiate(LoadGameButtonPrefab);
        newButton.transform.SetParent(this.transform, false);

        //set it to no_save if the data is null
        if (foundSave == null)
        {
            newButton.name = $"{((!isAutoSave) ? "LoadSaveButton" : "LoadAutoSaveButton")}_{index}_EmptySave";
            newButton.NoSave(index, isAutoSave);
        }
        else
        {
            newButton.name = $"{((!isAutoSave) ? "LoadSaveButton" : "LoadAutoSaveButton")}_{index}_{foundSave.Name}";
            newButton.SetSaveData(foundSave);
        }
        return newButton;
    }

    private void Start()
    {
    }

    private void Update()
    {

    }
}
