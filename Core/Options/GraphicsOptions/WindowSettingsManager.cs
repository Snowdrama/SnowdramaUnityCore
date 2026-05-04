using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            _resolutionIndex = FindClosestMatch();
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
            index = FindClosestMatch();

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

    private static int FindClosestMatch()
    {
        Debug.Log($"<color=red>Finding closest resolution match to: ({Screen.width}, {Screen.height})");
        var bestIndexFound = -1;
        var bestIndexFoundWithRefresh = -1;
        for (var i = 0; i < _resolutions.Count; i++)
        {
            var r = _resolutions[i];

            Debug.Log($"<color=orange>Testing: ({r.width}, {r.height}) -> ({Screen.width}, {Screen.height})");
            if (r.width == Screen.width && r.height == Screen.height)
            {
                Debug.Log("<color=green>Resolution Match!");
                // try to match refresh rate too if possible
                bestIndexFound = i;
                if (ApproximatelyEqual(r.refreshRate, Screen.currentResolution.refreshRateRatio))
                {
                    Debug.Log("<color=green>Refresh Match!");
                    bestIndexFoundWithRefresh = i;
                }
            }
        }

        if (bestIndexFoundWithRefresh >= 0)
        {
            Debug.Log($"<color=green>Best Found With Refresh: {bestIndexFoundWithRefresh}!");
            return bestIndexFoundWithRefresh;
        }
        else if (bestIndexFound >= 0)
        {
            Debug.Log($"<color=green>Best Found: {bestIndexFound}!");
            return bestIndexFound;
        }

        Debug.Log($"<color=range>Fallback to resolution index: {_resolutions.Count - 1}!");
        // fallback: highest resolution + highest refresh
        return _resolutions.Count - 1;
    }

    private static bool ApproximatelyEqual(RefreshRate a, RefreshRate b)
    {
        return a.numerator == b.numerator && a.denominator == b.denominator;
    }
}