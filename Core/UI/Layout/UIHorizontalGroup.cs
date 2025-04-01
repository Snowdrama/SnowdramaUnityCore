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
    public class UIHorizontalGroup : SnowUI
    {
        [Header("Horizonal Settings")]
        public int numberOfColumns = 0;
        public override void LateUpdate()
        {
            base.LateUpdate();
            if (transform.childCount != children.Count || forceUpdate || currentActiveCount != tempActiveCount)
            {
                CollectChildren();
                CalculateRows(children.Count, numberOfColumns);
                ProcessChildren();

                forceUpdate = false;
                currentActiveCount = tempActiveCount;
            }
        }
    }
}