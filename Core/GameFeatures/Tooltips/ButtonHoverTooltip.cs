using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    private bool hovering;
    private ShowTooltipMessage showMessage;

    [SerializeField] private TooltipData tooltipData;
    private void Start()
    {
        showMessage = Messages.Get<ShowTooltipMessage>();
    }
    private void OnDisable()
    {
        Messages.Return<ShowTooltipMessage>();
        showMessage = null;
    }
    // Update is called once per frame
    private Vector2 mousePosition;
    private void Update()
    {
        if (hovering)
        {
            tooltipData.position = mousePosition;
            showMessage?.Dispatch(tooltipData);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        mousePosition = eventData.position + (tooltipData.size * 0.5f);
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (hovering)
        {
            mousePosition = eventData.position + (tooltipData.size * 0.5f);
        }
    }
}
