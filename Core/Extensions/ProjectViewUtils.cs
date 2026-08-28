using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
/// <summary>
/// Thanks to: https://discussions.unity.com/t/tutorial-how-to-to-show-specific-folder-content-in-the-project-window-via-editor-scripting/685248
/// With some additions to make it work more often in Unity 6+.
/// </summary>
public static class ProjectViewUtils
{
    public static void OpenFolderInProjectView(string folderPath)
    {
#if UNITY_EDITOR
        // Load the folder as an object
        var folder = AssetDatabase.LoadAssetAtPath<Object>(folderPath);
        OpenFolderInProjectView(folder);
#endif
    }

    public static void OpenFolderInProjectView(UnityEngine.Object folder)
    {
#if UNITY_EDITOR
        // Load the folder as an object
        if (folder == null)
            return;

        // First try via assets inside.
        SelectFirstAssetInFolder(folder); // May or may not be needed.

        // Then try the folder directly. The delay makes it work more often
        EditorApplication.delayCall += () =>
        {
            ShowFolderContents(folder.GetInstanceID());
            Selection.activeObject = null;
        };
#endif
    }

    /// <summary>
    /// Selects the first asset inside the given folder.
    /// </summary>
    /// <param name="folder">The folder object (must be a folder in the Project view)</param>
    public static void SelectFirstAssetInFolder(Object folder)
    {
#if UNITY_EDITOR
        var folderPath = AssetDatabase.GetAssetPath(folder);

        var guids = AssetDatabase.FindAssets("t:Object", new[] { folderPath });
        if (guids.Length == 0)
            return;

        var firstAssetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        var firstAsset = AssetDatabase.LoadAssetAtPath<Object>(firstAssetPath);

        // Select
        if (firstAsset != null)
        {
            Selection.activeObject = firstAsset;
        }
#endif
    }

    /// <summary>
    /// Selects a folder in the project window and shows its content.
    /// Opens a new project window, if none is open yet.
    /// </summary>
    /// <param name="folderInstanceID">The instance of the folder asset to open.</param>
    private static void ShowFolderContents(int folderInstanceID)
    {
#if UNITY_EDITOR
        // Find the internal ProjectBrowser class in the editor assembly.
        var editorAssembly = typeof(Editor).Assembly;
        var projectBrowserType = editorAssembly.GetType("UnityEditor.ProjectBrowser");

        // Abort if reflection failed.
        if (projectBrowserType == null)
            return;

        // This is the internal method, which performs the desired action.
        // Should only be called if the project window is in two column mode.
        var showFolderContents = projectBrowserType.GetMethod(
            "ShowFolderContents", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        // Abort if reflection failed.
        if (showFolderContents == null)
            return;

        // Find any open project browser windows.
        var projectBrowserInstances = Resources.FindObjectsOfTypeAll(projectBrowserType);

        if (projectBrowserInstances.Length > 0)
        {
            for (var i = 0; i < projectBrowserInstances.Length; i++)
            {
                var window = projectBrowserInstances[i];

                // Skip if locked.
                if (IsProjectWindowLocked(window, projectBrowserType, true))
                    continue;

                ShowFolderContentsInternal(window, showFolderContents, folderInstanceID);
            }
        }
        else
        {
            var projectBrowser = OpenNewProjectBrowser(projectBrowserType);
            ShowFolderContentsInternal(projectBrowser, showFolderContents, folderInstanceID);
        }
#endif
    }

    public static bool IsProjectWindowLocked(Object projectBrowserInstance, System.Type projectBrowserType, bool defaultValue = true)
    {
#if UNITY_EDITOR
        var isLockedProperty = projectBrowserType.GetProperty("isLocked", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        // Assume default if reflection failed.
        if (isLockedProperty == null)
            return defaultValue;

        var isLocked = (bool)isLockedProperty.GetValue(projectBrowserInstance, null);
        return isLocked;
#endif
    }

    private static void ShowFolderContentsInternal(Object projectBrowser, MethodInfo showFolderContents, int folderInstanceID)
    {
#if UNITY_EDITOR
        // Sadly, there is no method to check for the view mode.
        // We can use the serialized object to find the private property.
        var serializedObject = new SerializedObject(projectBrowser);
        var inTwoColumnMode = serializedObject.FindProperty("m_ViewMode").enumValueIndex == 1;

        if (!inTwoColumnMode)
        {
            // If the browser is not in two column mode, we must set it to show the folder contents.
            var setTwoColumns = projectBrowser.GetType().GetMethod(
                "SetTwoColumns", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            setTwoColumns.Invoke(projectBrowser, null);
        }

        var revealAndFrameInFolderTree = true;
        EntityId folderEntityId = folderInstanceID;
        showFolderContents.Invoke(projectBrowser, new object[] { folderEntityId, revealAndFrameInFolderTree });
#endif
    }

#if UNITY_EDITOR
    private static EditorWindow OpenNewProjectBrowser(System.Type projectBrowserType)
    {
        var projectBrowser = EditorWindow.GetWindow(projectBrowserType);
        projectBrowser.Show();

        // Unity does some special initialization logic, which we must call,
        // before we can use the ShowFolderContents method (else we get a NullReferenceException).
        var init = projectBrowserType.GetMethod("Init", BindingFlags.Instance | BindingFlags.Public);
        init.Invoke(projectBrowser, null);

        return projectBrowser;
    }
#endif
}