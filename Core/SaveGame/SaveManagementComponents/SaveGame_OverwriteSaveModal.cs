using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SaveGame_OverwriteSaveModalMessage : AMessage<int, string> { }
public class SaveGame_OverwriteSaveModal : MonoBehaviour
{
    [SerializeField] private GameObject SaveGamePanel;
    [SerializeField] private TMP_InputField SaveName;
    [SerializeField] private Button SaveButton;
    [SerializeField] private Button CancelButton;

    private int saveSlot = 0;
    private SaveGame_OverwriteSaveModalMessage openSavegameModal;
    private void OnEnable()
    {
        SaveButton.onClick.AddListener(SaveToExistingSlot);
        CancelButton.onClick.AddListener(CancelSave);

        openSavegameModal = Messages.Get<SaveGame_OverwriteSaveModalMessage>();
        openSavegameModal.AddListener(OpenSaveModal);
    }
    private void OnDisable()
    {
        SaveButton.onClick.RemoveListener(SaveToExistingSlot);
        CancelButton.onClick.AddListener(CancelSave);

        openSavegameModal.RemoveListener(OpenSaveModal);
        openSavegameModal = null;
        Messages.Return<SaveGame_OverwriteSaveModalMessage>();
    }

    private void OpenSaveModal(int saveSlot, string existingName)
    {
        this.saveSlot = saveSlot;
        if (!string.IsNullOrEmpty(existingName))
        {
            SaveName.text = existingName;
        }

        SaveGamePanel.SetActive(true);
    }

    private void SaveToExistingSlot()
    {
        if (!SaveManager.SaveGame(saveSlot, GameData.GetGameData(), false, SaveName.text))
        {
            Messages.GetOnce<OpenConfirmationModalMessage>().Dispatch(
                "Are you sure you want to override the save?",
                new ModalButtonData()
                {
                    text = "Yes",
                    pressCallback = ForceSave,
                    disableTime = 2.0f,
                },
                new ModalButtonData()
                {
                    text = "No",
                    pressCallback = CancelSave,
                    disableTime = 0.0f,
                }
            );
        }
    }

    public void ForceSave()
    {
        SaveManager.SaveGame(saveSlot, GameData.GetGameData(), true, SaveName.text);
        SaveGamePanel.SetActive(false);
    }
    public void CancelSave()
    {
        //do nothing XD
        SaveGamePanel.SetActive(false);
    }
}
