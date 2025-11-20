using UnityEngine;
using UnityEngine.UI;

public class SaveGameDataToSpecificSlotButton : MonoBehaviour
{
    [SerializeField] private int saveSlot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(Save);
    }

    public void Save()
    {
        if (!SaveManager.SaveGame(saveSlot, GameData.GetGameData()))
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
        SaveManager.SaveGame(saveSlot, GameData.GetGameData(), true);
    }
    public void CancelSave()
    {
        //do nothing XD
    }
}
