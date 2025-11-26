using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SaveGame_OverwriteSaveModalMessage : AMessage<SaveGameInfo> { }
public class SaveGame_OverwriteSaveModal : MonoBehaviour
{
    [SerializeField] private GameObject SaveGamePanel;
    [SerializeField] private GameObject DarkBackground;
    [SerializeField] private TMP_InputField SaveName;
    [SerializeField] private Button SaveButton;
    [SerializeField] private float SaveButton_DisableTime = 1.0f;
    [SerializeField] private Button CancelButton;
    [SerializeField] private float CancelButton_DisableTime = 0.0f;

    private SaveGameInfo saveGameInfo;
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

    private void OpenSaveModal(SaveGameInfo saveGameInfo)
    {
        this.saveGameInfo = saveGameInfo;
        if (!string.IsNullOrEmpty(saveGameInfo.name))
        {
            SaveName.text = saveGameInfo.name;
        }

        SaveGamePanel.SetActive(true);
        DarkBackground?.SetActive(true);
    }

    private void SaveToExistingSlot()
    {
        if (!SaveManager.SaveGame(saveGameInfo.saveSlot, GameData.GetGameData(), false, SaveName.text))
        {
            Messages.GetOnce<OpenConfirmationModalMessage>().Dispatch(
                "Are you sure you want to override the save?",
                new ModalButtonData()
                {
                    text = "Yes",
                    pressCallback = ForceSave,
                    disableTime = SaveButton_DisableTime,
                },
                new ModalButtonData()
                {
                    text = "No",
                    pressCallback = CancelSave,
                    disableTime = CancelButton_DisableTime,
                }
            );
        }
    }

    public void ForceSave()
    {
        SaveManager.SaveGame(saveGameInfo.saveSlot, GameData.GetGameData(), true, SaveName.text);
        SaveGamePanel.SetActive(false);
        DarkBackground?.SetActive(false);
    }
    public void CancelSave()
    {
        //do nothing XD
        SaveGamePanel.SetActive(false);
        DarkBackground?.SetActive(false);
    }
}
