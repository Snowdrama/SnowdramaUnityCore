using System.Collections.Generic;
using UnityEngine;

public class ColorPalette : ScriptableObject
{
    [SerializeField]
    private List<Color> _colors;
    public List<Color> Colors
    {
        get { return _colors; }
    }
#if UNITY_EDITOR
    public void AssignColors(List<Color> colors)
    {
        _colors = new List<Color>(colors);
    }
#endif
}
