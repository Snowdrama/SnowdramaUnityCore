using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snowdrama.UI
{
    /// <summary>
    /// This class fits all the children into the current RectTransform
    /// </summary>
    [ExecuteInEditMode]
    public class UIGridGroup : SnowUI
    {

        [Header("Grid Settings")]
        public int numberOfRows = 0;
        public int numberOfColumns = 0;
        public override void LateUpdate()
        {
            base.LateUpdate();
            if (transform.childCount != children.Count || forceUpdate || currentActiveCount != tempActiveCount)
            {
                CollectChildren();

                CalculateGrid(children.Count, numberOfColumns, numberOfRows);


                ProcessChildren();
                forceUpdate = false;
                currentActiveCount = tempActiveCount;
            }
        }
    }
}