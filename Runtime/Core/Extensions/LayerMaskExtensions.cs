using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskExtensions
{
    public static bool LayerContains(this LayerMask layermask, int layer)
    {
        return layermask == (layermask | (1 << layer));
    }
}