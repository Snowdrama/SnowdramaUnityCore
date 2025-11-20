using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveGame_LoadGameButton : MonoBehaviour, ISaveButton
{
    private int saveSlot;
    private string saveName;

    [SerializeField] private Button loadButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private TMP_Text loadButtonText;
    private void Start()
    {
        loadButton.onClick.AddListener(TryLoad);
        deleteButton.onClick.AddListener(DeleteSave);
    }

    public void TryLoad()
    {
        if (SaveManager.CanLoadSave(saveSlot))
        {
            Messages.GetOnce<OpenConfirmationModalMessage>().Dispatch(
                $"Would you like to load:\n{saveName}",
                new ModalButtonData()
                {
                    text = "Yes",
                    pressCallback = LoadGame,
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
        else
        {
            Messages.GetOnce<OpenNoticeModalMessage>().Dispatch(
                $"Can't load, save doesn't exist",
                new ModalButtonData()
                {
                    text = "Ok",
                    pressCallback = null,
                    disableTime = 2.0f,
                }
            );
        }
    }

    private void LoadGame()
    {
        SaveManager.LoadSave(saveSlot);
    }


    private void DeleteSave()
    {
        Messages.GetOnce<OpenConfirmationModalMessage>().Dispatch(
            $"Are you sure you want to delete the save?\n{saveName}",
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
        loadButtonText.text = saveName;
    }
}
