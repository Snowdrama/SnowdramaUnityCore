using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Snowdrama
{
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

        [Header("Debug")]
        [SerializeField] private Vector2 _debugCurrentWindowSize = new Vector2(0, 0);
        [SerializeField] private Vector2 _debugMonitorResolution = new Vector2(0, 0);
        [Header("Current Selection Debug")]
        [SerializeField] private Vector2 _debugCurrentSelectedResolution = new Vector2(0, 0);
        [SerializeField] private int _debugCurrentSelectedResolutionIndex = 0;
        [SerializeField] private List<ResolutionOption> _debugResolitions = new();

        private void Update()
        {
            //this tries to get the window size
            _debugCurrentWindowSize = new Vector2(Screen.width, Screen.height);
            //this attempts to get the monitor resoltion not the current window size
            _debugMonitorResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

            //this is set on start or loaded from the save
            _debugCurrentSelectedResolution = new Vector2(CurrentResolution.width, CurrentResolution.height);
            _debugCurrentSelectedResolutionIndex = CurrentResolutionIndex;

            if (_debugResolitions.Count != UniqueResolutions.Count)
            {
                _debugResolitions.Clear();
                for (var i = 0; i < UniqueResolutions.Count; i++)
                {
                    _debugResolitions.Add(UniqueResolutions[i]);
                }
            }
        }

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

            var targetArea = Screen.currentResolution.width * Screen.currentResolution.height;
            var targetAreaFound = -1;

            var bestTargetHzFound = 1.0f;
            //find the largest possible resoltion
            Debug.Log($"<color=orange>Current Screen Resolution: ({Screen.currentResolution.width}, {Screen.currentResolution.height}) -> (Area: {Screen.currentResolution.width * Screen.currentResolution.height})");
            Debug.Log($"<color=orange>Current Window Size: ({Screen.width}, {Screen.height}) -> (Area: {Screen.width * Screen.height})");

            for (var i = 0; i < _resolutions.Count; i++)
            {
                var rez = _resolutions[i];
                var hz = (float)rez.refreshRate.denominator / (float)rez.refreshRate.numerator;
                var area = rez.width * rez.height;

                Debug.Log($"<color=yellow>Testing[{i}]: ({rez.width}, {rez.height}) @ {hz} -> area: {area} == {targetArea} && hz: {hz} == {bestTargetHzFound}");

                if (targetArea == area)
                {
                    Debug.Log($"<color=green>Found the matching the target area index: {i}!");
                    //targetAreaFound = i;

                    if (hz <= bestTargetHzFound)
                    {
                        Debug.Log($"<color=green>Found a better screen framerate: {i}!");
                        targetAreaFound = i;
                        bestTargetHzFound = hz;
                    }
                }

                if (area > largestArea)
                {
                    largestArea = area;
                    largestAreaIndex = i;
                }
            }

            if (targetAreaFound >= 0)
            {
                Debug.Log($"<color=green>Size with matching screen area and Best Hz: {largestAreaIndex}!");
                var rez = _resolutions[targetAreaFound];
                var hz = (float)rez.refreshRate.denominator / (float)rez.refreshRate.numerator;
                return targetAreaFound;
            }

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
}