using System;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.UI
{
    public class SnowUI : MonoBehaviour
    {
        [Header("Gap")]
        [Range(0, 1), SerializeField] protected float gapX = 0.1f;
        [Range(0, 1), SerializeField] protected float gapY = 0.1f;
        [Header("Active Items")]
        [SerializeField] protected bool forceActiveIfInactive = false;
        [SerializeField] protected bool useActive;
        [SerializeField] protected bool shrinkCountToElementCount;
        protected int currentActiveCount = 0;
        protected int tempActiveCount = 0;

        [Header("UI Update Direction")]
        public UIDirection direction = UIDirection.ColumnsFirst;
        public UIAlignDirection alignDirection = UIAlignDirection.TopLeft;

        [Header("Force Update")]
        public bool forceUpdate = false;

        [Header("Debug")]
        [SerializeField]
        protected int internalColumnCount;

        [SerializeField]
        protected int internalRowCount;

        [SerializeField]
        private List<RectTransform> _children;
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
        private float percentWidth;
        private float percentHeight;


        public virtual void OnEnable()
        {
            if (_children == null)
            {
                _children = new List<RectTransform>();

            }
        }

        public virtual void LateUpdate()
        {
            if (useActive)
            {
                tempActiveCount = 0;
                for (var i = 0; i < this.transform.childCount; i++)
                {
                    if (this.transform.GetChild(i).gameObject.activeSelf)
                    {
                        tempActiveCount++;
                    }
                }
            }
        }
        public virtual void UpdateLayout()
        {
        }

        protected void ProcessCellPercent(int x, int y, int index, float percent)
        {

        }

        protected void ProcessCell(int x, int y, int index)
        {
            if (index < this.children.Count)
            {
                var child = this.children[index];
                if (index < internalRowCount * internalColumnCount)
                {
                    var minX = x * percentWidth;
                    var maxX = (x * percentWidth) + percentWidth;

                    var maxY = 1.0f - (y * percentHeight);
                    var minY = 1.0f - ((y + 1) * percentHeight);

                    switch (alignDirection)
                    {
                        case UIAlignDirection.TopLeft:
                            child.anchorMin = new Vector2(minX, minY);
                            child.anchorMax = new Vector2(maxX, maxY);
                            break;
                        case UIAlignDirection.TopRight:
                            //top right align
                            child.anchorMin = new Vector2(1.0f - maxX, minY);
                            child.anchorMax = new Vector2(1.0f - minX, maxY);
                            break;
                        case UIAlignDirection.BottomLeft:
                            //bottom left align
                            child.anchorMin = new Vector2(minX, 1.0f - maxY);
                            child.anchorMax = new Vector2(maxX, 1.0f - minY);
                            break;
                        case UIAlignDirection.BottomRight:
                            //bottom right align
                            child.anchorMin = new Vector2(1.0f - maxX, 1.0f - maxY);
                            child.anchorMax = new Vector2(1.0f - minX, 1.0f - minY);
                            break;
                    }

                    child.offsetMin = new Vector2(0, 0);
                    child.offsetMax = new Vector2(0, 0);

                    var currentWidth = child.rect.width;
                    var currentHeight = child.rect.height;

                    var offsetX = currentWidth * gapX / 2.0f;
                    var offsetY = currentHeight * gapY / 2.0f;

                    child.offsetMin = new Vector2(offsetX, offsetY);
                    child.offsetMax = new Vector2(-offsetX, -offsetY);
                    child.ForceUpdateRectTransforms();
                }
                else
                {
                    Debug.LogError($"Child {child.gameObject.name} does not fit in the {internalColumnCount * internalRowCount} " +
                        $"spaces calculated by the row and column count. " +
                        $"Hiding the child, if this is not intentional, check the numberOfRows and numberOfColumns variable", child.gameObject);
                    child.gameObject.SetActive(false);
                }
            }
        }

        protected void ProcessChildrenWithPercent(float[] percents)
        {
            if (this.children.Count > 0)
            {
                var index = 0;
                switch (direction)
                {
                    case UIDirection.ColumnsFirst:
                        for (var y = 0; y < internalRowCount; y++)
                        {
                            for (var x = 0; x < internalColumnCount; x++)
                            {
                                this.ProcessCell(x, y, index);
                                ++index;
                            }
                        }
                        break;
                    case UIDirection.RowsFirst:
                        for (var x = 0; x < internalColumnCount; x++)
                        {
                            for (var y = 0; y < internalRowCount; y++)
                            {
                                this.ProcessCell(x, y, index);
                                ++index;
                            }
                        }
                        break;
                }
            }
        }

        protected void ProcessChildren()
        {
            if (this.children.Count > 0)
            {
                var index = 0;
                switch (direction)
                {
                    case UIDirection.ColumnsFirst:
                        for (var y = 0; y < internalRowCount; y++)
                        {
                            for (var x = 0; x < internalColumnCount; x++)
                            {
                                this.ProcessCell(x, y, index);
                                ++index;
                            }
                        }
                        break;
                    case UIDirection.RowsFirst:
                        for (var x = 0; x < internalColumnCount; x++)
                        {
                            for (var y = 0; y < internalRowCount; y++)
                            {
                                this.ProcessCell(x, y, index);
                                ++index;
                            }
                        }
                        break;
                }
            }
        }
        protected void CalculateColumns(int count, int rowCount)
        {
            internalRowCount = rowCount;
            internalColumnCount = Mathf.CeilToInt((float)count / (float)rowCount);
            percentWidth = 1.0f / (float)internalColumnCount;
            percentHeight = 1.0f / (float)internalRowCount;
        }

        protected void CalculateRows(int count, int columnCount)
        {
            internalColumnCount = columnCount;
            internalRowCount = Mathf.CeilToInt((float)count / (float)columnCount);
            percentWidth = 1.0f / (float)internalColumnCount;
            percentHeight = 1.0f / (float)internalRowCount;
        }
        protected void CalculateGrid(int count, int columnCount, int rowCount)
        {
            internalColumnCount = columnCount;
            internalRowCount = rowCount;

            if (shrinkCountToElementCount)
            {

                switch (direction)
                {
                    case UIDirection.ColumnsFirst:
                        if (count <= columnCount)
                        {
                            internalColumnCount = count;
                        }
                        if (Mathf.CeilToInt((float)count / (float)columnCount) < rowCount)
                        {
                            internalRowCount = Mathf.CeilToInt((float)count / (float)columnCount);
                        }
                        break;

                    case UIDirection.RowsFirst:

                        if (Mathf.CeilToInt((float)count / (float)rowCount) < columnCount)
                        {
                            internalRowCount = Mathf.CeilToInt((float)count / (float)rowCount);
                        }
                        if (count <= rowCount)
                        {
                            internalRowCount = count;
                        }
                        break;
                }
            }

            percentWidth = 1.0f / (float)internalColumnCount;
            percentHeight = 1.0f / (float)internalRowCount;
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
        public int GetRowCount()
        {
            return internalRowCount;
        }
        public int GetColumnCount()
        {
            return internalColumnCount;
        }
    }

}