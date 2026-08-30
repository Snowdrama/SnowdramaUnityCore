using UnityEngine;
using UnityEngine.Audio;

namespace Snowdrama
{
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
                Options.MASTER_VOLUME_KEY,
                new AudioChannelData()
                {
                    mixerChannelName = Options.MASTER_VOLUME_KEY,
                    defaultValue = 0.5f
                }
            },

            {
                Options.MUSIC_VOLUME_KEY,
                new AudioChannelData()
                {
                    mixerChannelName = Options.MUSIC_VOLUME_KEY,
                    defaultValue = 0.5f
                }
            },
            {
                Options.SOUND_VOLUME_KEY,
                new AudioChannelData()
                {
                    mixerChannelName = Options.SOUND_VOLUME_KEY,
                    defaultValue = 0.5f
                }
            },
            {
                Options.VOICE_VOLUME_KEY,
                new AudioChannelData()
                {
                    mixerChannelName = Options.VOICE_VOLUME_KEY,
                    defaultValue = 0.5f
                }
            },
            {
                Options.AMBIENCE_VOLUME_KEY,
                new AudioChannelData()
                {
                    mixerChannelName = Options.AMBIENCE_VOLUME_KEY,
                    defaultValue = 0.5f
                }
            },
        };

        private void Start()
        {
            //load each 
            foreach (var data in audioChannels)
            {
                //get the value and if not found use the default
                var linearVolume = Options.GetFloatValue(data.Value.mixerChannelName, data.Value.defaultValue);
                audioMixer.SetFloat(data.Key, linearVolume.LinearToDecibel());
            }

            foreach (var data in audioChannels)
            {
                Options.RegisterFloatOptionCallback(data.Key, this.VolumeValueChanged);
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
}
