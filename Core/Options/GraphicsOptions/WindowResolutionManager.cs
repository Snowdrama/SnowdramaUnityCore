using UnityEngine;


//updates the window to be windowed or have specific resolutions
public class WindowManager : MonoBehaviour
{

    private void Start()
    {
        //Screen.fullScreen = false;
        //Screen.fullScreenMode = FullScreenMode.Windowed;
        //load the resolution and stuff from settings
        foreach (var res in Screen.resolutions)
        {

        }
        var width = Options.GetIntValue("ScreenWidth", 1920);
        var height = Options.GetIntValue("ScreenHeight", 1080);
        var isFullscreen = Options.GetBoolValue("IsFullscreen", true);
        Screen.SetResolution(width, height, isFullscreen);
    }

    // Update is called once per frame
    private void Update()
    {

    }
}
