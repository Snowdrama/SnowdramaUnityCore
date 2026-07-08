using Snowdrama.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.UI
{
    /// <summary>
    /// This class fits all the children into horizontal columns based on some size.
    /// </summary>
    [ExecuteInEditMode]
    public class UIHorizontalFlex : MonoBehaviour, ISnowUILayout
    {
        [Header("Horizonal Settings")]
        public float[] ratio = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
        public float[] percentageColumns = new float[] { 0.25f, 0.25f, 0.25f, 0.25f };
        public bool leftToRight = true;

        [SerializeField] private float spaceBetween = 0.01f;
        [SerializeField] private float spaceTopBot = 0.02f;
        [SerializeField] private float spaceLeftRight = 0.02f;


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

            var current = 0.0f;

            for (var i = 0; i < this.children.Count; i++)
            {
                //break if we don't have a thing
                if (i >= percentageColumns.Length) { break; }

                var child = this.children[i];
                var percent = percentageColumns[i];


                if (leftToRight)
                {

                    if (i == 0)
                    {
                        // ******************************** 1.0f - 0.0 - 0.25 + 0.1
                        //child.anchorMin = new Vector2(0.0f + spaceLeftRight, 1.0f - current - percent + spaceBetween);
                        //child.anchorMax = new Vector2(1.0f - spaceLeftRight, 1.0f - current - spaceTopBot);

                        child.anchorMin = new Vector2(current + spaceLeftRight, 0.0f + spaceTopBot);
                        child.anchorMax = new Vector2(current + percent - spaceBetween, 1.0f - spaceTopBot);
                    }
                    else if (i == this.children.Count - 1)
                    {
                        //child.anchorMin = new Vector2(0.0f + spaceLeftRight, 1.0f - current - percent + spaceTopBot);
                        //child.anchorMax = new Vector2(1.0f - spaceLeftRight, 1.0f - current - spaceBetween);

                        child.anchorMin = new Vector2(current + spaceBetween, 0.0f + spaceTopBot);
                        child.anchorMax = new Vector2(current + percent - spaceLeftRight, 1.0f - spaceTopBot);
                    }
                    else
                    {
                        //child.anchorMin = new Vector2(0.0f + spaceLeftRight, 1.0f - current - percent + spaceBetween);
                        //child.anchorMax = new Vector2(1.0f - spaceLeftRight, 1.0f - current - spaceBetween);



                        child.anchorMin = new Vector2(current + spaceBetween, 0.0f + spaceTopBot);
                        child.anchorMax = new Vector2(current + percent - spaceBetween, 1.0f - spaceTopBot);
                    }
                }












                //child.anchorMin = new Vector2(current, 0.0f);
                //child.anchorMax = new Vector2(current + percent, 1.0f);

                current = current + percent;
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