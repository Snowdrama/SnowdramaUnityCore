using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ModalSaveGame : MonoBehaviour
{
    [SerializeField] private TMP_InputField SaveName;
    [SerializeField] private Button SaveButton;
    [SerializeField] private Button CancelButton;

    [Header("Button Disable Times")]
    [SerializeField] private float GameSavedNotice_DisableTime = 0.0f;
    [SerializeField] private float Error_DisableTime = 0.0f;
    [SerializeField] private float OverwriteSave_ButtonDisableTime = 1.0f;
    [SerializeField] private float CancelButton_DisableTime = 0.0f;

    private CanvasGroup canvasGroup;
    private float currentAlpha;
    private float targetAlpha;
    private float currentAlphaVelocity;
    private Modal_SaveGameMessage openSavegameModal;
    private void OnEnable()
    {
        SaveButton.onClick.AddListener(this.SaveToNewSlot);
        CancelButton.onClick.AddListener(this.Cancel);
        openSavegameModal = Messages.Get<Modal_SaveGameMessage>();
        openSavegameModal.AddListener(this.OpenSaveModal);
        currentAlpha = targetAlpha = 0;
        canvasGroup = this.GetComponent<CanvasGroup>();
    }
    private void OnDisable()
    {
        SaveButton.onClick.RemoveListener(this.SaveToNewSlot);
        CancelButton.onClick.RemoveListener(this.Cancel);
        openSavegameModal.RemoveListener(this.OpenSaveModal);
        openSavegameModal = null;
        Messages.Return<Modal_SaveGameMessage>();
    }
    private int saveSlot = -1;
    private string saveName = "";
    private void OpenSaveModal(int saveSlot, string saveName)
    {
        this.saveSlot = saveSlot;
        this.saveName = saveName;
        SaveName.text = saveName;
        targetAlpha = 1;
    }

    private void SaveToNewSlot()
    {
        //we're making a new save that doesn't need a specific slot
        if (saveSlot == -1)
        {
            //so get us an unused one
            saveSlot = SaveManager.GetUnusedSaveSlot();
        }

        //do we have a save?
        if (SaveManager.HasSaveGame(saveSlot, false))
        {
            Messages.GetOnce<OpenConfirmationModalMessage>().Dispatch(
                "Are you sure you want to override the save?",
                new ModalButtonData()
                {
                    text = "Yes",
                    pressCallback = this.ForceSave,
                    disableTime = OverwriteSave_ButtonDisableTime,
                },
                new ModalButtonData()
                {
                    text = "No",
                    pressCallback = this.Cancel,
                    disableTime = CancelButton_DisableTime,
                }
            );
        }
        else
        {
            //There's no save in the slot

            //Force saving the save
            if (SaveManager.SaveGame(saveSlot, GameDataManager.GetGameData(), true, SaveName.text))
            {
                //then show a game saved modal
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
                //technically this should never trigger XD
                //but you know... for safety
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

    }

    private void Complete()
    {
        targetAlpha = 0;
    }

    private void Cancel()
    {
        targetAlpha = 0;
    }

    public void ForceSave()
    {
        Debug.Log($"Writing Save: {saveName} to Save Slot: {saveSlot}");
        if (SaveScreenshotHelper.lastScreenshot != null)
        {
            SaveManager.SaveGame(saveSlot, GameDataManager.GetGameData(), true, SaveName.text, Application.version, SaveScreenshotHelper.lastScreenshot);
        }
        else
        {

            SaveManager.SaveGame(saveSlot, GameDataManager.GetGameData(), true, SaveName.text, Application.version);
        }
        targetAlpha = 0;
    }

    private void Update()
    {
        currentAlpha = Mathf.SmoothDamp(currentAlpha, targetAlpha, ref currentAlphaVelocity, 0.1f);
        canvasGroup.alpha = currentAlpha;
        if (canvasGroup.alpha > 0.2f)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
