using System.IO;
using UnityEngine;

/// <summary>
/// A tool to replace the File calls
/// This properly handles switching to 
/// WebGL stuff when needed
/// </summary>
public class FileHelper
{
    public static bool Exists(string path)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log($"Checking File Exists in WebGL!");
        return WebGLBinding.HasKeyInLocalStorage(path);
#else
        Debug.Log($"Checking File Exists! Not WebGL");
        return File.Exists(path);
#endif
    }

    public static string ReadAllText(string path)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log($"ReadAllText, WebGL");
        return WebGLBinding.GetFromLocalStorage(path);
#else
        Debug.Log($"ReadAllText, Not WebGL");
        return File.ReadAllText(path);
#endif
    }

    public static void WriteAllBytes(string path, byte[] data)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        //No saving images in WebGL
#else
        Debug.Log($"WriteAllBytes, Not WebGL");
        File.WriteAllBytes(path, data);
#endif
    }

    public static void WriteAllText(string path, string data)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log($"WriteAllText, WebGL");
        WebGLBinding.SetToLocalStorage(path, data);
#else
        Debug.Log($"WriteAllText, Not WebGL");
        File.WriteAllText(path, data);
#endif
    }
    public static void Delete(string path)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log($"Delete, WebGL");
        WebGLBinding.RemoveFromLocalStorage(path);
#else
        Debug.Log($"Delete, Not WebGL");
        File.Delete(path);
#endif
    }
}


public class DirectoryHelper
{
    public static bool Exists(string path)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return true; //no need to check on webgl
#else
        return Directory.Exists(path);
#endif
    }
    public static void CreateDirectory(string path)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        //no need to create on webgl
#else
        Directory.CreateDirectory(path);
#endif
    }

}
