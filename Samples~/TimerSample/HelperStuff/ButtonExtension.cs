using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonExtension : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent downAction;
    public UnityEvent upAction;

    private Button _button;
    public void OnPointerDown(PointerEventData eventData)
    {
        // ignore if button not interactable
        if (!_button.interactable) return;
        downAction.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // ignore if button not interactable
        if (!_button.interactable) return;
        upAction.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        _button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
