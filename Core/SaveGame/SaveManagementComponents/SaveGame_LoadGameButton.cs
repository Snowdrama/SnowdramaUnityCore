using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveGame_LoadGameButton : MonoBehaviour, ISaveButton
{
    private SaveGameInfo saveGameInfo;

    [SerializeField] private Button LoadButton;
    [SerializeField] private TMP_Text loadButtonText;

    [SerializeField] private float LoadButton_DisableTime = 0.0f;
    [SerializeField] private float LoadButtonCancel_DisableTime = 0.0f;

    [SerializeField] private float Error_DisableTime = 0.0f;
    private void Start()
    {
        LoadButton.onClick.AddListener(this.TryLoad);
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
                    pressCallback = this.LoadGame,
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

    public void SetButtonInfo(SaveGameInfo saveGameInfo)
    {
        this.saveGameInfo = saveGameInfo;
        loadButtonText.text = $"{saveGameInfo.name} - {saveGameInfo.dateModified}";
    }
}
