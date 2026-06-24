using Snowdrama.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ModalWindow : MonoBehaviour
{
    [SerializeField] private GameObject[] _toggleWhenActive;
    [SerializeField] private bool _startEnabled = false;

    private CanvasGroup canvasGroup;
    [SerializeField] private float showHideTime = 0.25f;

    private bool _modalActive;
    private bool _elementsActive;
    private bool ModalActive
    {
        get { return _modalActive; }
        set
        {
            _modalActive = value;
            if (_modalActive)
            {
                targetAlpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
            else
            {
                targetAlpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
    }
    private bool ElementsActive
    {
        get { return _elementsActive; }
        set
        {
            if (_elementsActive != value)
            {
                _elementsActive = value;
                if (_elementsActive)
                {
                    foreach (var item in _toggleWhenActive)
                    {
                        item.gameObject.SetActive(true);
                    }
                }
                else
                {
                    foreach (var item in _toggleWhenActive)
                    {
                        item.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    [Header("Debug")]
    [SerializeField] private float currentAlpha;
    [SerializeField] private float targetAlpha;
    [SerializeField] private float currentAlphaVelocity;

    private void Start()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();

        if (_startEnabled)
        {
            this.ModalActive = true;
        }
        else
        {
            this.ModalActive = false;
        }
    }

    public void SetModalState(bool activeState)
    {
        this.ModalActive = activeState;
    }

    public void CloseModal()
    {
        this.SetModalState(false);
    }

    public void OpenModal()
    {
        this.SetModalState(true);
    }
    public void ToggleModal()
    {
        this.SetModalState(!this.ModalActive);
    }
    private void Update()
    {
        currentAlpha = Mathf.SmoothDamp(currentAlpha, targetAlpha, ref currentAlphaVelocity, showHideTime, Mathf.Infinity, Time.unscaledDeltaTime);
        canvasGroup.alpha = currentAlpha;

        if (currentAlpha <= 0.05f)
        {
            this.ElementsActive = false;
        }
        else
        {
            this.ElementsActive = true;
        }
        currentAlpha = currentAlpha.Clamp(0, 1);
    }
}
