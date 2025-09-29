using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Snowdrama.UI
{
    [Serializable] public class ClickEvent : UnityEvent { }

    public class SnowUIButton : Selectable, IPointerClickHandler, IEventSystemHandler, ISubmitHandler
    {
        public Image highlightImage;

        [SerializeField] public ClickEvent onClick;

        public bool selectIfNoActiveObjects;
        EventSystem eventSystem;
        protected override void Start()
        {
            highlightImage?.gameObject?.SetActive(false);
        }

        private void Update()
        {
            if (eventSystem == null)
            {
                eventSystem = EventSystem.current;
            }

            if (eventSystem != null)
            {
                if (eventSystem.currentSelectedGameObject == null)
                {
                    if (selectIfNoActiveObjects)
                    {
                        eventSystem.SetSelectedGameObject(this.gameObject);
                    }
                }
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            highlightImage?.gameObject?.SetActive(true);
            Select();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            highlightImage?.gameObject?.SetActive(false);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            highlightImage?.gameObject?.SetActive(true);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            highlightImage?.gameObject?.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            onClick?.Invoke();
        }

        [ContextMenu("Do Something")]
        public void Test()
        {
            Debug.LogError("Test");
        }

        public enum SomeEnum
        {
            One,
            Two,
            Three
        }

        private void ActualMethod(SomeEnum argument)
        {
            // Implement method
        }

        [EnumAction(typeof(SomeEnum))]
        public void SomeMethod(int argument)
        {
            ActualMethod((SomeEnum)argument);
        }
    }

}