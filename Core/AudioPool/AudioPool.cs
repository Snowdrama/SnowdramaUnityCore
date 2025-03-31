using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Audio;

public class PlaySoundSignal : ASignal<AudioClip, string, float> {}
public class PlaySoundSignal2D : ASignal<AudioClip, Vector2, string, float> {}
public class PlaySoundSignal3D : ASignal<AudioClip, Vector3, string, float> {}

public partial class MessagedSoundPool : MonoBehaviour
{
    [SerializeField] int audioPlayerCount = 64;

    [SerializeField] bool use2DPlayers = true;
    [SerializeField] bool use3DPlayers = true;

    List<AudioSource> players = new List<AudioSource>();

    PlaySoundSignal _playSoundSignal;
    PlaySoundSignal3D _playSoundSignal3D;
    PlaySoundSignal2D _playSoundSignal2D;

    [SerializeField] AudioMixer Mixer;
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

        _playSoundSignal = Signals.Get<PlaySoundSignal>();
        _playSoundSignal.AddListener(PlaySound);
        _playSoundSignal2D = Signals.Get<PlaySoundSignal2D>();
        _playSoundSignal2D.AddListener(PlayPositionedSound2D);
        _playSoundSignal3D = Signals.Get<PlaySoundSignal3D>();
        _playSoundSignal3D.AddListener(PlayPositionedSound3D);
    }

    public void OnDestroy()
    {
        _playSoundSignal.RemoveListener(PlaySound);
        _playSoundSignal2D.RemoveListener(PlayPositionedSound2D);
        _playSoundSignal3D.RemoveListener(PlayPositionedSound3D);


        Signals.Return<PlaySoundSignal>();
        Signals.Return<PlaySoundSignal3D>();
        Signals.Return<PlaySoundSignal2D>();
    }

    public void PlaySound(AudioClip stream, string busName, float volume)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (!players[i].isPlaying)
            {
                var mixerGroups = Mixer.FindMatchingGroups(busName);

                var busVolumeDb = Mathf.Lerp(-80, 0, volume / 100);
                players[i].volume = busVolumeDb;

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
                players[i].transform.position = new Vector2(0,0);
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
    /// <param name="volume">Value 0-100 percentage of the bus volume so 100% volume at 50% bus volume is 50% volume</param>
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
                var mixerGroups = Mixer.FindMatchingGroups(busName);

                var busVolumeDb = Mathf.Lerp(-80, 0, volume / 100);
                players[i].volume = busVolumeDb;

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
    /// <param name="busName"></param>
    /// <param name="volume">Value 0-100 percentage of the bus volume so 100% volume at 50% bus volume is 50% volume</param>
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
                var mixerGroups = Mixer.FindMatchingGroups(busName);

                var busVolumeDb = Mathf.Lerp(-80, 0, volume / 100);
                players[i].volume = busVolumeDb;

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
