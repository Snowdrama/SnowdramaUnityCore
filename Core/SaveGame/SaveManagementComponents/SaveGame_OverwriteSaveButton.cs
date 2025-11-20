using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveGame_OverwriteSaveButton : MonoBehaviour, ISaveButton
{
    private int saveSlot;
    private string saveName;

    [SerializeField] private Button saveButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private TMP_Text saveButtonText;

    public void SetSaveInfo(int saveSlot, string saveName)
    {
        this.saveName = saveName;
        this.saveSlot = saveSlot;
        saveButtonText.text = saveName;
    }
    private void Start()
    {
        saveButton.onClick.AddListener(OpenModal);
        deleteButton.onClick.AddListener(DeleteSave);
    }

    private void OpenModal()
    {
        Messages.GetOnce<SaveGame_OverwriteSaveModalMessage>().Dispatch(saveSlot, saveName);
    }
    private void DeleteSave()
    {
        Messages.GetOnce<OpenConfirmationModalMessage>().Dispatch(
            $"Are you sure you want to delete the save?\n {saveName}",
            new ModalButtonData()
            {
                text = "Yes",
                pressCallback = ForceDelete,
                disableTime = 2.0f,
            },
            new ModalButtonData()
            {
                text = "No",
                pressCallback = null,
                disableTime = 0.0f,
            }
        );
    }

    private void ForceDelete()
    {
        SaveManager.DeleteSaveGame(saveSlot, true);
        Destroy(this.gameObject);
    }

    public void SetButtonInfo(int saveSlot, string saveName)
    {
        this.saveSlot = saveSlot;
        this.saveName = saveName;
    }
}
