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
    public class UIVerticalGroup : SnowUI
    {
        [Header("Vertical Settings")]
        public int numberOfRows = 0;
        public override void LateUpdate()
        {
            base.LateUpdate();
            if (transform.childCount != children.Count || forceUpdate || currentActiveCount != tempActiveCount)
            {
                CollectChildren();
                if (this.shrinkCountToElementCount)
                {
                    CalculateRows(children.Count, (children.Count <= numberOfRows) ? children.Count : numberOfRows);
                }
                else
                {
                    CalculateColumns(children.Count, numberOfRows);
                }
                ProcessChildren();
                forceUpdate = false;
                currentActiveCount = tempActiveCount;
            }
        }
    }
}
