using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Snowdrama.UI
{
    [RequireComponent(typeof(Image))]
    public class SnowUISlider : Selectable, IDragHandler
    {
        float percentageFill = 1.0f;
        Vector2 cursorPosition = new Vector2();
        public SnowUISliderHandle cursor;
        [Header("Events")]
        public UnityEvent onValueChanged;
        protected override void Start()
        {
            FillImage();
        }

        public override void OnMove(AxisEventData eventData)
        {
            base.OnMove(eventData);
            if (this.interactable)
            {
                if (Mathf.Abs(eventData.moveVector.x) > 0.1f)
                {
                    percentageFill += eventData.moveVector.x * 0.1f;
                    percentageFill = Mathf.Clamp01(percentageFill);
                }
                FillImage();
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            UpdateFillFromMouse(eventData.position);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            UpdateFillFromMouse(eventData.position);
        }

        private void UpdateFillFromMouse(Vector2 position)
        {
            if (this.interactable)
            {
                var rectTransform = this.GetComponent<RectTransform>();
                var leftX = rectTransform.position.x + rectTransform.rect.xMin;
                var rightX = rectTransform.position.x + rectTransform.rect.xMax;

                var x = position.x - leftX;
                var max = rightX - leftX;

                var percent = x / max;
                percentageFill = Mathf.Clamp01(percent);
                FillImage();
            }
        }

        private void FillImage()
        {
            if (this.interactable)
            {
                var rectTransform = this.GetComponent<RectTransform>();
                rectTransform = this.GetComponent<RectTransform>();
                var leftX = rectTransform.position.x + rectTransform.rect.xMin;
                var offset = rectTransform.rect.width * percentageFill;
                cursorPosition.x = leftX + offset;
                cursorPosition.y = rectTransform.position.y;
                if (cursor != null)
                {
                    onValueChanged.Invoke();
                    cursor.GetComponent<RectTransform>().position = cursorPosition;
                }
                this.GetComponent<Image>().fillAmount = percentageFill;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (cursor != null)
            {
                cursor.SetColor(this.GetComponent<CanvasRenderer>().GetColor());
            }
        }

        public float GetValue()
        {
            return percentageFill;
        }

        public void SetValue(float value)
        {
            percentageFill = value;
            FillImage();
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