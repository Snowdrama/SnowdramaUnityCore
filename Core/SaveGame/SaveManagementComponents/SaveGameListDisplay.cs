using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SaveGameListDisplay : MonoBehaviour
{
    [SerializeField] private bool AddSaves = true;
    [SerializeField] private bool AddAutosaves = false;
    [SerializeField] private GameObject saveButtonPrefab;
    [SerializeField] private GameObject autoSaveButtonPrefab;
    [SerializeField] private RectTransform buttonContainer;
    [SerializeField] private float buttonHeight = 25.0f;
    private SaveGameListChanged savesChanged;
    private void OnEnable()
    {
        savesChanged = Messages.Get<SaveGameListChanged>();
        savesChanged.AddListener(LoadSaveList);

    }

    private void OnDisable()
    {
        savesChanged.RemoveListener(LoadSaveList);
        savesChanged = null;
        Messages.Return<SaveGameListChanged>();
    }

    private List<GameObject> buttons = new List<GameObject>();

    private void Start()
    {
        LoadSaveList();
    }

    private void Update()
    {
    }
    private void LoadSaveList()
    {

        for (int i = 0; i < buttons.Count; i++)
        {
            Destroy(buttons[i]);
        }
        buttons.Clear();

        var saveDataStruct = SaveManager.GetSaveList();
        int saveCount = 0;

        if (AddSaves)
        {
            saveCount += saveDataStruct.saveLocations.Count;
        }

        if (AddAutosaves)
        {
            saveCount += saveDataStruct.autoSaveLocations.Count;
        }

        buttonContainer.offsetMin = new Vector2(0, 0);
        buttonContainer.offsetMax = new Vector2(0, buttonHeight * saveCount);


        foreach (var kvp in saveDataStruct.autoSaveLocations.OrderByDescending(x => x.Value.dateModified))
        {
            var go = Instantiate(autoSaveButtonPrefab, buttonContainer, false);
            go.GetComponent<ISaveButton>().SetButtonInfo(kvp.Key, $"{kvp.Value.name} - {kvp.Value.dateModified}");
            buttons.Add(go);
        }


        foreach (var kvp in saveDataStruct.saveLocations.OrderByDescending(x => x.Value.dateModified))
        {
            var go = Instantiate(saveButtonPrefab, buttonContainer, false);
            go.GetComponent<ISaveButton>().SetButtonInfo(kvp.Key, $"{kvp.Value.name} - {kvp.Value.dateModified}");
            buttons.Add(go);
        }

        this.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
    }
}


public interface ISaveButton
{
    void SetButtonInfo(int saveSlot, string saveName);
}