using System.Collections.Generic;
using UnityEngine;

public class SaveGameListDisplay : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
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

    private void LoadSaveList()
    {

        for (int i = 0; i < buttons.Count; i++)
        {
            Destroy(buttons[i]);
        }
        buttons.Clear();


        var saveDataStruct = SaveManager.GetSaveList();

        buttonContainer.offsetMin = new Vector2(0, 0);
        buttonContainer.offsetMax = new Vector2(0, buttonHeight * saveDataStruct.saveLocations.Count);

        foreach (var kvp in saveDataStruct.saveLocations)
        {
            var go = Instantiate(buttonPrefab, buttonContainer, false);
            go.GetComponent<ISaveButton>().SetButtonInfo(kvp.Key, $"{kvp.Value.name}");
            buttons.Add(go);
        }
    }
}


public interface ISaveButton
{
    void SetButtonInfo(int saveSlot, string saveName);
}