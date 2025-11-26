using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SaveGameListDisplay : MonoBehaviour
{
    [SerializeField] private bool AddSaves = true;
    [SerializeField] private bool AddAutosaves = false;
    [SerializeField] private GameObject saveButtonPrefab;
    [SerializeField] private RectTransform buttonContainer;
    [SerializeField] private float buttonHeight = 25.0f;
    private SaveGameListChangedMessage savesChanged;
    private void OnEnable()
    {
        savesChanged = Messages.Get<SaveGameListChangedMessage>();
        savesChanged.AddListener(LoadSaveList);

    }

    private void OnDisable()
    {
        savesChanged.RemoveListener(LoadSaveList);
        savesChanged = null;
        Messages.Return<SaveGameListChangedMessage>();
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
        List<KeyValuePair<int, SaveGameInfo>> saves = new List<KeyValuePair<int, SaveGameInfo>>();

        if (AddSaves)
        {
            foreach (var sd in saveDataStruct.saveLocations)
            {
                saves.Add(sd);
            }
        }

        if (AddAutosaves)
        {
            foreach (var sd in saveDataStruct.autoSaveLocations)
            {
                saves.Add(sd);
            }
        }

        buttonContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonHeight * saves.Count);
        Debug.Log($"Setting Height to: {buttonHeight * saves.Count}");

        saves = saves.OrderByDescending(x => x.Value.dateModified).ToList();


        foreach (var kvp in saves)
        {
            var go = Instantiate(saveButtonPrefab, buttonContainer, false);
            go.GetComponent<ISaveButton>().SetButtonInfo(kvp.Value);
            buttons.Add(go);
        }

        //scroll to the top
        this.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
    }
}


public interface ISaveButton
{
    void SetButtonInfo(SaveGameInfo saveGameInfo);
}