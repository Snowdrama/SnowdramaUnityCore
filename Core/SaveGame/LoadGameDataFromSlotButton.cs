using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameDataFromSlotButton : MonoBehaviour
{
    [SerializeField] private TMP_Text saveNameText;
    [SerializeField] private int saveSlot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(Save);
    }

    public void SetButtonInfo(int saveSlotId, string saveName)
    {
        saveSlot = saveSlotId;
        saveNameText.text = saveName;
    }

    public void Save()
    {
        if (SaveManager.CanLoadSave(saveSlot))
        {
            Messages.GetOnce<OpenConfirmationModalMessage>().Dispatch(
                $"Would you like to load Save {saveSlot}",
                new ModalButtonData()
                {
                    text = "Yes",
                    pressCallback = LoadGame,
                    disableTime = 2.0f,
                },
                new ModalButtonData()
                {
                    text = "No",
                    pressCallback = CancelLoad,
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
                    pressCallback = CancelLoad,
                    disableTime = 2.0f,
                }
            );
        }
    }

    public void LoadGame()
    {
        SaveManager.LoadSave(saveSlot);
    }
    public void CancelLoad()
    {
        //do nothing XD
    }

    // Update is called once per frame
    private void Update()
    {

    }
}
