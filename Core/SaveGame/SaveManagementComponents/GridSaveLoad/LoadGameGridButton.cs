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

    [Header("Safe Info")]
    [SerializeField] private Image saveImage;
    [SerializeField] private TMP_Text saveName;
    [SerializeField] private TMP_Text saveDate;

    [Header("Save Toggles")]
    [SerializeField] private GameObject SaveInfo;
    [SerializeField] private GameObject NoSaveInfo;
    private void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(this.OnClick);
    }

    private void OnClick()
    {
        SaveManager.LoadSave(currentSaveData.saveSlot, false);
    }

    public void SetSaveData(SaveGameInfo saveData)
    {
        currentSaveData = saveData;

        saveName.text = currentSaveData.name;

        //parse the date
        var date = DateTime.ParseExact(currentSaveData.dateModified, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
        if (date != null)
        {
            saveDate.text = date.ToString("MM/dd/yyyy hh:mm:ss tt");
        }

        if (File.Exists(currentSaveData.imagePath))
        {
            var imageBytes = File.ReadAllBytes(currentSaveData.imagePath);
            var tex = new Texture2D(2, 2);
            ImageConversion.LoadImage(tex, imageBytes);
            saveImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            saveImage.sprite = defaultSaveSprite;
        }
    }
    public void NoSave(int saveSlot)
    {
        saveImage.sprite = defaultSaveSprite;
        currentSaveData = new SaveGameInfo()
        {
            saveSlot = saveSlot,
            isAutoSave = false,
        };
        SaveInfo.gameObject.SetActive(false);
        NoSaveInfo.gameObject.SetActive(true);
    }
}
