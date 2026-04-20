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

    public static int ToLayerInt(this LayerMask layerMask) {
		int layerNumber = 0;
		int layer = layerMask.value;
		while(layer > 0) {
			layer = layer >> 1;
			layerNumber++;
		}
		return layerNumber - 1;
	}
}