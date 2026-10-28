using UnityEngine;

public static class DebugEx
{
    public static void Log(string output,
        GameObject target = null,
        bool toggle = true,
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {
        if (toggle)
        {
            var path = sourceFilePath.Split("\\");
            Debug.Log($"[{path[path.Length - 1]}:{sourceLineNumber}]:{output}", target);
        }
    }
    public static void Gray(string output,
        GameObject target = null,
        bool toggle = true,
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {
        if (toggle)
        {
            var path = sourceFilePath.Split("\\");
            Debug.Log($"<color=#CCC>{path[path.Length - 1]}:{sourceLineNumber}]:{output}</color>", target);
        }
    }
    public static void DarkGray(string output,
        GameObject target = null,
        bool toggle = true,
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {
        if (toggle)
        {
            var path = sourceFilePath.Split("\\");
            Debug.Log($"<color=#888>{path[path.Length - 1]}:{sourceLineNumber}]:{output}</color>", target);
        }
    }
    public static void White(string output,
        GameObject target = null,
        bool toggle = true,
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {
        if (toggle)
        {
            var path = sourceFilePath.Split("\\");
            Debug.Log($"<color=#FFF>{path[path.Length - 1]}:{sourceLineNumber}]:{output}</color>", target);
        }
    }
    public static void Black(string output,
        GameObject target = null,
        bool toggle = true,
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {
        if (toggle)
        {
            var path = sourceFilePath.Split("\\");
            Debug.Log($"<color=#000>{path[path.Length - 1]}:{sourceLineNumber}]:{output}</color>", target);
        }
    }
    public static void Magenta(string output,
        GameObject target = null,
        bool toggle = true,
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {

        if (toggle)
        {
            var path = sourceFilePath.Split("\\");
            Debug.Log($"<color=#F0F>{path[path.Length - 1]}:{sourceLineNumber}]:{output}</color>", target);
        }
    }
    public static void Blue(string output,
        GameObject target = null,
        bool toggle = true,
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {

        if (toggle)
        {
            var path = sourceFilePath.Split("\\");
            Debug.Log($"<color=#00F>{path[path.Length - 1]}:{sourceLineNumber}]:{output}</color>", target);
        }
    }
    public static void Cyan(string output,
        GameObject target = null,
        bool toggle = true,
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {

        if (toggle)
        {
            var path = sourceFilePath.Split("\\");
            Debug.Log($"<color=#0FF>{path[path.Length - 1]}:{sourceLineNumber}]:{output}</color>", target);
        }
    }
    public static void Red(string output,
        GameObject target = null,
        bool toggle = true,
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {

        if (toggle)
        {
            var path = sourceFilePath.Split("\\");
            Debug.Log($"<color=#F00>{path[path.Length - 1]}:{sourceLineNumber}]:{output}</color>", target);
        }
    }
    public static void Green(string output,
        GameObject target = null,
        bool toggle = true,
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {

        if (toggle)
        {
            var path = sourceFilePath.Split("\\");
            Debug.Log($"<color=#0F0>{path[path.Length - 1]}:{sourceLineNumber}]:{output}</color>", target);
        }
    }
    public static void Yellow(string output,
        GameObject target = null,
        bool toggle = true,
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {

        if (toggle)
        {
            var path = sourceFilePath.Split("\\");
            Debug.Log($"<color=#FF0>{path[path.Length - 1]}:{sourceLineNumber}]:{output}</color>", target);
        }
    }

    public static void DarkMagenta(string output,
        GameObject target = null,
        bool toggle = true,
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {

        if (toggle)
        {
            var path = sourceFilePath.Split("\\");
            Debug.Log($"<color=#808>{path[path.Length - 1]}:{sourceLineNumber}]:{output}</color>", target);
        }
    }
    public static void DarkBlue(string output,
        GameObject target = null,
        bool toggle = true,
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {

        if (toggle)
        {
            var path = sourceFilePath.Split("\\");
            Debug.Log($"<color=#008>{path[path.Length - 1]}:{sourceLineNumber}]:{output}</color>", target);
        }
    }
    public static void DarkCyan(string output,
        GameObject target = null,
        bool toggle = true,
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {

        if (toggle)
        {
            var path = sourceFilePath.Split("\\");
            Debug.Log($"<color=#088>{path[path.Length - 1]}:{sourceLineNumber}]:{output}</color>", target);
        }
    }
    public static void DarkRed(string output,
        GameObject target = null,
        bool toggle = true,
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {

        if (toggle)
        {
            var path = sourceFilePath.Split("\\");
            Debug.Log($"<color=#800>{path[path.Length - 1]}:{sourceLineNumber}]:{output}</color>", target);
        }
    }
    public static void DarkGreen(string output,
        GameObject target = null,
        bool toggle = true,
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {

        if (toggle)
        {
            var path = sourceFilePath.Split("\\");
            Debug.Log($"<color=#080>{path[path.Length - 1]}:{sourceLineNumber}]:{output}</color>", target);
        }
    }
    public static void DarkYellow(string output,
        GameObject target = null,
        bool toggle = true,
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {

        if (toggle)
        {
            var path = sourceFilePath.Split("\\");
            Debug.Log($"<color=#880>{path[path.Length - 1]}:{sourceLineNumber}]:{output}</color>", target);
        }
    }

    public static void WriteLine(string output,
        GameObject target = null,
        bool toggle = true,
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {
        if (toggle)
        {
            var path = sourceFilePath.Split("\\");
            Debug.Log($"<color=#CCC>{path[path.Length - 1]}:{sourceLineNumber}]:{output}</color>", target);
        }
    }
}