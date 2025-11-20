using System.Collections.Generic;
using UnityEngine;

public class SaveGameListDisplay : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private RectTransform buttonContainer;
    [SerializeField] private float buttonHeight = 25.0f;
    private void Start()
    {
        var saveDataStruct = SaveManager.GetSaveList();

        buttonContainer.offsetMin = new Vector2(0, 0);
        buttonContainer.offsetMax = new Vector2(0, buttonHeight * saveDataStruct.saveLocations.Count);

        foreach (var kvp in saveDataStruct.saveLocations)
        {
            var go = Instantiate(buttonPrefab, buttonContainer, false);
            go.GetComponent<LoadGameDataFromSlotButton>().SetButtonInfo(kvp.Key, $"Save {kvp.Key}: {kvp.Value.name}");
        }
    }
}
