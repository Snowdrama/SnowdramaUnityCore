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
    public class UIVerticalGroup : SnowUIGroup, ISnowUILayout
    {
        [Header("Vertical Settings")]
        public int numberOfRows = 0;
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
                this.CalculateColumns(this.children.Count, (this.children.Count <= numberOfRows) ? this.children.Count : numberOfRows);
            }
            else
            {
                this.CalculateColumns(this.children.Count, numberOfRows);
            }
            this.ProcessChildren();
            forceUpdate = false;
            currentActiveCount = tempActiveCount;
        }
    }
}




