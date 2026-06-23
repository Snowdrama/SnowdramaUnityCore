using UnityEngine;
using UnityEngine.UI;

public class SaveGame_DeleteSaveButton : MonoBehaviour, ISaveButton
{
    private SaveGameInfo saveGameInfo;
    [SerializeField] private Button DeleteButton;
    [SerializeField] private float DeleteButton_DisableTime = 1.0f;
    [SerializeField] private float DeleteCancelButton_DisableTime = 0.0f;

    private void Start()
    {
        DeleteButton.onClick.AddListener(this.DeleteSave);
    }

    public void SetButtonInfo(SaveGameInfo saveInfo)
    {
        saveGameInfo = saveInfo;
    }
    private void DeleteSave()
    {
        Debug.Log($"<color=red>Prepping to delete {saveGameInfo.saveSlot} - {saveGameInfo.name}");
        Messages.GetOnce<OpenConfirmationModalMessage>().Dispatch(
            $"Are you sure you want to delete the save?\n {saveGameInfo.name}",
            new ModalButtonData()
            {
                text = "Yes",
                pressCallback = this.ForceDelete,
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
        Debug.Log($"<color=red>Force Deleting Save {saveGameInfo.saveSlot} - {saveGameInfo.name}");
        SaveManager.DeleteSaveGame(saveGameInfo.saveSlot, saveGameInfo.isAutoSave, true);
        Destroy(this.gameObject);
    }
}
