using Snowdrama.UI;
using System;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveGameGridButton : MonoBehaviour
{
    private SaveGameInfo currentSaveData;
    [SerializeField] private Sprite defaultSaveSprite;
    [Header("Safe Info")]
    [SerializeField] private Image saveImage;
    [SerializeField] private Image noSaveImage;
    [SerializeField] private TMP_Text saveNameTMP;
    [SerializeField] private TMP_Text saveDateTMP;

    [Header("Save Toggles")]
    [SerializeField] private GameObject SaveInfo;
    [SerializeField] private GameObject NoSaveInfo;

    [Header("Router Stuff")]
    //this is here so when loading we can force close the menu
    public UIRouter containingMenu;

    [Header("Modal Text")]
    [SerializeField, TextArea, Tooltip("Use [SAVE_NAME] as the replacement for the name from the save file")]
    private string deleteConfirmationText = "Are you sure\nyou want to delete\n[SAVE_NAME]";
    [SerializeField, TextArea, Tooltip("Use [SAVE_NAME] as the replacement for the name from the save file")]
    private string autoSaveDeleteError = "Can't Delete Auto Saves!";


    [Header("Confirmation Buttons")]
    private string deleteConfirmation_Yes = "Yes";
    private string deleteConfirmation_No = "No";
    private string autoSaveDeleteError_Ok = "Okay";

    private void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(this.OnClick);

    }
    private void OnClick()
    {
        //trigger save game
        Debug.Log($"Trying to open the save modal!");
        Messages.GetOnce<Modal_SaveGameMessage>().Dispatch(currentSaveData.saveSlot, currentSaveData.name);
    }

    public void OnDeleteSave()
    {
        if (currentSaveData.isAutoSave)
        {
            Messages.GetOnce<OpenNoticeModalMessage>().Dispatch(
                autoSaveDeleteError.Replace("[SAVE_NAME]", currentSaveData.name),
                new ModalButtonData()
                {
                    text = autoSaveDeleteError_Ok,
                    disableTime = 0.0f,
                    pressCallback = () =>
                    {
                    }
                });
        }
        else
        {
            Messages.GetOnce<OpenConfirmationModalMessage>().Dispatch(
                $"Are you sure you\nwant to Delete\n\"{currentSaveData.name}\"?",
                new ModalButtonData()
                {
                    text = deleteConfirmation_Yes,
                    disableTime = 1.0f,
                    pressCallback = () =>
                    {
                        //force delete the save
                        SaveManager.DeleteSaveGame(currentSaveData.saveSlot, true);
                    }
                },
                new ModalButtonData()
                {
                    text = deleteConfirmation_No,
                    disableTime = 0.0f,
                    pressCallback = null
                });
        }
    }

    public void SetSaveData(SaveGameInfo saveData)
    {
        SaveInfo.gameObject.SetActive(true);
        NoSaveInfo.gameObject.SetActive(false);

        currentSaveData = saveData;

        saveNameTMP.text = saveData.name;

        //parse the date
        var date = DateTime.ParseExact(saveData.dateModified, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
        if (date != null)
        {
            saveDateTMP.text = date.ToString("MM/dd/yyyy hh:mm:ss tt");
        }

        if (File.Exists(saveData.imagePath))
        {
            var imageBytes = File.ReadAllBytes(saveData.imagePath);
            var tex = new Texture2D(2, 2);
            ImageConversion.LoadImage(tex, imageBytes);
            saveImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            noSaveImage.sprite = defaultSaveSprite;
        }
        else
        {
            saveImage.sprite = defaultSaveSprite;
            noSaveImage.sprite = defaultSaveSprite;
        }
    }
    public void NoSave(int saveSlot)
    {
        saveImage.sprite = defaultSaveSprite;
        noSaveImage.sprite = defaultSaveSprite;
        currentSaveData = new SaveGameInfo()
        {
            saveSlot = saveSlot,
            isAutoSave = false,
        };
        SaveInfo.gameObject.SetActive(false);
        NoSaveInfo.gameObject.SetActive(true);
    }
}
