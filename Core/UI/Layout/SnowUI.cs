using System;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.UI
{
    public class SnowUI : MonoBehaviour
    {
        [Header("Gap")]
        [Range(0, 1)] public float gapX = 0.1f;
        [Range(0, 1)] public float gapY = 0.1f;
        [Header("Active Items")]
        public bool forceActiveIfInactive = false;
        public bool useActive;
        protected int currentActiveCount = 0;
        protected int tempActiveCount = 0;

        [Header("UI Update Direction")]
        public UIDirection direction = UIDirection.ColumnsFirst;
        public UIAlignDirection alignDirection = UIAlignDirection.TopLeft;

        [Header("Force Update")]
        public bool forceUpdate = false;

        public int columnCount
        {
            get; private set;
        }
        public int rowCount
        {
            get; private set;
        }

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


        private void OnEnable()
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
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).gameObject.activeSelf)
                    {
                        tempActiveCount++;
                    }
                }
            }
        }

        protected void ProcessCell(int x, int y, int index)
        {
            if (index < children.Count)
            {
                var child = children[index];
                if (index < rowCount * columnCount)
                {
                    var minX = x * percentWidth;
                    var maxX = x * percentWidth + percentWidth;

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
                }
                else
                {
                    Debug.LogError($"Child {child.gameObject.name} does not fit in the {columnCount * rowCount} " +
                        $"spaces calculated by the row and column count. " +
                        $"Hiding the child, if this is not intentional, check the numberOfRows and numberOfColumns variable", child.gameObject);
                    child.gameObject.SetActive(false);
                }
            }
        }

        protected void ProcessChildren()
        {
            if (children.Count > 0)
            {
                int index = 0;
                switch (direction)
                {
                    case UIDirection.ColumnsFirst:
                        for (int y = 0; y < rowCount; y++)
                        {
                            for (int x = 0; x < columnCount; x++)
                            {
                                ProcessCell(x, y, index);
                                ++index;
                            }
                        }
                        break;
                    case UIDirection.RowsFirst:
                        for (int x = 0; x < columnCount; x++)
                        {
                            for (int y = 0; y < rowCount; y++)
                            {
                                ProcessCell(x, y, index);
                                ++index;
                            }
                        }
                        break;
                }
            }
        }
        protected void CalculateColumns(int count, int rowCount)
        {
            this.rowCount = rowCount;
            columnCount = Mathf.CeilToInt((float)count / (float)rowCount);
            percentWidth = 1.0f / (float)columnCount;
            percentHeight = 1.0f / (float)rowCount;
        }

        protected void CalculateRows(int count, int columnCount)
        {
            this.columnCount = columnCount;
            rowCount = Mathf.CeilToInt((float)children.Count / (float)columnCount);
            percentWidth = 1.0f / (float)columnCount;
            percentHeight = 1.0f / (float)rowCount;
        }
        protected void CalculateGrid(int count, int columnCount, int rowCount)
        {
            this.columnCount = columnCount;
            this.rowCount = rowCount;

            percentWidth = 1.0f / (float)columnCount;
            percentHeight = 1.0f / (float)rowCount;
        }

        protected void CollectChildren()
        {
            children.Clear();
            foreach (Transform child in transform)
            {
                if (forceActiveIfInactive)
                {
                    child.gameObject.SetActive(true);
                    children.Add(child.GetComponent<RectTransform>());
                }
                else if (useActive)
                {
                    if (child.gameObject.activeSelf)
                    {
                        children.Add(child.GetComponent<RectTransform>());
                    }
                }
                else
                {
                    children.Add(child.GetComponent<RectTransform>());
                }
            }
        }
    }
}