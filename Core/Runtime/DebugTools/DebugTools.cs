using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugTools
{
    /// <summary>
    /// An Alias of Debug.Log that takes a bool flag to enable or disable the log.
    /// 
    /// Intended to be used so you can "hide" your logs per class by including a
    /// debug flag in options or in the component to enable and disable logging
    /// </summary>
    /// <param name="log"></param>
    /// <param name="enableFlag"></param>
    /// <param name="target"></param>
    public static void Log(string log, bool enableFlag = false, GameObject target = null)
    {
        if (enableFlag)
        {
            Debug.Log(log, target);
        }
    }
    /// <summary>
    /// An Alias of Debug.LogWarning that takes a bool flag to enable or disable the log.
    /// 
    /// Intended to be used so you can "hide" your logs per class by including a
    /// debug flag in options or in the component to enable and disable logging
    /// </summary>
    /// <param name="log"></param>
    /// <param name="enableFlag"></param>
    /// <param name="target"></param>
    public static void Warning(string log, bool enableFlag = false, GameObject target = null)
    {
        if (enableFlag)
        {
            Debug.LogWarning(log, target);
        }
    }
    /// <summary>
    /// An Alias of Debug.LogError that takes a bool flag to enable or disable the log.
    /// 
    /// Intended to be used so you can "hide" your logs per class by including a
    /// debug flag in options or in the component to enable and disable logging
    /// </summary>
    /// <param name="log"></param>
    /// <param name="enableFlag"></param>
    /// <param name="target"></param>
    public static void Error(string log, bool enableFlag = false, GameObject target = null)
    {
        if (enableFlag)
        {
            Debug.LogError(log, target);
        }
    }
}
