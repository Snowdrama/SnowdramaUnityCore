using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SaveGame_CreateNewSaveModalMessage : AMessage { }

[RequireComponent(typeof(CanvasGroup))]
public class ModalCreateNewSave : MonoBehaviour
{
    [SerializeField] private TMP_InputField SaveName;
    [SerializeField] private Button SaveButton;
    [SerializeField] private Button CancelButton;
    [SerializeField] private float GameSavedNotice_DisableTime = 0.0f;
    [SerializeField] private float Error_DisableTime = 0.0f;
    private CanvasGroup canvasGroup;
    private float currentAlpha;
    private float targetAlpha;
    private float currentAlphaVelocity;
    private SaveGame_CreateNewSaveModalMessage openSavegameModal;
    private void OnEnable()
    {
        SaveButton.onClick.AddListener(this.SaveToNewSlot);
        CancelButton.onClick.AddListener(this.Cancel);
        openSavegameModal = Messages.Get<SaveGame_CreateNewSaveModalMessage>();
        openSavegameModal.AddListener(this.OpenSaveModal);
        currentAlpha = targetAlpha = 0;
    }
    private void OnDisable()
    {
        SaveButton.onClick.RemoveListener(this.SaveToNewSlot);
        CancelButton.onClick.RemoveListener(this.Cancel);
        openSavegameModal.RemoveListener(this.OpenSaveModal);
        openSavegameModal = null;
        Messages.Return<SaveGame_CreateNewSaveModalMessage>();
    }

    private void OpenSaveModal()
    {
        SaveName.text = "";
        targetAlpha = 1;
    }

    private void SaveToNewSlot()
    {
        if (SaveManager.SaveGameToNewSlot(GameDataManager.GetGameData(), true, SaveName.text))
        {
            Messages.GetOnce<OpenNoticeModalMessage>().Dispatch(
                $"Game Saved!",
                new ModalButtonData()
                {
                    text = "Ok",
                    pressCallback = this.Complete,
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
                    pressCallback = this.Cancel,
                    disableTime = Error_DisableTime,
                }
            );
        }
    }

    private void Complete()
    {
        targetAlpha = 0;
    }

    private void Cancel()
    {
        targetAlpha = 0;
    }

    private void Update()
    {
        currentAlpha = Mathf.SmoothDamp(currentAlpha, targetAlpha, ref currentAlphaVelocity, 0.1f);
        canvasGroup.alpha = currentAlpha;
    }
}
