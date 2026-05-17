using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Confirmation Modals are for showing choices
/// to the player, like yes/no
/// 
/// Examples: 
/// "Are you sure you want to delete save 4? -> Yes -> No"
/// "Do you want to dismantle this item? -> Yes -> No"
/// "Are you sure you want to exit to the desktop? -> Keep Playing -> Exit Game"
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class ModalConfirmation : MonoBehaviour
{
    [SerializeField] private GameObject modalPanel;
    [SerializeField] private GameObject DarkBackground;
    [SerializeField] private TMP_Text modalText;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_Text cancelButtonText;
    [SerializeField] private Button okButton;
    [SerializeField] private TMP_Text okButtonText;

    private OpenConfirmationModalMessage saveModalMessage;
    private ModalButtonData ok;
    private ModalButtonData cancel;

    private float targetAlpha = 0.0f;
    private float currentAlpha = 0.0f;
    private CanvasGroup canvasGroup;
    private void Start()
    {
        cancelButton.onClick.AddListener(this.CancelPressed);
        okButton.onClick.AddListener(this.OkPressed);
        canvasGroup = this.GetComponent<CanvasGroup>();

        currentAlpha = targetAlpha = 0.0f;
        canvasGroup.alpha = 0.0f;
    }

    private void OnEnable()
    {
        saveModalMessage = Messages.Get<OpenConfirmationModalMessage>();
        saveModalMessage.AddListener(this.OpenModal);
    }

    private void OnDisable()
    {
        saveModalMessage.RemoveListener(this.OpenModal);
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
        targetAlpha = 1.0f;
    }


    private void CancelPressed()
    {
        cancel.pressCallback?.Invoke();
        targetAlpha = 0.0f;
    }

    private void OkPressed()
    {
        ok.pressCallback?.Invoke();
        targetAlpha = 0.0f;
    }

    private void Update()
    {
        if (ok.disableTime > 0)
        {
            ok.disableTime -= Time.unscaledDeltaTime;
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
            cancel.disableTime -= Time.unscaledDeltaTime;
            cancelButton.interactable = false;
            cancelButtonText.text = $"{cancel.text} ({cancel.disableTime:F1})";
            if (cancel.disableTime <= 0)
            {
                cancelButton.interactable = true;
                cancelButtonText.text = $"{cancel.text}";
            }
        }

        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime * 4.0f);
        canvasGroup.alpha = currentAlpha;
        if (canvasGroup.alpha > 0.2f)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
