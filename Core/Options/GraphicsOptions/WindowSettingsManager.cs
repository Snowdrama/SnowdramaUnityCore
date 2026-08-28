using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Snowdrama
{
    [System.Serializable]
    public struct WindowSettingsManagerSettingJSON
    {
        public bool ShowConsoleMessages;
    }
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
        public static List<ResolutionOption> GetOptionsForResolution(ResolutionOption res)
        {
            return Resolutions
                .Where(r => r.width == res.width && r.height == res.height)
                .OrderBy(r => r.refreshRate.numerator)
                .ToList();
        }
        private static List<ResolutionOption> _resolutions;
        public static List<ResolutionOption> Resolutions
        {
            get
            {
                //ensure it's not null
                if (_resolutions == null)
                {
                    _resolutions = new List<ResolutionOption>();
                }

                //it's already set up so just return it
                if (_resolutions.Count > 0)
                {
                    //Debug.Log($"<color=#08F>Returning Existing Resolution List...");
                    //Debug.Log($"<color=#08F>_resolutions != null: {_resolutions != null} ");
                    //Debug.Log($"<color=#08F>_resolutions.Count > 0: {_resolutions.Count > 0}");
                    return _resolutions;
                }

                //Debug.Log($"<color=blue>Generating Resolution List...");
                //Debug.Log($"<color=blue>_resolutions != null: {_resolutions != null} ");
                //Debug.Log($"<color=blue>_resolutions.Count > 0: {_resolutions.Count > 0}");

                //if it's null build the list
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
                    .Distinct()
                    .ToList();
                Debug.Log($"<color=green>Resolutions Generated!: {_resolutions.Count > 0}");
                return _resolutions;
            }
        }

        //default is -1 until a valid resolution is chosen
        private static FullScreenMode _fullscreenMode;
        private static int _resolutionIndex = -1;
        public static int ResolutionIndex
        {
            get { return _resolutionIndex; }
            set
            {
                if (_resolutionIndex != value)
                {
                    _resolutionIndex = value;
                    //TODO: Write Resolution To The Options?
                }
            }
        }
        public static ResolutionOption CurrentResolution
        {
            get
            {
                if (ResolutionIndex == -1)
                {
                    //if we're -1 we should try and get the largest screen size
                    ResolutionIndex = GetLargestScreenSize();
                }

                return Resolutions[ResolutionIndex];
            }
        }
        public static FullScreenMode CurrentFullScreenMode => _fullscreenMode;

        public const string RESOLUTION_SETTING_KEY = "ResolutionIndex";
        public const string FULLSCREEN_SETTING_KEY = "FullscreenMode";

        [Header("Debug")]
        [SerializeField, EditorReadOnly] private Vector2 _debugCurrentWindowSize = new Vector2(0, 0);
        [SerializeField, EditorReadOnly] private Vector2 _debugMonitorResolution = new Vector2(0, 0);
        [Header("Current Selection Debug")]
        [SerializeField, EditorReadOnly] private Vector2 _debugCurrentSelectedResolution = new Vector2(0, 0);
        [SerializeField, EditorReadOnly] private int _debugCurrentSelectedResolutionIndex = 0;
        [SerializeField] private List<ResolutionOption> _debugResolutions = new();

        private static WindowSettingsManagerSettingJSON _settings = new WindowSettingsManagerSettingJSON()
        {
            ShowConsoleMessages = true,
        };

        private void Update()
        {
            //this tries to get the window size
            _debugCurrentWindowSize = new Vector2(Screen.width, Screen.height);
            //this attempts to get the monitor resoltion not the current window size
            _debugMonitorResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

            //this is set on start or loaded from the save
            _debugCurrentSelectedResolution = new Vector2(CurrentResolution.width, CurrentResolution.height);
            _debugCurrentSelectedResolutionIndex = ResolutionIndex;

            if (_debugResolutions.Count != Resolutions.Count)
            {
                _debugResolutions.Clear();
                for (var i = 0; i < Resolutions.Count; i++)
                {
                    _debugResolutions.Add(Resolutions[i]);
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            var settingsJson = Resources.Load<TextAsset>("WindowSettingsManagerSettings");
            if (settingsJson == null)
            {
                Debug.LogError("Can't find WindowSettingsManagerSettings.jsonc " +
                    "Please use the menu item: Snowdrama -> Required -> Create WindowSettingsManagerSettings JSON" +
                    "to create one in the Resources folder");
                return;
            }
            _settings = JsonConvert.DeserializeObject<WindowSettingsManagerSettingJSON>(settingsJson.text);
            LoadSettings();
            ApplyResolution();
        }

        private static void LoadSettings()
        {
            //only load from the options dealing with the editor
#if !UNITY_EDITOR
            //only load the screen option in builds
            ResolutionIndex = Options.GetIntValue(RESOLUTION_SETTING_KEY, -1);
#endif

            //if the index isn't valid, we need to try and get the largest screen size
            if (!IsValidIndex(ResolutionIndex))
            {
                ResolutionIndex = GetLargestScreenSize();

#if !UNITY_EDITOR
                //only save the screen option in builds
                Options.SetIntValue(RESOLUTION_SETTING_KEY, ResolutionIndex);
#endif
            }

            //get the full screen mode to see if we're windowed or exclusive fullscreen
            _fullscreenMode = (FullScreenMode)Options.GetIntValue(
                FULLSCREEN_SETTING_KEY,
                (int)FullScreenMode.FullScreenWindow
            );
        }

        public static void SetResolution(int index)
        {
            if (!IsValidIndex(index))
                index = GetLargestScreenSize();

            ResolutionIndex = index;

#if !UNITY_EDITOR
            //only save the screen option in builds
            Options.SetIntValue(RESOLUTION_SETTING_KEY, index);
#endif

            ApplyResolution();
        }

        public static void SetFullscreenMode(FullScreenMode mode)
        {
            _fullscreenMode = mode;
#if !UNITY_EDITOR
            //only save the screen option in builds
            Options.SetIntValue(RESOLUTION_SETTING_KEY, (int)mode);
#endif

            ApplyResolution();
        }

        private static void ApplyResolution()
        {
            if (!IsValidIndex(ResolutionIndex))
                return;

            var res = Resolutions[ResolutionIndex];

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
            return index >= 0 && index < Resolutions.Count;
        }

        /// <summary>
        /// Returns the largest size display from the Resolutions list
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
            DebugLog($"<color=orange>Current Screen Resolution: ({Screen.currentResolution.width}, {Screen.currentResolution.height}) -> (Area: {Screen.currentResolution.width * Screen.currentResolution.height})");
            DebugLog($"<color=orange>Current Window Size: ({Screen.width}, {Screen.height}) -> (Area: {Screen.width * Screen.height})");

            for (var i = 0; i < Resolutions.Count; i++)
            {
                var rez = Resolutions[i];
                var hz = (float)rez.refreshRate.denominator / (float)rez.refreshRate.numerator;
                var area = rez.width * rez.height;

                DebugLog($"<color=yellow>Testing[{i}]: ({rez.width}, {rez.height}) @ {hz} -> area: {area} == {targetArea} && hz: {hz} == {bestTargetHzFound}");

                if (targetArea == area)
                {
                    DebugLog($"<color=green>Found the matching the target area index: {i}!");
                    //targetAreaFound = i;

                    if (hz <= bestTargetHzFound)
                    {
                        DebugLog($"<color=green>Found a better screen framerate: {i}!");
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
                DebugLog($"<color=green>Size with matching screen area and Best Hz: {targetAreaFound}!");
                var rez = Resolutions[targetAreaFound];
                var hz = (float)rez.refreshRate.denominator / (float)rez.refreshRate.numerator;
                DebugLog($"<color=green>Returning Resolution: {rez.width}x{rez.height}:{hz:F2}!");
                return targetAreaFound;
            }

            if (largestAreaIndex >= 0)
            {
                DebugLog($"<color=green>Size with largest area: {largestAreaIndex}!");
                return largestAreaIndex;
            }

            DebugLog($"<color=range>Fallback to resolution index: {Resolutions.Count - 1}! " +
                $"({Resolutions[Resolutions.Count - 1].width}, {Resolutions[Resolutions.Count - 1].height})");
            // fallback: highest resolution + highest refresh
            return Resolutions.Count - 1;
        }
#if UNITY_EDITOR

        [UnityEditor.MenuItem("Snowdrama/Required/Create WindowSettingsManager Settings JSON(In Resources)")]
        public static void CreateSupportedLanguagesJSON()
        {
            ProjectViewUtils.OpenFolderInProjectView($"Assets/Resources");

            WindowSettingsManagerSettingJSON defaultData = new()
            {
                ShowConsoleMessages = true,
            };

            var dataString = JsonConvert.SerializeObject(defaultData, new JsonSerializerSettings() { Formatting = Formatting.Indented });
            if (!File.Exists($"Assets/Resources/WindowSettingsManagerSettings.jsonc"))
            {
                File.WriteAllText($"Assets/Resources/WindowSettingsManagerSettings.jsonc", dataString);
                UnityEditor.AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError("DANGER! ENSURE YOU ACTUALLY WANT TO DO THIS!!! " +
                    "Can't overwrite WindowSettingsManagerSettings.jsonc because it already exists! " +
                    "Overwriting this would delete any scene configuration you have! " +
                    "Check the WindowSettingsManagerSettings.json file and ensure you actually want to delete it! " +
                    "If this ACTUALLY intended please manually delete the WindowSettingsManagerSettings.jsonc and run again. ");
            }

        }
#endif
        #region Debug
        private static void DebugLog(string log, GameObject target = null)
        {
            if (_settings.ShowConsoleMessages)
            {
                Debug.Log(log, target);
            }
        }
        private static void DebugLogWarning(string log, GameObject target = null)
        {
            if (_settings.ShowConsoleMessages)
            {
                Debug.LogWarning(log, target);
            }
        }
        private static void DebugLogError(string log, GameObject target = null)
        {
            if (_settings.ShowConsoleMessages)
            {
                Debug.LogError(log, target);
            }
        }
        #endregion
    }
}
