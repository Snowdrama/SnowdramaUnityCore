using Snowdrama.UI;
using System;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameGridButton : MonoBehaviour
{
    private SaveGameInfo currentSaveData;

    [SerializeField] private Sprite defaultSaveSprite;

    [Header("Save Info")]
    [SerializeField] private Image saveImage;
    [SerializeField] private Image noSaveImage;
    [SerializeField] private TMP_Text saveNameTMP;
    [SerializeField] private TMP_Text saveDateTMP;
    [SerializeField] private TMP_Text noSaveTextTMP;

    [Header("Save Toggles")]
    [SerializeField] private GameObject SaveInfo;
    [SerializeField] private GameObject NoSaveInfo;

    [Header("Settings")]
    [SerializeField] private bool promptConfirmation = true;

    [Header("Router Stuff")]
    //this is here so when loading we can force close the menu
    public UIRouter containingMenu;

    [Header("Modal Text")]
    [SerializeField, TextArea, Tooltip("Use [SAVE_NAME] as the replacement for the name from the save file")]
    private string loadConfirmationText = "Are you sure\nyou want to load\n[SAVE_NAME]";
    [SerializeField, TextArea, Tooltip("Use [SAVE_NAME] as the replacement for the name from the save file")]
    private string deleteConfirmationText = "Are you sure\nyou want to delete\n[SAVE_NAME]";
    [SerializeField, TextArea, Tooltip("Use [SAVE_NAME] as the replacement for the name from the save file")]
    private string autoSaveDeleteError = "Can't Delete Auto Saves!";


    [Header("Confirmation Buttons")]
    private string loadConfirmation_Yes = "Yes";
    private string loadConfirmation_No = "No";
    private string deleteConfirmation_Yes = "Yes";
    private string deleteConfirmation_No = "No";
    private string autoSaveDeleteError_Ok = "Okay";

    [Header("Auto Save Text")]
    [SerializeField, TextArea, Tooltip("Use [INDEX] as the replacement so it will save 'Auto Save 0'")]
    private string autoSaveText = "Auto Save [INDEX]";
    [SerializeField, TextArea, Tooltip("Use [INDEX] as the replacement so it will save 'Auto Save 0'")]
    private string noAutoSaveText = "No Auto Save Data";
    [SerializeField, TextArea, Tooltip("Use [INDEX] as the replacement so it will save 'Auto Save 0'")]
    private string noSaveText = "No Save Data";

    private void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(this.OnClick);
    }

    private void OnClick()
    {
        if (promptConfirmation)
        {
            Messages.GetOnce<OpenConfirmationModalMessage>().Dispatch(
                loadConfirmationText.Replace("[SAVE_NAME]", currentSaveData.name),
                new ModalButtonData()
                {
                    text = loadConfirmation_Yes,
                    disableTime = 0.0f,
                    pressCallback = () =>
                    {
                        SaveManager.LoadSave(currentSaveData.saveSlot, false);
                        //force close the menu if we load something
                        containingMenu?.CloseAll();
                    }
                },
                new ModalButtonData()
                {
                    text = loadConfirmation_No,
                    disableTime = 0.0f,
                    pressCallback = null
                });
        }
        else
        {
            SaveManager.LoadSave(currentSaveData.saveSlot, false);
            //force close the menu if we load something
            containingMenu?.CloseAll();
        }
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
        currentSaveData = saveData;
        saveNameTMP.text = currentSaveData.name;

        //parse the date
        var date = DateTime.ParseExact(currentSaveData.dateModified, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
        if (date != null)
        {
            saveDateTMP.text = date.ToString("MM/dd/yyyy hh:mm:ss tt");
        }

        if (File.Exists(currentSaveData.imagePath))
        {
            var imageBytes = File.ReadAllBytes(currentSaveData.imagePath);
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
    public void NoSave(int saveSlot, bool isAutoSave)
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

        if (isAutoSave)
        {
            noSaveTextTMP.text = noAutoSaveText;
        }
        else
        {
            noSaveTextTMP.text = noSaveText;
        }
    }

}
