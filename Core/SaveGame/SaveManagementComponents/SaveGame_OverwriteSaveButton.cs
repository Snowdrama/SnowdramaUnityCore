using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveGame_OverwriteSaveButton : MonoBehaviour, ISaveButton
{
    private SaveGameInfo saveGameInfo;

    [SerializeField] private Button SaveButton;
    [SerializeField] private TMP_Text saveButtonText;

    private void Start()
    {
        SaveButton.onClick.AddListener(this.OpenModal);
    }

    private void OpenModal()
    {
        Debug.Log($"<color=orange>Opening Overwrite Modal {saveGameInfo.saveSlot} - {saveGameInfo.name}");
        Messages.GetOnce<Modal_OverwriteSaveMessage>().Dispatch(saveGameInfo);
    }

    public void SetButtonInfo(SaveGameInfo saveInfo)
    {
        saveGameInfo = saveInfo;
        saveButtonText.text = $"{saveGameInfo.name} - {saveGameInfo.dateModified}";
    }
}
