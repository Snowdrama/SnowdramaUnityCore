using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioChannelData
{
    public string mixerChannelName = "Music";
    public float defaultValue = 1.0f;
}


public class AudioVolumeManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public UnitySerializedDictionary<string, AudioChannelData> audioChannels = new UnitySerializedDictionary<string, AudioChannelData>()
    {
        {
            "MasterVolume",
            new AudioChannelData()
            {
                mixerChannelName = "MasterVolume",
                defaultValue = 1.0f
            }
        },

        {
            "MusicVolume",
            new AudioChannelData()
            {
                mixerChannelName = "MusicVolume",
                defaultValue = 1.0f
            }
        },
        {
            "SoundVolume",
            new AudioChannelData()
            {
                mixerChannelName = "SoundVolume",
                defaultValue = 1.0f
            }
        },
        {
            "VoiceVolume",
            new AudioChannelData()
            {
                mixerChannelName = "VoiceVolume",
                defaultValue = 1.0f
            }
        },
    };

    private void Start()
    {
        foreach (var data in audioChannels)
        {
            Options.RegisterFloatOptionCallback(data.Key, VolumeValueChanged);
        }
    }

    private void VolumeValueChanged(string name, float volume)
    {
        if (audioChannels.ContainsKey(name))
        {
            var chanel = audioChannels[name];
            audioMixer.SetFloat(chanel.mixerChannelName, volume.LinearToDecibel());
        }
    }
}
