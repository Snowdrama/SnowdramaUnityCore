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
public class ModalNotice : MonoBehaviour
{
    [SerializeField] private GameObject modalPanel;
    [SerializeField] private TMP_Text modalText;
    [SerializeField] private Button okButton;
    [SerializeField] private TMP_Text okButtonText;

    private OpenNoticeModalMessage saveModalMessage;
    private ModalButtonData ok;

    private void OnEnable()
    {
        saveModalMessage = Messages.Get<OpenNoticeModalMessage>();
        saveModalMessage.AddListener(OpenModal);
    }

    private void OnDisable()
    {
        saveModalMessage.RemoveListener(OpenModal);
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
        modalPanel.SetActive(true);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        modalPanel.SetActive(false);
        okButton.onClick.AddListener(OkPressed);
    }

    private void CancelPressed()
    {
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
    }
}
