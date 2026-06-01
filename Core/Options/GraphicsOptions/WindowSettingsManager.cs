using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class WindowSettingsManager : MonoBehaviour
{
    [System.Serializable]
    public struct ResolutionOption
    {
        public int width;
        public int height;
        public RefreshRate refreshRate;

        public override string ToString()
        {
            return $"{width}x{height} @ {refreshRate.numerator / refreshRate.denominator}Hz";
        }
    }
    public static IReadOnlyList<ResolutionOption> UniqueResolutions => _resolutions.Distinct().ToList();
    public static List<ResolutionOption> GetOptionsForResolution(ResolutionOption res)
    {
        return _resolutions
            .Where(r => r.width == res.width && r.height == res.height)
            .OrderBy(r => r.refreshRate.numerator)
            .ToList();
    }
    public static IReadOnlyList<ResolutionOption> Resolutions => _resolutions;

    private static List<ResolutionOption> _resolutions = new();

    private static int _resolutionIndex;
    private static FullScreenMode _fullscreenMode;
    public static int CurrentResolutionIndex => _resolutionIndex;
    public static ResolutionOption CurrentResolution => UniqueResolutions[CurrentResolutionIndex];
    public static FullScreenMode CurrentFullScreenMode => _fullscreenMode;

    public const string RESOLUTION_SETTING_KEY = "ResolutionIndex";
    public const string FULLSCREEN_SETTING_KEY = "FullscreenMode";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        BuildResolutionList();
        LoadSettings();
        ApplyResolution();
    }

    private static void BuildResolutionList()
    {
        _resolutions = Screen.resolutions
            .Select(r => new ResolutionOption
            {
                width = r.width,
                height = r.height,
                refreshRate = r.refreshRateRatio
            })
            // remove duplicates (same width/height/refresh)
            .GroupBy(r => (r.width, r.height, r.refreshRate.numerator, r.refreshRate.denominator))
            .Select(g => g.First())
            // sort nicely for UI
            .OrderBy(r => r.width)
            .ThenBy(r => r.height)
            .ThenBy(r => r.refreshRate.numerator)
            .ToList();
    }

    private static void LoadSettings()
    {
        _resolutionIndex = Options.GetIntValue(RESOLUTION_SETTING_KEY, -1);

        if (!IsValidIndex(_resolutionIndex))
        {
            _resolutionIndex = GetLargestScreenSize();
            Options.SetIntValue(RESOLUTION_SETTING_KEY, _resolutionIndex);
        }

        _fullscreenMode = (FullScreenMode)Options.GetIntValue(
            FULLSCREEN_SETTING_KEY,
            (int)FullScreenMode.FullScreenWindow
        );
    }

    public static void SetResolution(int index)
    {
        if (!IsValidIndex(index))
            index = GetLargestScreenSize();

        _resolutionIndex = index;
        Options.SetIntValue(RESOLUTION_SETTING_KEY, index);

        ApplyResolution();
    }

    public static void SetFullscreenMode(FullScreenMode mode)
    {
        _fullscreenMode = mode;
        Options.SetIntValue(FULLSCREEN_SETTING_KEY, (int)mode);

        ApplyResolution();
    }

    private static void ApplyResolution()
    {
        if (!IsValidIndex(_resolutionIndex))
            return;

        var res = _resolutions[_resolutionIndex];

        // IMPORTANT: Refresh rate only matters in ExclusiveFullScreen
        if (_fullscreenMode == FullScreenMode.ExclusiveFullScreen)
        {
            Screen.SetResolution(res.width, res.height, _fullscreenMode, res.refreshRate);
        }
        else
        {
            // Borderless/windowed ignores refresh rate
            Screen.SetResolution(res.width, res.height, _fullscreenMode);
        }
    }

    private static bool IsValidIndex(int index)
    {
        return index >= 0 && index < _resolutions.Count;
    }


    /// <summary>
    /// Returns the largest size display from the _resolutions list
    /// </summary>
    /// <returns></returns>
    private static int GetLargestScreenSize()
    {
        //var bestIndexFoundWithRefresh = -1;
        //var largestWidth = 0.0f;
        //var largestHeight = 0.0f;
        //var largestRefreshRate = 0.0f;
        var largestArea = 0.0f;
        var largestAreaIndex = 0;
        //find the largest possible resoltion
        for (var i = 0; i < _resolutions.Count; i++)
        {
            var r = _resolutions[i];
            var hz = (float)r.refreshRate.denominator / (float)r.refreshRate.numerator;
            var area = r.width * r.height;

            if (area > largestArea)
            {
                largestArea = area;
                largestAreaIndex = i;
            }
            //Debug.Log($"<color=orange>Testing[{i}]: ({r.width}, {r.height}) -> ({largestWidth}, {largestHeight}) @ {largestRefreshRate}");
            //if (r.width >= largestWidth)
            //{
            //    largestWidth = r.width;
            //}
            //if (r.height >= largestHeight)
            //{
            //    largestHeight = r.height;
            //}
            //if (hz >= largestRefreshRate)
            //{
            //    largestRefreshRate = hz;
            //}
        }

        //Debug.Log($"<color=green>Best Possible Resolution: ({largestWidth}, {largestHeight}) -> ({largestRefreshRate}) | By Area Best Index {largestAreaIndex}");

        //var bestCurrentWidth = 0.0f;
        //var bestCurrentHeight = 0.0f;
        //var bestCurrentRefreshRate = 0.0f;
        //for (var i = 0; i < _resolutions.Count; i++)
        //{
        //    var r = _resolutions[i];
        //    var hz = (float)r.refreshRate.denominator / (float)r.refreshRate.numerator;

        //    Debug.Log($"<color=magenta>Comparing [{i}] Against Theoretical Largest: ({r.width}, {r.height}) @ {hz} == ({largestWidth}, {largestHeight}) @ {largestRefreshRate} ");

        //    if (
        //        r.width <= largestWidth && r.width >= bestCurrentWidth &&
        //        r.height <= largestHeight && r.height >= bestCurrentHeight &&
        //        hz <= largestRefreshRate && hz >= bestCurrentRefreshRate)
        //    {
        //        bestCurrentWidth = r.width;
        //        bestCurrentHeight = r.height;
        //        bestCurrentRefreshRate = hz;
        //        bestIndexFoundWithRefresh = i;
        //    }
        //}

        if (largestAreaIndex >= 0)
        {
            Debug.Log($"<color=green>Size with largest area: {largestAreaIndex}!");
            return largestAreaIndex;
        }

        Debug.Log($"<color=range>Fallback to resolution index: {_resolutions.Count - 1}! " +
            $"({_resolutions[_resolutions.Count - 1].width}, {_resolutions[_resolutions.Count - 1].height})");
        // fallback: highest resolution + highest refresh
        return _resolutions.Count - 1;
    }
}