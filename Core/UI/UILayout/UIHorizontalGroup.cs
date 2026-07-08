using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.UI
{
    /// <summary>
    /// This class fits all the children into horizontal columns based on some size.
    /// </summary>
    [ExecuteInEditMode]
    public class UIHorizontalGroup : SnowUIGroup, ISnowUILayout
    {
        [Header("Horizonal Settings")]
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
            if (shrinkCountToElementCount)
            {
                this.CalculateRows(this.children.Count, (this.children.Count <= numberOfColumns) ? this.children.Count : numberOfColumns);
            }
            else
            {
                this.CalculateRows(this.children.Count, numberOfColumns);
            }
            this.ProcessChildren();

            forceUpdate = false;
            currentActiveCount = tempActiveCount;
        }
    }
}