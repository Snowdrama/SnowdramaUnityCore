using System;
using System.Runtime.InteropServices;
using UnityEngine;


public class WebGLBinding : MonoBehaviour
{
#if UNITY_WEBGL
    [DllImport("__Internal")]
    public static extern void DebugLog(string output);
    [DllImport("__Internal")]
    public static extern void SetToLocalStorage(string key, string data);
    [DllImport("__Internal")]
    public static extern string GetFromLocalStorage(string key);
    [DllImport("__Internal")]
    public static extern bool RemoveFromLocalStorage(string key);
    [DllImport("__Internal")]
    public static extern bool HasKeyInLocalStorage(string key);
#else
    //dummy proxies that shoudn't be used 
    public static void DebugLog(string output) { throw new Exception("WebGL functions cannot be used in non WebGL contexts"); }
    public static void SetToLocalStorage(string key, string data) { throw new Exception("WebGL functions cannot be used in non WebGL contexts"); }
    public static string GetFromLocalStorage(string key) { throw new Exception("WebGL functions cannot be used in non WebGL contexts"); }
    public static bool RemoveFromLocalStorage(string key) { throw new Exception("WebGL functions cannot be used in non WebGL contexts"); }
    public static bool HasKeyInLocalStorage(string key) { throw new Exception("WebGL functions cannot be used in non WebGL contexts"); }
#endif
}