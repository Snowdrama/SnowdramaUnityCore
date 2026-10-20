using JetBrains.Annotations;
using Snowdrama.UI;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Snowdrama.UI
{
    /// <summary>
    /// This class fits all the children into vertical columns based on some size.
    /// </summary>
    [ExecuteInEditMode]
    public class UIVerticalFlex : MonoBehaviour, ISnowUILayout
    {
        [Header("Horizonal Settings")]
        [SerializeField] private float[] desktopRatios = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
        [SerializeField] private float[] webGLRatios = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
        [SerializeField] private float[] percentages = new float[] { 0.25f, 0.25f, 0.25f, 0.25f };
        [SerializeField] private bool topDown = true;

        [SerializeField] private float spaceBetween = 0.01f;
        [SerializeField] private float spaceTopBot = 0.02f;
        [SerializeField] private float spaceLeftRight = 0.02f;

        [Header("Active Items")]
        [SerializeField] private bool forceActiveIfInactive = false;
        [SerializeField] private bool useActive;
        [SerializeField] private bool shrinkCountToElementCount;

        [Header("Force Update")]
        [SerializeField] private bool forceUpdate = false;

        [Header("Debug")]
        [SerializeField]
        protected int internalColumnCount;

        [SerializeField]
        protected int internalRowCount;

        [SerializeField]
        private List<RectTransform> _children = new();
        public List<RectTransform> children
        {
            get => _children; private set => _children = value;
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

#if UNITY_WEBGL
            var ratios = webGLRatios;
#else
            var ratios = desktopRatios;
#endif
            for (var i = 0; i < ratios.Length; i++)
            {
                total += ratios[i];
            }
            percentages = new float[ratios.Length];
            for (var i = 0; i < ratios.Length; i++)
            {
                percentages[i] = ratios[i] / total;
            }
        }
        public void UpdateLayout()
        {
            this.CollectChildren();

            var current = 0.0f;

            for (var i = 0; i < this.children.Count; i++)
            {
                //break if we don't have a thing
                if (i >= percentages.Length) { break; }

                if (topDown)
                {
                    var child = this.children[i];
                    var percent = percentages[i];

                    if (i == 0)
                    {
                        // ******************************** 1.0f - 0.0 - 0.25 + 0.1
                        child.anchorMin = new Vector2(0.0f + spaceLeftRight, 1.0f - current - percent + spaceBetween);
                        child.anchorMax = new Vector2(1.0f - spaceLeftRight, 1.0f - current - spaceTopBot);
                    }
                    else if (i == this.children.Count - 1)
                    {
                        child.anchorMin = new Vector2(0.0f + spaceLeftRight, 1.0f - current - percent + spaceTopBot);
                        child.anchorMax = new Vector2(1.0f - spaceLeftRight, 1.0f - current - spaceBetween);
                    }
                    else
                    {
                        child.anchorMin = new Vector2(0.0f + spaceLeftRight, 1.0f - current - percent + spaceBetween);
                        child.anchorMax = new Vector2(1.0f - spaceLeftRight, 1.0f - current - spaceBetween);
                    }

                    current = current + percent;
                }
                else
                {
                    var child = this.children[i];
                    var percent = percentages[i];

                    if (i == 0)
                    {
                        child.anchorMin = new Vector2(0.0f + spaceLeftRight, current + spaceTopBot);
                        child.anchorMax = new Vector2(1.0f - spaceLeftRight, current + percent - spaceBetween);
                    }
                    else if (i == this.children.Count - 1)
                    {
                        child.anchorMin = new Vector2(0.0f + spaceLeftRight, current + spaceBetween);
                        child.anchorMax = new Vector2(1.0f - spaceLeftRight, current + percent - spaceTopBot);
                    }
                    else
                    {
                        child.anchorMin = new Vector2(0.0f + spaceLeftRight, current + spaceBetween);
                        child.anchorMax = new Vector2(1.0f - spaceLeftRight, current + percent - spaceBetween);
                    }

                    current = current + percent;
                }
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

        public void MarkDirtyLayout()
        {
            forceUpdate = true;
        }
    }
}