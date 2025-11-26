using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SaveGame_CreateNewSaveModalMessage : AMessage { }
public class SaveGame_CreateNewSaveModal : MonoBehaviour
{
    [SerializeField] private GameObject SaveGamePanel;
    [SerializeField] private GameObject DarkBackground;
    [SerializeField] private TMP_InputField SaveName;
    [SerializeField] private Button SaveButton;
    [SerializeField] private Button CancelButton;
    [SerializeField] private float GameSavedNotice_DisableTime = 0.0f;
    [SerializeField] private float Error_DisableTime = 0.0f;

    private SaveGame_CreateNewSaveModalMessage openSavegameModal;
    private void OnEnable()
    {
        SaveButton.onClick.AddListener(SaveToNewSlot);
        CancelButton.onClick.AddListener(Cancel);
        openSavegameModal = Messages.Get<SaveGame_CreateNewSaveModalMessage>();
        openSavegameModal.AddListener(OpenSaveModal);
    }
    private void OnDisable()
    {
        SaveButton.onClick.RemoveListener(SaveToNewSlot);
        CancelButton.onClick.RemoveListener(Cancel);
        openSavegameModal.RemoveListener(OpenSaveModal);
        openSavegameModal = null;
        Messages.Return<SaveGame_CreateNewSaveModalMessage>();
    }

    private void OpenSaveModal()
    {
        SaveName.text = "";
        SaveGamePanel.SetActive(true);
        DarkBackground?.SetActive(true);
    }

    private void SaveToNewSlot()
    {
        if (SaveManager.SaveGameToNewSlot(GameData.GetGameData(), true, SaveName.text))
        {
            Messages.GetOnce<OpenNoticeModalMessage>().Dispatch(
                $"Game Saved!",
                new ModalButtonData()
                {
                    text = "Ok",
                    pressCallback = Complete,
                    disableTime = GameSavedNotice_DisableTime,
                }
            );
        }
        else
        {
            Messages.GetOnce<OpenNoticeModalMessage>().Dispatch(
                $"Error: Game was unable to be saved.",
                new ModalButtonData()
                {
                    text = "Ok",
                    pressCallback = Cancel,
                    disableTime = Error_DisableTime,
                }
            );
        }
    }

    private void Complete()
    {
        SaveGamePanel.SetActive(false);
        DarkBackground?.SetActive(false);
    }

    private void Cancel()
    {
        SaveGamePanel.SetActive(false);
        DarkBackground?.SetActive(false);
    }
}
