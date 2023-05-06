using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugTools
{
    public static void DebugLog(string log, bool enableFlag = false, GameObject target = null)
    {
        if (enableFlag)
        {
            Debug.Log(log, target);
        }
    }
    public static void DebugLogWarning(string log, bool enableFlag = false, GameObject target = null)
    {
        if (enableFlag)
        {
            Debug.LogWarning(log, target);
        }
    }
    public static void DebugLogError(string log, bool enableFlag = false, GameObject target = null)
    {
        if (enableFlag)
        {
            Debug.LogError(log, target);
        }
    }
}
