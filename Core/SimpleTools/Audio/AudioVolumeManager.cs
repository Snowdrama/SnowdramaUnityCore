using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioChannelData
{
    public string optionsKey = "MusicVolume";
    public string mixerChannelName = "Music";
    public float defaultValue = 1.0f;
}
public class AudioVolumeManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    public OptionsObject options;

    public List<AudioChannelData> audioChannelData = new List<AudioChannelData>();
    void Update()
    {
        foreach (var item in audioChannelData)
        {
            //0 to 1 comverted logarithmically to -80 to 0 DB
            audioMixer.SetFloat(item.mixerChannelName, Mathf.Log10(options.GetFloatValue(item.optionsKey, item.defaultValue) * 100) * 20);
        }
    }
}
