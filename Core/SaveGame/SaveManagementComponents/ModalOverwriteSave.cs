using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SaveGame_OverwriteSaveModalMessage : AMessage<SaveGameInfo> { }

[RequireComponent(typeof(CanvasGroup))]
public class ModalOverwriteSave : MonoBehaviour
{
    [SerializeField] private TMP_InputField SaveName;
    [SerializeField] private Button SaveButton;
    [SerializeField] private float SaveButton_DisableTime = 1.0f;
    [SerializeField] private Button CancelButton;
    [SerializeField] private float CancelButton_DisableTime = 0.0f;

    private CanvasGroup canvasGroup;
    private float currentAlpha;
    private float targetAlpha;
    private float currentAlphaVelocity;
    private SaveGameInfo saveGameInfo;
    private SaveGame_OverwriteSaveModalMessage openSavegameModal;
    private void OnEnable()
    {
        SaveButton.onClick.AddListener(this.SaveToExistingSlot);
        CancelButton.onClick.AddListener(this.CancelSave);

        openSavegameModal = Messages.Get<SaveGame_OverwriteSaveModalMessage>();
        openSavegameModal.AddListener(this.OpenSaveModal);
        canvasGroup = this.GetComponent<CanvasGroup>();
    }
    private void OnDisable()
    {
        SaveButton.onClick.RemoveListener(this.SaveToExistingSlot);
        CancelButton.onClick.AddListener(this.CancelSave);

        openSavegameModal.RemoveListener(this.OpenSaveModal);
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

        targetAlpha = 1;
    }

    private void SaveToExistingSlot()
    {
        //TODO Check if we need to do this, I think no
        //if (!SaveManager.SaveGame(saveGameInfo.saveSlot, GameDataManager.GetGameData(), false, SaveName.text))
        //{
        //}

        Debug.Log($"Prepping to overwrite save");
        //open the panel
        targetAlpha = 0;
        //dispatch the confirm modal message
        Messages.GetOnce<OpenConfirmationModalMessage>().Dispatch(
            "Are you sure you want to override the save?",
            new ModalButtonData()
            {
                text = "Yes",
                pressCallback = this.ForceSave,
                disableTime = SaveButton_DisableTime,
            },
            new ModalButtonData()
            {
                text = "No",
                pressCallback = this.CancelSave,
                disableTime = CancelButton_DisableTime,
            }
        );
    }

    public void ForceSave()
    {
        Debug.Log($"Writing Save: {SaveName.text} to Save Slot: {saveGameInfo.saveSlot}");
        SaveManager.SaveGame(saveGameInfo.saveSlot, GameDataManager.GetGameData(), true, SaveName.text);
        targetAlpha = 0;
    }
    public void CancelSave()
    {
        //do nothing XD
        Debug.Log($"Canceling Overwriting Save");
        targetAlpha = 0;
    }
    private void Update()
    {
        currentAlpha = Mathf.SmoothDamp(currentAlpha, targetAlpha, ref currentAlphaVelocity, 0.1f);
        canvasGroup.alpha = currentAlpha;
    }
}
