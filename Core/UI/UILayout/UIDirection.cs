using Snowdrama.UI;
using UnityEngine;

namespace Snowdrama.UI
{
    /// <summary>
    /// This class fits all the children into the current RectTransform
    /// Use SnowUIContentFitter if you need the RectTransform to fit
    /// to the size of the children
    /// </summary>
    /// 
    public enum UIDirection
    {
        ColumnsFirst,
        RowsFirst,
    }
    public enum UIAlignDirection
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
    }

    public enum UIAnchorType
    {
        Center, //Anchors (0.5, 0.5) (0.5, 0.5)
        Top, // Anchors (0,1) (1,1)
        Bottom, // Anchors (0,0) (1,0)
        Left, // Anchors (0,0) (0,1)
        Right,// Anchors (1,0) (1,1)
        topLeft,// Anchors (0,1) (0,1)
        TopRight,// Anchors (1,1) (1,1)
        BottomLeft,// Anchors (0,0) (0,0)
        BottomRight,// Anchors (1,0) (1,0)
    }
}

