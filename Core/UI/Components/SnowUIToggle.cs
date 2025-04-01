using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Snowdrama.UI
{
    public enum SnowUIToggleType
    {
        SetActiveType,
        SwitchImageType
    }

    [RequireComponent(typeof(Image))]
    public class SnowUIToggle : Selectable, IPointerDownHandler, ISubmitHandler
    {
        public bool isOn;
        public Image checkmark;
        public List<Selectable> thingsToDisable;

        public SnowUIToggleType toggleType;

        public Sprite onSprite;
        public Sprite offSprite;

        public UnityEvent<bool> onToggle;

        protected override void Start()
        {
            UpdateItems();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            Toggle();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            Toggle();
        }

        public void Toggle()
        {
            isOn = !isOn;
            UpdateItems();
        }

        public void UpdateItems()
        {
            onToggle.Invoke(isOn);
            if (isOn)
            {
                switch (toggleType)
                {
                    case SnowUIToggleType.SetActiveType:
                        checkmark.gameObject.SetActive(true);
                        break;
                    case SnowUIToggleType.SwitchImageType:
                        checkmark.sprite = onSprite;
                        break;
                }
                foreach (var item in thingsToDisable)
                {
                    if (item != null)
                    {
                        item.interactable = true;
                    }
                }
            }
            else
            {
                switch (toggleType)
                {
                    case SnowUIToggleType.SetActiveType:
                        checkmark.gameObject.SetActive(false);
                        break;
                    case SnowUIToggleType.SwitchImageType:
                        checkmark.sprite = offSprite;
                        break;
                }
                foreach (var item in thingsToDisable)
                {
                    if (item != null)
                    {
                        item.interactable = false;
                    }
                }
            }
        }
        public void SetValue(bool value)
        {
            isOn = value;
            UpdateItems();
        }

        public bool GetValue()
        {
            return isOn;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (image == null)
            {
                image = this.GetComponent<Image>();
            }
        }
#endif
    }
}