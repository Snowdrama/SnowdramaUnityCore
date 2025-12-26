using UnityEditor;
using UnityEngine;

public class FolderHelper : MonoBehaviour
{
    [MenuItem("Snowdrama/Paths/Open Persistent Data Path", priority = 1)]
    private static void OpenPersistentDataPath()
    {
        System.Diagnostics.Process.Start(Application.persistentDataPath);
    }

    [MenuItem("Snowdrama/Paths/Open Data Path", priority = 2)]
    private static void OpenDataPath()
    {
        System.Diagnostics.Process.Start(Application.dataPath);
    }

    [MenuItem("Snowdrama/Paths/Open Console Log", priority = 3)]
    private static void OpenConsoleLogPath()
    {
        System.Diagnostics.Process.Start(Application.consoleLogPath);
    }

    [MenuItem("Snowdrama/Paths/Open Cache Path", priority = 9)]
    private static void OpenCachePath()
    {
        System.Diagnostics.Process.Start(Application.temporaryCachePath);
    }

    [MenuItem("Snowdrama/Paths/Open Streaming Asset Path(Probably Broken)", priority = 99)]
    private static void OpenStreamingDataPath()
    {
        System.Diagnostics.Process.Start(Application.streamingAssetsPath);
    }
}
