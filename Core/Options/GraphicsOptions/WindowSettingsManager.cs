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
                var hz = refreshRate.denominator != 0
                    ? (float)refreshRate.numerator / refreshRate.denominator
                    : 0f;

                return $"{width}x{height} @ {hz:0.##}Hz";
            }
        }

        public static List<ResolutionOption> GetOptionsForResolution(ResolutionOption res)
        {
            return Resolutions
                .Where(r => r.width == res.width && r.height == res.height)
                .OrderByDescending(GetRefreshRateHz)
                .ToList();
        }

        private static List<ResolutionOption> _resolutions;

        public static List<ResolutionOption> Resolutions
        {
            get
            {
                if (_resolutions == null)
                {
                    _resolutions = new List<ResolutionOption>();
                }

                if (_resolutions.Count > 0)
                {
                    return _resolutions;
                }

                _resolutions = Screen.resolutions
                    .Select(r => new ResolutionOption
                    {
                        width = r.width,
                        height = r.height,
                        refreshRate = r.refreshRateRatio
                    })
                    .Where(r => r.width > 0 && r.height > 0)
                    // remove duplicates (same width/height/refresh)
                    .GroupBy(r => (r.width, r.height, r.refreshRate.numerator, r.refreshRate.denominator
                    ))
                    .Select(g => g.First())
                    .OrderBy(r => r.width)
                    .ThenBy(r => r.height)
                    .ThenByDescending(GetRefreshRateHz)
                    .ToList();

                DebugLog($"<color=green>Resolutions Generated!: {_resolutions.Count > 0}");

                if (_resolutions.Count == 0 && Screen.width > 0 && Screen.height > 0)
                {
                    _resolutions.Add(new ResolutionOption
                    {
                        width = Screen.width,
                        height = Screen.height,
                        refreshRate = new RefreshRate
                        {
                            numerator = 60,
                            denominator = 1
                        }
                    });
                }

                return _resolutions;
            }
        }

        private static FullScreenMode _fullscreenMode;
        private static int _resolutionIndex = -1;

        public static int ResolutionIndex
        {
            get => _resolutionIndex;
            set
            {
                if (_resolutionIndex != value)
                {
                    _resolutionIndex = value;
                }
            }
        }

        public static ResolutionOption CurrentResolution
        {
            get
            {
                if (Resolutions.Count == 0)
                {
                    return new ResolutionOption
                    {
                        width = Screen.width,
                        height = Screen.height,
                        refreshRate = new RefreshRate
                        {
                            numerator = 60,
                            denominator = 1
                        }
                    };
                }

                if (!IsValidIndex(ResolutionIndex))
                {
                    ResolutionIndex = GetLargestScreenSize();
                }

                return Resolutions[ResolutionIndex];
            }
        }

        public static FullScreenMode CurrentFullScreenMode => _fullscreenMode;

        public const string RESOLUTION_SETTING_KEY = "ResolutionIndex";
        public const string FULLSCREEN_SETTING_KEY = "FullscreenMode";

        [Header("Debug")]
        [SerializeField, EditorReadOnly] private Vector2 _debugCurrentWindowSize = Vector2.zero;
        [SerializeField, EditorReadOnly] private Vector2 _debugMonitorResolution = Vector2.zero;

        [Header("Current Selection Debug")]
        [SerializeField, EditorReadOnly] private Vector2 _debugCurrentSelectedResolution = Vector2.zero;
        [SerializeField, EditorReadOnly] private int _debugCurrentSelectedResolutionIndex = 0;
        [SerializeField] private List<ResolutionOption> _debugResolutions = new();

        private static WindowSettingsManagerSettingJSON _settings = new WindowSettingsManagerSettingJSON
        {
            ShowConsoleMessages = true
        };

        private void Update()
        {
            _debugCurrentWindowSize = new Vector2(Screen.width, Screen.height);
            _debugMonitorResolution = new Vector2(
                Screen.currentResolution.width,
                Screen.currentResolution.height
            );

            var current = CurrentResolution;
            _debugCurrentSelectedResolution = new Vector2(current.width, current.height);
            _debugCurrentSelectedResolutionIndex = ResolutionIndex;

            if (_debugResolutions.Count != Resolutions.Count)
            {
                _debugResolutions.Clear();
                _debugResolutions.AddRange(Resolutions);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            var settingsJson = Resources.Load<TextAsset>("WindowSettingsManagerSettings");

            if (settingsJson == null)
            {
                Debug.LogWarning(
                    "WindowSettingsManagerSettings was not found in a Resources folder. " +
                    "Using default settings."
                );
            }
            else
            {
                try
                {
                    _settings = JsonConvert.DeserializeObject<WindowSettingsManagerSettingJSON>(
                        settingsJson.text
                    );
                }
                catch (JsonException exception)
                {
                    Debug.LogError(
                        $"Failed to read WindowSettingsManagerSettings: {exception.Message}"
                    );
                }
            }

            LoadSettings();
            ApplyResolution();
        }

        private static void LoadSettings()
        {
#if !UNITY_EDITOR
            ResolutionIndex = Options.GetIntValue(RESOLUTION_SETTING_KEY, -1);
#endif

            if (!IsValidIndex(ResolutionIndex))
            {
                ResolutionIndex = GetLargestScreenSize();

#if !UNITY_EDITOR
                Options.SetIntValue(RESOLUTION_SETTING_KEY, ResolutionIndex);
#endif
            }

            _fullscreenMode = (FullScreenMode)Options.GetIntValue(
                FULLSCREEN_SETTING_KEY,
                (int)FullScreenMode.FullScreenWindow
            );
        }

        public static void SetResolution(int index)
        {
            if (Resolutions.Count == 0)
            {
                DebugLogWarning("Cannot set a resolution because no resolution options are available.");
                return;
            }

            if (!IsValidIndex(index))
            {
                index = GetLargestScreenSize();
            }

            ResolutionIndex = index;

#if !UNITY_EDITOR
            Options.SetIntValue(RESOLUTION_SETTING_KEY, index);
#endif

            ApplyResolution();
        }

        public static void SetFullscreenMode(FullScreenMode mode)
        {
            _fullscreenMode = mode;

#if !UNITY_EDITOR
            // Save the fullscreen mode under its own setting key.
            Options.SetIntValue(FULLSCREEN_SETTING_KEY, (int)mode);
#endif

            ApplyResolution();
        }

        private static void ApplyResolution()
        {
            if (Resolutions.Count == 0 || !IsValidIndex(ResolutionIndex))
            {
                return;
            }

            var res = Resolutions[ResolutionIndex];

            if (_fullscreenMode == FullScreenMode.ExclusiveFullScreen)
            {
                Screen.SetResolution(
                    res.width,
                    res.height,
                    _fullscreenMode,
                    res.refreshRate
                );
            }
            else
            {
                Screen.SetResolution(res.width, res.height, _fullscreenMode);
            }
        }

        private static bool IsValidIndex(int index)
        {
            return index >= 0 && index < Resolutions.Count;
        }

        private static int GetLargestScreenSize()
        {
            if (Resolutions.Count == 0)
            {
                return -1;
            }

            var targetWidth = Screen.currentResolution.width;
            var targetHeight = Screen.currentResolution.height;

            // Prefer the exact current display size, using the highest refresh rate.
            var matchingIndices = Enumerable.Range(0, Resolutions.Count)
                .Where(i =>
                    Resolutions[i].width == targetWidth &&
                    Resolutions[i].height == targetHeight)
                .OrderByDescending(i => GetRefreshRateHz(Resolutions[i]))
                .ToList();

            if (matchingIndices.Count > 0)
            {
                return matchingIndices[0];
            }

            // Otherwise choose the option with the greatest pixel area,
            // breaking ties by preferring the highest refresh rate.
            var bestIndex = 0;

            for (var i = 1; i < Resolutions.Count; i++)
            {
                var candidate = Resolutions[i];
                var best = Resolutions[bestIndex];

                var candidateArea = (long)candidate.width * candidate.height;
                var bestArea = (long)best.width * best.height;

                if (candidateArea > bestArea ||
                    (candidateArea == bestArea &&
                     GetRefreshRateHz(candidate) > GetRefreshRateHz(best)))
                {
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        private static float GetRefreshRateHz(ResolutionOption resolution)
        {
            if (resolution.refreshRate.denominator == 0)
            {
                return 0f;
            }

            return (float)resolution.refreshRate.numerator / resolution.refreshRate.denominator;
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem(
            "Snowdrama/Required/Create WindowSettingsManager Settings JSON(In Resources)"
        )]
        public static void CreateSupportedLanguagesJSON()
        {
            ProjectViewUtils.OpenFolderInProjectView("Assets/Resources");

            var defaultData = new WindowSettingsManagerSettingJSON
            {
                ShowConsoleMessages = true
            };

            var dataString = JsonConvert.SerializeObject(
                defaultData,
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                }
            );

            const string path = "Assets/Resources/WindowSettingsManagerSettings.jsonc";

            if (!File.Exists(path))
            {
                Directory.CreateDirectory("Assets/Resources");
                File.WriteAllText(path, dataString);
                UnityEditor.AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError(
                    "WindowSettingsManagerSettings.jsonc already exists and was not overwritten. " +
                    "Delete it manually only if you intend to recreate it."
                );
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
