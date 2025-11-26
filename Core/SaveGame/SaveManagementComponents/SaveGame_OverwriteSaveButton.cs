using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveGame_OverwriteSaveButton : MonoBehaviour, ISaveButton
{
    private SaveGameInfo saveGameInfo;

    [SerializeField] private Button SaveButton;
    [SerializeField] private Button DeleteButton;
    [SerializeField] private TMP_Text saveButtonText;
    [SerializeField] private float DeleteButton_DisableTime = 1.0f;
    [SerializeField] private float DeleteCancelButton_DisableTime = 0.0f;

    private void Start()
    {
        SaveButton.onClick.AddListener(OpenModal);
        DeleteButton.onClick.AddListener(DeleteSave);
    }

    private void OpenModal()
    {
        Messages.GetOnce<SaveGame_OverwriteSaveModalMessage>().Dispatch(saveGameInfo);
    }
    private void DeleteSave()
    {
        Messages.GetOnce<OpenConfirmationModalMessage>().Dispatch(
            $"Are you sure you want to delete the save?\n {saveGameInfo.name}",
            new ModalButtonData()
            {
                text = "Yes",
                pressCallback = ForceDelete,
                disableTime = DeleteButton_DisableTime,
            },
            new ModalButtonData()
            {
                text = "No",
                pressCallback = null,
                disableTime = DeleteCancelButton_DisableTime,
            }
        );
    }

    private void ForceDelete()
    {
        SaveManager.DeleteSaveGame(saveGameInfo.saveSlot, saveGameInfo.isAutoSave, true);
        Destroy(this.gameObject);
    }

    public void SetButtonInfo(SaveGameInfo saveInfo)
    {

        this.saveGameInfo = saveInfo;
        saveButtonText.text = $"{saveGameInfo.name} - {saveGameInfo.dateModified}";
    }
}
