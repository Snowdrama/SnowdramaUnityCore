using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class VirtualCursorUIHoverComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    OnUIHoverStartMessage onUIHoverStartSignal;
    OnUIHoverEndMessage onUIHoverEndSignal;
    public void OnPointerEnter(PointerEventData data)
    {
        onUIHoverStartSignal?.Dispatch();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        onUIHoverEndSignal?.Dispatch();
    }

    private void OnEnable()
    {
        onUIHoverStartSignal = Messages.Get<OnUIHoverStartMessage>();
        onUIHoverEndSignal = Messages.Get<OnUIHoverEndMessage>();
    }

    private void OnDisable()
    {
        Messages.Return<OnUIHoverStartMessage>();
        Messages.Return<OnUIHoverEndMessage>();
    }
}
