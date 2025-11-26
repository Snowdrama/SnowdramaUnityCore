using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveGame_LoadGameButton : MonoBehaviour, ISaveButton
{
    private SaveGameInfo saveGameInfo;

    [SerializeField] private Button LoadButton;
    [SerializeField] private Button DeleteButton;
    [SerializeField] private TMP_Text loadButtonText;

    [SerializeField] private float LoadButton_DisableTime = 0.0f;
    [SerializeField] private float LoadButtonCancel_DisableTime = 0.0f;

    [SerializeField] private float DeleteButton_DisableTime = 1.0f;
    [SerializeField] private float DeleteButtonCancel_DisableTime = 0.0f;

    [SerializeField] private float Error_DisableTime = 0.0f;
    private void Start()
    {
        LoadButton.onClick.AddListener(TryLoad);
        DeleteButton.onClick.AddListener(DeleteSave);
    }

    public void TryLoad()
    {
        if (SaveManager.CanLoadSave(saveGameInfo.saveSlot, saveGameInfo.isAutoSave))
        {
            Messages.GetOnce<OpenConfirmationModalMessage>().Dispatch(
                $"Would you like to load:\n{saveGameInfo.name}",
                new ModalButtonData()
                {
                    text = "Yes",
                    pressCallback = LoadGame,
                    disableTime = LoadButton_DisableTime,
                },
                new ModalButtonData()
                {
                    text = "No",
                    pressCallback = null,
                    disableTime = LoadButtonCancel_DisableTime,
                }
            );
        }
        else
        {
            Messages.GetOnce<OpenNoticeModalMessage>().Dispatch(
                $"Can't load, save doesn't exist",
                new ModalButtonData()
                {
                    text = "Ok",
                    pressCallback = null,
                    disableTime = Error_DisableTime,
                }
            );
        }
    }

    private void LoadGame()
    {
        SaveManager.LoadSave(saveGameInfo.saveSlot, saveGameInfo.isAutoSave);
    }


    private void DeleteSave()
    {
        Messages.GetOnce<OpenConfirmationModalMessage>().Dispatch(
            $"Are you sure you want to delete the save?\n{saveGameInfo.name}",
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
                disableTime = DeleteButtonCancel_DisableTime,
            }
        );
    }

    private void ForceDelete()
    {
        SaveManager.DeleteSaveGame(saveGameInfo.saveSlot, saveGameInfo.isAutoSave, true);
        Destroy(this.gameObject);
    }
    public void SetButtonInfo(SaveGameInfo saveGameInfo)
    {
        this.saveGameInfo = saveGameInfo;
        loadButtonText.text = $"{saveGameInfo.name} - {saveGameInfo.dateModified}";
    }
}
