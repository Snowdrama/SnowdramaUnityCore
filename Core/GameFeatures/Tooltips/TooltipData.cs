using System;
using UnityEngine;
//public class HideTooltipMessage : AMessage { }


[Serializable]
public struct TooltipData
{
    public string id;
    public float timeRemaining;
    public Vector2 position;
    public Vector2 size;
    public string tooltipText;
}
