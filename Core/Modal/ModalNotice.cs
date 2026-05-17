using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Notice Modals are for showing information
/// to the player but don't have a choice
/// 
/// Examples: 
/// "Save in slot 4 doesn't exist -> Ok"
/// "Can't delete last object -> Ok"
/// "Needs a key to open -> Ok"
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class ModalNotice : MonoBehaviour
{
    [SerializeField] private GameObject modalPanel;
    [SerializeField] private GameObject DarkBackground;
    [SerializeField] private TMP_Text modalText;
    [SerializeField] private Button okButton;
    [SerializeField] private TMP_Text okButtonText;

    private OpenNoticeModalMessage saveModalMessage;
    private ModalButtonData ok;

    private float targetAlpha = 0.0f;
    private float currentAlpha = 0.0f;
    private CanvasGroup canvasGroup;
    private void Start()
    {
        okButton.onClick.AddListener(this.OkPressed);
        canvasGroup = this.GetComponent<CanvasGroup>();

        currentAlpha = targetAlpha = 0.0f;
        canvasGroup.alpha = 0.0f;
    }

    private void OnEnable()
    {
        saveModalMessage = Messages.Get<OpenNoticeModalMessage>();
        saveModalMessage.AddListener(this.OpenModal);
    }

    private void OnDisable()
    {
        saveModalMessage.RemoveListener(this.OpenModal);
        saveModalMessage = null;
        Messages.Return<OpenNoticeModalMessage>();
    }

    public void OpenModal(string setModalText, ModalButtonData okModalData)
    {

        modalText.text = setModalText;
        ok = okModalData;

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
        targetAlpha = 1.0f;
    }
    private void OkPressed()
    {
        ok.pressCallback?.Invoke();
        targetAlpha = 0.0f;
    }

    // Update is called once per frame
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
