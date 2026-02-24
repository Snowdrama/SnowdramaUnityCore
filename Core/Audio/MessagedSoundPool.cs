using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlaySoundMessage : AMessage<AudioClip, string, float> { }
public class PlaySoundMessage2D : AMessage<AudioClip, Vector2, string, float> { }
public class PlaySoundMessage3D : AMessage<AudioClip, Vector3, string, float> { }

public partial class MessagedSoundPool : MonoBehaviour
{
    [SerializeField] private int audioPlayerCount = 64;

    [SerializeField] private bool use2DPlayers = true;
    [SerializeField] private bool use3DPlayers = true;

    private List<AudioSource> players = new List<AudioSource>();

    private PlaySoundMessage _playSoundMessage;
    private PlaySoundMessage3D _playSoundMessage3D;
    private PlaySoundMessage2D _playSoundMessage2D;

    [SerializeField] private AudioMixer Mixer;
    public void Awake()
    {
        for (int i = 0; i < audioPlayerCount; i++)
        {
            var newPlayer = new GameObject();
            var audioSource = newPlayer.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            players.Add(audioSource);
            newPlayer.transform.SetParent(this.transform);
        }
        if (use2DPlayers)
        {
            for (int i = 0; i < audioPlayerCount; i++)
            {
                var newPlayer = new GameObject();
                var audioSource = newPlayer.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                players.Add(audioSource);
                newPlayer.transform.SetParent(this.transform);
            }
        }
        if (use3DPlayers)
        {
            for (int i = 0; i < audioPlayerCount; i++)
            {
                var newPlayer = new GameObject();
                var audioSource = newPlayer.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                players.Add(audioSource);
                newPlayer.transform.SetParent(this.transform);
            }
        }

        _playSoundMessage = Messages.Get<PlaySoundMessage>();
        _playSoundMessage.AddListener(PlaySound);
        _playSoundMessage2D = Messages.Get<PlaySoundMessage2D>();
        _playSoundMessage2D.AddListener(PlayPositionedSound2D);
        _playSoundMessage3D = Messages.Get<PlaySoundMessage3D>();
        _playSoundMessage3D.AddListener(PlayPositionedSound3D);
    }

    public void OnDestroy()
    {
        _playSoundMessage.RemoveListener(PlaySound);
        _playSoundMessage2D.RemoveListener(PlayPositionedSound2D);
        _playSoundMessage3D.RemoveListener(PlayPositionedSound3D);


        Messages.Return<PlaySoundMessage>();
        Messages.Return<PlaySoundMessage3D>();
        Messages.Return<PlaySoundMessage2D>();
    }

    public void PlaySound(AudioClip stream, string busName, float volume)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (!players[i].isPlaying)
            {

                players[i].volume = volume;

                var mixerGroups = Mixer.FindMatchingGroups(busName);
                if (mixerGroups != null && mixerGroups.Length > 0)
                {
                    players[i].outputAudioMixerGroup = mixerGroups[0];
                }
                else
                {
                    players[i].outputAudioMixerGroup = null;
                }

                players[i].clip = stream;
                players[i].spatializePostEffects = false;
                players[i].spatialBlend = 0.0f;
                players[i].transform.position = new Vector2(0, 0);
                players[i].Play();
                return;
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="playPosition"></param>
    /// <param name="busName"></param>
    /// <param name="volume">Value 0-1 percentage of the bus volume so 100% volume at 50% bus volume is 50% volume</param>
    public void PlayPositionedSound2D(AudioClip stream, Vector2 playPosition, string busName, float volume)
    {
        if (!use2DPlayers)
        {
            Debug.LogError("Tried to play a 2D sound but not using 2D sound pool, See MessagedSoundPool");
            return;
        }
        for (int i = 0; i < players.Count; i++)
        {
            if (!players[i].isPlaying)
            {
                players[i].volume = volume;

                var mixerGroups = Mixer.FindMatchingGroups(busName);
                if (mixerGroups != null && mixerGroups.Length > 0)
                {
                    players[i].outputAudioMixerGroup = mixerGroups[0];
                }
                else
                {
                    players[i].outputAudioMixerGroup = null;
                }
                players[i].clip = stream;
                players[i].spatializePostEffects = true;
                players[i].spatialBlend = 1.0f;
                players[i].transform.position = playPosition;
                players[i].Play();
                return;
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="playPosition"></param>
    /// <param name="busName">The name of the bus to use, if none is used</param>
    /// <param name="volume">Value 0-1 percentage of the bus volume so 100% volume at 50% bus volume is 50% volume</param>
    public void PlayPositionedSound3D(AudioClip stream, Vector3 playPosition, string busName, float volume)
    {
        if (!use3DPlayers)
        {
            Debug.LogError("Tried to play a 3D sound but not using 3D sound pool, See MessagedSoundPool");
            return;
        }

        for (int i = 0; i < players.Count; i++)
        {
            if (!players[i].isPlaying)
            {
                players[i].volume = volume;

                var mixerGroups = Mixer.FindMatchingGroups(busName);
                if (mixerGroups != null && mixerGroups.Length > 0)
                {
                    players[i].outputAudioMixerGroup = mixerGroups[0];
                }
                else
                {
                    players[i].outputAudioMixerGroup = null;
                }
                players[i].clip = stream;
                players[i].spatializePostEffects = true;
                players[i].spatialBlend = 1.0f;
                players[i].transform.position = playPosition;
                players[i].Play();
                return;
            }
        }
    }
}
