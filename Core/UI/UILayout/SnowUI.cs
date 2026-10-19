using Codice.Client.BaseCommands.BranchExplorer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.UI
{

    [RequireComponent(typeof(RectTransform))]
    public abstract class SnowUIGroup : SnowUI, ISnowUILayout
    {
        [Header("Gap")]
        [Range(0, 1), SerializeField] protected float gapX = 0.1f;
        [Range(0, 1), SerializeField] protected float gapY = 0.1f;

        [Header("Outer Gap Pixels[Experimental]")]
        [SerializeField] protected float gapXOuterPixel = 0.0f;
        [SerializeField] protected float gapYOuterPixel = 0.0f;

        [Header("Inner Gap Pixels[Experimental]")]
        [SerializeField] protected float gapXInnerPixel = 0.0f;
        [SerializeField] protected float gapYInnerPixel = 0.0f;

        [Header("Active Items")]
        [SerializeField] protected bool forceActiveIfInactive = false;
        [SerializeField] protected bool useActive = true;
        [SerializeField] protected bool shrinkCountToElementCount = true;
        protected int currentActiveCount = 0;
        protected int tempActiveCount = 0;

        [Header("UI Update Direction")]
        public UIDirection direction = UIDirection.ColumnsFirst;
        public UIAlignDirection alignDirection = UIAlignDirection.TopLeft;
        [SerializeField] private UIAnchorType anchorType = UIAnchorType.Center;

        [Header("Size Control")]
        [Header("Note this doesn't resize the children but instead sets the parent size to the FittedSize as if the number of elements were that size")]
        [SerializeField] protected bool FitParentToChildren = false;
        [SerializeField] protected bool FitHeight = false;
        [SerializeField] protected bool FitWidth = false;
        [SerializeField] protected Vector2 FittedSize = new Vector2(100, 100);

        [Header("Force Update")]
        public bool forceUpdate = false;

        [Header("Debug")]
        [SerializeField]
        protected int internalColumnCount;

        [SerializeField]
        protected int internalRowCount;

        private RectTransform MyRect;
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


        public override void OnEnable()
        {
            base.OnEnable();
            if (_children == null)
            {
                _children = new List<RectTransform>();

            }
            MyRect = this.GetComponent<RectTransform>();
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
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
        public override void UpdateLayout()
        {
            base.UpdateLayout();
        }

        protected void ProcessCellPercent(int x, int y, int index, float percent)
        {
        }

        protected void ProcessCell(int x, int y, int index, bool firstElementX = true, bool lastElementX = true, bool firstElementY = true, bool lastElementY = true)
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

                    // Use the parent's rect rather than child.rect: child.rect can be stale
                    // right after the anchors change, the parent's size is what defines the cell.
                    var currentWidth = MyRect.rect.width * percentWidth;
                    var currentHeight = MyRect.rect.height * percentHeight;

                    var offsetYTop = currentHeight * gapY / 2.0f;
                    var offsetYBot = currentHeight * gapY / 2.0f;

                    var offsetXLeft = currentWidth * gapX / 2.0f;
                    var offsetXRight = currentWidth * gapX / 2.0f;


                    if (firstElementY)
                    {
                        offsetYTop += gapYOuterPixel;
                    }
                    else
                    {
                        offsetYTop += gapYInnerPixel;
                    }
                    if (lastElementY)
                    {
                        offsetYBot += gapYOuterPixel;
                    }
                    else
                    {
                        offsetYBot += gapYInnerPixel;
                    }
                    if (firstElementX)
                    {
                        offsetXLeft += gapXOuterPixel;
                    }
                    else
                    {
                        offsetXLeft += gapXInnerPixel;
                    }
                    if (lastElementX)
                    {
                        offsetXRight += gapXOuterPixel;
                    }
                    else
                    {
                        offsetXRight += gapXInnerPixel;
                    }

                    child.offsetMin = new Vector2(offsetXLeft, offsetYBot);
                    child.offsetMax = new Vector2(-offsetXRight, -offsetYTop);
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
                                this.ProcessCell(x, y, index, x == 0, x == internalColumnCount - 1, y == 0, y == internalRowCount - 1);
                                ++index;
                            }
                        }
                        break;
                    case UIDirection.RowsFirst:
                        for (var x = 0; x < internalColumnCount; x++)
                        {
                            for (var y = 0; y < internalRowCount; y++)
                            {
                                this.ProcessCell(x, y, index, x == 0, x == internalColumnCount - 1, y == 0, y == internalRowCount - 1);
                                ++index;
                            }
                        }
                        break;
                }
            }
        }

        protected void ProcessChildren()
        {
            // Size and place the panel first so the cells (and their gaps) are
            // calculated against the final panel size.
            if (FitParentToChildren)
            {
                this.FitParent();
            }

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
                                this.ProcessCell(x, y, index, x == 0, x == internalColumnCount - 1, y == 0, y == internalRowCount - 1);
                                ++index;
                            }
                        }
                        break;
                    case UIDirection.RowsFirst:
                        for (var x = 0; x < internalColumnCount; x++)
                        {
                            for (var y = 0; y < internalRowCount; y++)
                            {
                                this.ProcessCell(x, y, index, x == 0, x == internalColumnCount - 1, y == 0, y == internalRowCount - 1);
                                ++index;
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Sets the anchors, pivot, size and position of this panel so that it is
        /// FittedSize * (columns/rows) large and sits flush against the edge/corner of
        /// its parent described by anchorType.
        /// </summary>
        private void FitParent()
        {
            if (MyRect == null)
            {
                MyRect = this.GetComponent<RectTransform>();
            }

            switch (anchorType)
            {
                case UIAnchorType.Center:
                    MyRect.anchorMin = new Vector2(0.5f, 0.5f);
                    MyRect.anchorMax = new Vector2(0.5f, 0.5f);
                    break;
                case UIAnchorType.Top:
                    MyRect.anchorMin = new Vector2(0, 1);
                    MyRect.anchorMax = new Vector2(1, 1);
                    break;
                case UIAnchorType.Bottom:
                    MyRect.anchorMin = new Vector2(0, 0);
                    MyRect.anchorMax = new Vector2(1, 0);
                    break;
                case UIAnchorType.Left:
                    MyRect.anchorMin = new Vector2(0, 0);
                    MyRect.anchorMax = new Vector2(0, 1);
                    break;
                case UIAnchorType.Right:
                    MyRect.anchorMin = new Vector2(1, 0);
                    MyRect.anchorMax = new Vector2(1, 1);
                    break;
                case UIAnchorType.topLeft:
                    MyRect.anchorMin = new Vector2(0, 1);
                    MyRect.anchorMax = new Vector2(0, 1);
                    break;
                case UIAnchorType.TopRight:
                    MyRect.anchorMin = new Vector2(1, 1);
                    MyRect.anchorMax = new Vector2(1, 1);
                    break;
                case UIAnchorType.BottomLeft:
                    MyRect.anchorMin = new Vector2(0, 0);
                    MyRect.anchorMax = new Vector2(0, 0);
                    break;
                case UIAnchorType.BottomRight:
                    MyRect.anchorMin = new Vector2(1, 0);
                    MyRect.anchorMax = new Vector2(1, 0);
                    break;
            }

            MyRect.pivot = GetPivotForAnchor(anchorType);

            var stretchX = !Mathf.Approximately(MyRect.anchorMin.x, MyRect.anchorMax.x);
            var stretchY = !Mathf.Approximately(MyRect.anchorMin.y, MyRect.anchorMax.y);

            if (FitWidth)
            {
                MyRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, FittedSize.x * internalColumnCount);
            }
            else if (stretchX)
            {
                MyRect.offsetMin = new Vector2(0, MyRect.offsetMin.y);
                MyRect.offsetMax = new Vector2(0, MyRect.offsetMax.y);
            }

            if (FitHeight)
            {
                MyRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, FittedSize.y * internalRowCount);
            }
            else if (stretchY)
            {
                MyRect.offsetMin = new Vector2(MyRect.offsetMin.x, 0);
                MyRect.offsetMax = new Vector2(MyRect.offsetMax.x, 0);
            }

            MyRect.anchoredPosition = Vector2.zero;

            MyRect.ForceUpdateRectTransforms();
        }

        private static Vector2 GetPivotForAnchor(UIAnchorType type)
        {
            switch (type)
            {
                case UIAnchorType.Top: return new Vector2(0.5f, 1f);
                case UIAnchorType.Bottom: return new Vector2(0.5f, 0f);
                case UIAnchorType.Left: return new Vector2(0f, 0.5f);
                case UIAnchorType.Right: return new Vector2(1f, 0.5f);
                case UIAnchorType.topLeft: return new Vector2(0f, 1f);
                case UIAnchorType.TopRight: return new Vector2(1f, 1f);
                case UIAnchorType.BottomLeft: return new Vector2(0f, 0f);
                case UIAnchorType.BottomRight: return new Vector2(1f, 0f);
                case UIAnchorType.Center:
                default:
                    return new Vector2(0.5f, 0.5f);
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
                        // Mirror of ColumnsFirst: columns are the axis that shrinks here
                        if (Mathf.CeilToInt((float)count / (float)rowCount) < columnCount)
                        {
                            internalColumnCount = Mathf.CeilToInt((float)count / (float)rowCount);
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


    public class SnowUI : MonoBehaviour, ISnowUILayout
    {

        public virtual void OnEnable()
        {
        }
        public virtual void OnDisable()
        {
        }
        public virtual void Update()
        {
        }
        public virtual void LateUpdate()
        {
        }
        public virtual void FixedUpdate()
        {

        }
        public virtual void Start()
        {
        }
        public virtual void Awake()
        {
        }
        public virtual void UpdateLayout()
        {
        }

        public virtual void MarkDirtyLayout()
        {
        }

    }

}