using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Options
{
    //these are shortcuts for the volume option keys
    //so Option.SetFloat(Options.MASTER_VOLUME_KEY, 0.5f);
    public const string MASTER_VOLUME_KEY = "MasterVolume";
    public const string MUSIC_VOLUME_KEY = "MusicVolume";
    public const string SOUND_VOLUME_KEY = "SoundVolume";
    public const string VOICE_VOLUME_KEY = "VoiceVolume";
    public const string FOOTSTEP_VOLUME_KEY = "FootstepVolume";
    public const string AMBIENCE_VOLUME_KEY = "AmbienceVolume";

    //these are some shortcuts for the name of the bus in the mixer
    //so var mixerGroups = Mixer.FindMatchingGroups(busName);
    //Also used in PlaySoundMessage:
    //Messages
    //    .GetOnce<PlaySoundMessage>()
    //    .Dispatch(audioClip, Options.MASTER_MIXER_BUS_KEY, 1.0f);
    public const string MASTER_MIXER_BUS_KEY = "Master";
    public const string MUSIC_MIXER_BUS_KEY = "Music";
    public const string SOUND_MIXER_BUS_KEY = "Sound";
    public const string VOICE_MIXER_BUS_KEY = "Voice";
    public const string FOOTSTEP_MIXER_BUS_KEY = "Footstep";
    public const string AMBIENCE_MIXER_BUS_KEY = "Ambience"; //rain, burning fire etc
    private static Dictionary<Type, IOptionsContainer> data = new Dictionary<Type, IOptionsContainer>();

    public static void Set<T>(string key, T value)
    {
        //make sure we have a container
        if (!data.TryGetValue(typeof(T), out var container))
        {
            container = new OptionsContainer<T>();
            data.Add(typeof(T), container);
        }

        //now that we know this is set, we call set key, value
        ((OptionsContainer<T>)container).Set(key, value);
        Save();
    }
    public static T Get<T>(string key, T defaultValue)
    {
        //make sure we have a container
        if (!data.TryGetValue(typeof(T), out var container))
        {
            return defaultValue;
        }

        //now that we know this is set, we call set key, value
        return ((OptionsContainer<T>)container).Get(key, defaultValue);
    }
    public static void SetCallback<T>(string key, Action<T> callback)
    {
        //make sure we have a container
        if (!data.TryGetValue(typeof(T), out var container))
        {
            container = new OptionsContainer<T>();
            data.Add(typeof(T), container);
        }

        //now that we know this is set, we call set key, value
        ((OptionsContainer<T>)container).SetCallback(key, callback);
    }

    public static void UnsetCallback<T>(string key, Action<T> callback)
    {
        //make sure we have a container
        if (!data.TryGetValue(typeof(T), out var container))
        {
            container = new OptionsContainer<T>();
            data.Add(typeof(T), container);
        }

        //now that we know this is set, we call set key, value
        ((OptionsContainer<T>)container).UnsetCallback(key, callback);
    }
    public static string BSSaveString = "";
    public static void Save()
    {
        var jsonString = CreateSaveString(data);
        File.WriteAllText($"Options.jsonc", jsonString);
    }
    private static string CreateSaveString(Dictionary<Type, IOptionsContainer> data)
    {
        var json = new JObject();
        foreach (var (type, container) in data)
        {
            var typeName = type.AssemblyQualifiedName;
            if (typeName == null)
                continue;

            json[typeName] = container.GetValuesToken(); ;
        }
        var jsonString = json.ToString(Formatting.Indented);

        return jsonString;
    }

    public static void Load()
    {
        if (File.Exists($"Options.jsonc"))
        {
            var loadJSON = File.ReadAllText($"Options.jsonc");
            data = Load(loadJSON);
        }
        else
        {
            data = Load(Resources.Load<TextAsset>("DefaultSaveJSON").text);
        }
    }
    private static Dictionary<Type, IOptionsContainer> Load(string jsonString)
    {
        var root = JObject.Parse(jsonString);
        var loadedData = new Dictionary<Type, IOptionsContainer>();

        foreach (var property in root.Properties())
        {
            var type = Type.GetType(property.Name, throwOnError: false);
            if (type == null)
                continue;

            var containerType = typeof(OptionsContainer<>).MakeGenericType(type);
            var container = (IOptionsContainer?)property.Value.ToObject(containerType);


            if (container != null)
            {
                container.LoadValues(property.Value);
                loadedData[type] = container;
            }
        }

        return loadedData;
    }

    public static void Clear()
    {
        data.Clear();
    }

#if UNITY_EDITOR

    [MenuItem("Snowdrama/Required/Create Default Options JSON")]
    public static void CreateSceneJSON()
    {
        var defaultData = new Dictionary<Type, IOptionsContainer>();

        defaultData.Add(typeof(int), new OptionsContainer<int>()
        {
            values =
            {
                {
                    "screen_setting",
                    0 //0 should be fullscreen
                },
                {
                    "screen_resolution_option",
                    0 //0 should be 1920x1080
                },
            }
        });
        defaultData.Add(typeof(float), new OptionsContainer<float>()
        {
            values =
            {
                //we always want to default with some audio values
                {
                    "master_volume",
                    0.5f
                },
                {
                    "music_volume",
                    0.5f
                },
                {
                    "sound_volume",
                    0.5f
                },
                {
                    "voice_volume",
                    0.5f
                },
                {
                    "footstep_volume",
                    0.5f
                },
                //and some basic input sensitivity stuff
                {
                    "mouse_sensitivity_x",
                    1.0f
                },
                {
                    "mouse_sensitivity_y",
                    1.0f
                },
                {
                    "gamepad_sensitivity_x",
                    1.0f
                },
                {
                    "gamepad_sensitivity_y",
                    1.0f
                },
            }
        });
        defaultData.Add(typeof(bool), new OptionsContainer<bool>()
        {
            values =
            {
                {
                    "invert_x",
                    false
                },
                {
                    "invert_y",
                    false
                },
                //graphics options
                {
                    "vsync",
                    true
                },
            }
        });

        var dataString = CreateSaveString(defaultData);
        if (!File.Exists($"Assets/Resources/DefaultOptions.json"))
        {
            File.WriteAllText($"Assets/Resources/DefaultOptions.json", dataString);
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError($"DANGER! ENSURE YOU ACTUALLY WANT TO DO THIS!!!" +
                $"Can't overwrite DefaultOptions.json because it already exists. " +
                $"If this intended please manually delete the DefaultOptions.json and run again");
        }
    }
#endif
}
