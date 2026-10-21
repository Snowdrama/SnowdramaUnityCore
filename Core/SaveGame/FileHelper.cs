using System.IO;

/// <summary>
/// A tool to replace the File calls
/// This properly handles switching to 
/// WebGL stuff when needed
/// </summary>
public class FileHelper
{
    public static bool Exists(string path)
    {
#if WEB_GL
        return WebGLBinding.HasKeyInStorage(path);
#else
        return File.Exists(path);
#endif
    }

    public static string ReadAllText(string path)
    {
#if WEB_GL
        return WebGLBinding.GetFromLocalStorage(path, data);
#else
        return File.ReadAllText(path);
#endif
    }

    public static void WriteAllBytes(string path, byte[] data)
    {
#if WEB_GL
        //Nothing
#else
        File.WriteAllBytes(path, data);
#endif
    }

    public static void WriteAllText(string path, string data)
    {
#if WEB_GL
        WebGLBinding.SetToLocalStorage(path, data);
#else
        File.WriteAllText(path, data);
#endif
    }
    public static void Delete(string path)
    {
#if WEB_GL
        WebGLBinding.RemoveFromLocalStorage(path);
#else
        File.Delete(path);
#endif
    }
}


public class DirectoryHelper
{
    public static bool Exists(string path)
    {
#if WEB_GL
        return true; //no need to check on webgl
#else
        return Directory.Exists(path);
#endif
    }
    public static void CreateDirectory(string path)
    {
#if WEB_GL
        //no need to create on webgl
#else
        Directory.CreateDirectory(path);
#endif
    }

}
