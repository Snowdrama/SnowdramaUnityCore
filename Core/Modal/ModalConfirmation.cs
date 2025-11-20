using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenConfirmationModalMessage : AMessage<string, ModalButtonData, ModalButtonData> { }
public class OpenNoticeModalMessage : AMessage<string, ModalButtonData> { }
public struct ModalButtonData
{
    public string text;
    public Action pressCallback;
    public float disableTime;
}
/// <summary>
/// Confirmation Modals are for showing choices
/// to the player, like yes/no
/// 
/// Examples: 
/// "Are you sure you want to delete save 4? -> Yes -> No"
/// "Do you want to dismantle this item? -> Yes -> No"
/// "Are you sure you want to exit to the desktop? -> Keep Playing -> Exit Game"
/// </summary>
public class ModalConfirmation : MonoBehaviour
{
    [SerializeField] private GameObject modalPanel;
    [SerializeField] private TMP_Text modalText;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_Text cancelButtonText;
    [SerializeField] private Button okButton;
    [SerializeField] private TMP_Text okButtonText;

    private OpenConfirmationModalMessage saveModalMessage;
    private ModalButtonData ok;
    private ModalButtonData cancel;

    private void OnEnable()
    {
        saveModalMessage = Messages.Get<OpenConfirmationModalMessage>();
        saveModalMessage.AddListener(OpenModal);
    }

    private void OnDisable()
    {
        saveModalMessage.RemoveListener(OpenModal);
        saveModalMessage = null;
        Messages.Return<OpenConfirmationModalMessage>();
    }

    public void OpenModal(string setModalText, ModalButtonData okModalData, ModalButtonData cancelModalData)
    {

        modalText.text = setModalText;
        ok = okModalData;
        cancel = cancelModalData;


        if (ok.disableTime > 0)
        {
            okButton.interactable = false;
            okButtonText.text = $"{ok.text} ({ok.disableTime:F1})";
        }
        else
        {
            okButton.interactable = true;
            okButtonText.text = $"{ok.text}";
        }

        if (cancel.disableTime > 0)
        {
            cancelButton.interactable = false;
            cancelButtonText.text = $"{cancel.text} ({cancel.disableTime:F1})";
        }
        else
        {
            cancelButton.interactable = true;
            cancelButtonText.text = $"{cancel.text}";
        }
        modalPanel.SetActive(true);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        modalPanel.SetActive(false);
        cancelButton.onClick.AddListener(CancelPressed);
        okButton.onClick.AddListener(OkPressed);
    }

    private void CancelPressed()
    {
        cancel.pressCallback?.Invoke();
        modalPanel.SetActive(false);
    }

    private void OkPressed()
    {
        ok.pressCallback?.Invoke();
        modalPanel.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (ok.disableTime > 0)
        {
            ok.disableTime -= Time.deltaTime;
            okButton.interactable = false;
            okButtonText.text = $"{ok.text} ({ok.disableTime:F1})";
            if (ok.disableTime <= 0)
            {
                okButton.interactable = true;
                okButtonText.text = $"{ok.text}";
            }
        }

        if (cancel.disableTime > 0)
        {
            cancel.disableTime -= Time.deltaTime;
            cancelButton.interactable = false;
            cancelButtonText.text = $"{cancel.text} ({cancel.disableTime:F1})";
            if (cancel.disableTime <= 0)
            {
                cancelButton.interactable = true;
                cancelButtonText.text = $"{cancel.text}";
            }
        }
    }
}
