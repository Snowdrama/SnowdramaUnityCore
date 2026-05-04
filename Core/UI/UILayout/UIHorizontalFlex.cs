using Snowdrama.UI;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Snowdrama.UI
{
    /// <summary>
    /// This class fits all the children into horizontal columns based on some size.
    /// </summary>
    [ExecuteInEditMode]
    public class UIHorizontalFlex : MonoBehaviour
    {
        [Header("Horizonal Settings")]
        public float[] ratio = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
        public float[] percentageColumns = new float[] { 0.25f, 0.25f, 0.25f, 0.25f };


        [Header("Active Items")]
        [SerializeField] private bool forceActiveIfInactive = false;
        [SerializeField] private bool useActive;
        [SerializeField] private bool shrinkCountToElementCount;

        [Header("Force Update")]
        public bool forceUpdate = false;

        [Header("Debug")]
        [SerializeField]
        protected int internalColumnCount;

        [SerializeField]
        protected int internalRowCount;

        [SerializeField]
        private List<RectTransform> _children = new();
        public List<RectTransform> children
        {
            get
            {
                return _children;
            }
            private set
            {
                _children = value;
            }
        }
        private int currentActiveCount = 0;
        private int tempActiveCount = 0;
        public void OnEnable()
        {
            this.UpdateLayout();
        }

        public void LateUpdate()
        {
            if (this.transform.childCount != this.children.Count || forceUpdate || currentActiveCount != tempActiveCount)
            {
                this.CalculatePercentage();
                this.UpdateLayout();
            }
        }
        private void CalculatePercentage()
        {
            var total = 0.0f;
            for (var i = 0; i < ratio.Length; i++)
            {
                total += ratio[i];
            }
            percentageColumns = new float[ratio.Length];
            for (var i = 0; i < ratio.Length; i++)
            {
                percentageColumns[i] = ratio[i] / total;
            }
        }
        public void UpdateLayout()
        {
            this.CollectChildren();

            var currentX = 0.0f;

            for (var i = 0; i < this.children.Count; i++)
            {
                //break if we don't have a thing
                if (i >= percentageColumns.Length) { break; }

                var child = this.children[i];
                var percent = percentageColumns[i];

                child.anchorMin = new Vector2(currentX, 0.0f);
                child.anchorMax = new Vector2(currentX + percent, 1.0f);

                currentX = currentX + percent;
            }

            forceUpdate = false;
            currentActiveCount = tempActiveCount;
        }
        protected void CollectChildren()
        {
            this.children.Clear();
            foreach (Transform child in this.transform)
            {
                if (forceActiveIfInactive)
                {
                    child.gameObject.SetActive(true);
                    this.children.Add(child.GetComponent<RectTransform>());
                }
                else if (useActive)
                {
                    if (child.gameObject.activeSelf)
                    {
                        this.children.Add(child.GetComponent<RectTransform>());
                    }
                }
                else
                {
                    this.children.Add(child.GetComponent<RectTransform>());
                }
            }
        }
    }
}