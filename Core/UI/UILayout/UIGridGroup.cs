using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.UI
{
    public interface ISnowUILayout
    {
        void UpdateLayout();
    }
    /// <summary>
    /// This class fits all the children into the current RectTransform
    /// </summary>
    [ExecuteInEditMode]
    public class UIGridGroup : SnowUIGroup, ISnowUILayout
    {

        [Header("Grid Settings")]
        public int numberOfRows = 0;
        public int numberOfColumns = 0;

        public override void OnEnable()
        {
            base.OnEnable();
            this.UpdateLayout();
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
            if (this.transform.childCount != this.children.Count || forceUpdate || currentActiveCount != tempActiveCount)
            {
                this.UpdateLayout();
            }
        }

        public override void UpdateLayout()
        {
            this.CollectChildren();
            this.CalculateGrid(this.children.Count, numberOfColumns, numberOfRows);
            this.ProcessChildren();
            forceUpdate = false;
            currentActiveCount = tempActiveCount;
        }
    }
}