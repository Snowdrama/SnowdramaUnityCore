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

    [Header("Empty Save Settings")]
    [SerializeField, Tooltip("Use this if you don't have a 'create new' option. for fixed count save games")]
    private bool fillWithEmptySaves;
    [SerializeField] private int saveCount = 16;

    [Header("UI")]
    [SerializeField] private UIGridGroup grid;

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

        var saves = SaveManager.GetSaveList();

        var saveList = new List<SaveGameInfo>();
        if (UseNormalSaves)
        {
            saveList.AddRange(saves.saveLocations.Values);
        }

        if (UseAutoSaves)
        {
            saveList.AddRange(saves.autoSaveLocations.Values);
        }

        if (SortByDate)
        {
            saveList = saveList.OrderByDescending(x => x.dateModified).ToList();
        }

        for (var i = 0; i < saveCount; i++)
        {
            var foundSave = saveList.Find(x => x.saveSlot == i);

            if (fillWithEmptySaves)
            {
                //create a new button then parent it to us
                var newButton = Instantiate(LoadGameButtonPrefab);
                newButton.transform.SetParent(this.transform, false);

                if (foundSave != null)
                {
                    newButton.SetSaveData(foundSave);
                }
                else
                {
                    newButton.NoSave(i);
                }

                buttons.Add(newButton);
            }
            else if (foundSave != null)
            {
                var newButton = Instantiate(LoadGameButtonPrefab);
                newButton.transform.SetParent(this.transform, false);
                newButton.SetSaveData(foundSave);
                buttons.Add(newButton);
            }
        }
        grid.UpdateLayout();
    }

    private void Start()
    {
    }

    private void Update()
    {

    }
}
