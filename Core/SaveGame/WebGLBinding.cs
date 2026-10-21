using System.Runtime.InteropServices;
using UnityEngine;


public class WebGLBinding : MonoBehaviour
{
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
}