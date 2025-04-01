using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class VirtualCursorUIHoverComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    OnUIHoverStartSignal onUIHoverStartSignal;
    OnUIHoverEndSignal onUIHoverEndSignal;
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
        onUIHoverStartSignal = Signals.Get<OnUIHoverStartSignal>();
        onUIHoverEndSignal = Signals.Get<OnUIHoverEndSignal>();
    }

    private void OnDisable()
    {
        Signals.Return<OnUIHoverStartSignal>();
        Signals.Return<OnUIHoverEndSignal>();
    }
}
