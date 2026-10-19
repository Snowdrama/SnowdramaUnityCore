using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class PlaySoundMessage : AMessage<(AudioClip clip, string mixerName, float volume, float pitch)> { }
public class PlaySoundMessage2D : AMessage<(AudioClip clip, Vector2 position, string mixerName, float volume, float pitch)> { }
public class PlaySoundMessage3D : AMessage<(AudioClip clip, Vector3 position, string mixerName, float volume, float pitch)> { }

public partial class MessagedSoundPool : MonoBehaviour
{
    [SerializeField] private int audioPlayerCount = 64;

    [SerializeField] private bool use2DPlayers = true;
    [SerializeField] private bool use3DPlayers = true;

    private List<AudioSource> players = new List<AudioSource>();
    private Queue<AudioSource> unusedPlayers = new Queue<AudioSource>();
    private List<AudioSource> usedPlayers = new List<AudioSource>();

    public void Update()
    {
        foreach (var player in players)
        {
            //if it's not playing it should
            //be added back to unusedPlayers
            if (!player.isPlaying)
            {
                unusedPlayers.Enqueue(player);
            }
        }
        usedPlayers = usedPlayers.Where(x => !x.isPlaying).ToList();
    }

    private PlaySoundMessage _playSoundMessage;
    private PlaySoundMessage3D _playSoundMessage3D;
    private PlaySoundMessage2D _playSoundMessage2D;

    [SerializeField] private AudioMixer Mixer;
    public void Awake()
    {
        for (var i = 0; i < audioPlayerCount; i++)
        {
            var newPlayer = new GameObject();
            var audioSource = newPlayer.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            players.Add(audioSource);
            newPlayer.transform.SetParent(this.transform);
        }
        if (use2DPlayers)
        {
            for (var i = 0; i < audioPlayerCount; i++)
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
            for (var i = 0; i < audioPlayerCount; i++)
            {
                var newPlayer = new GameObject();
                var audioSource = newPlayer.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                players.Add(audioSource);
                newPlayer.transform.SetParent(this.transform);
            }
        }
    }
    public void OnEnable()
    {
        _playSoundMessage = Messages.Get<PlaySoundMessage>();
        _playSoundMessage.AddListener(this.PlaySound);
        _playSoundMessage2D = Messages.Get<PlaySoundMessage2D>();
        _playSoundMessage2D.AddListener(this.PlayPositionedSound2D);
        _playSoundMessage3D = Messages.Get<PlaySoundMessage3D>();
        _playSoundMessage3D.AddListener(this.PlayPositionedSound3D);
    }

    public void OnDisable()
    {
        _playSoundMessage.RemoveListener(this.PlaySound);
        _playSoundMessage2D.RemoveListener(this.PlayPositionedSound2D);
        _playSoundMessage3D.RemoveListener(this.PlayPositionedSound3D);

        Messages.Return<PlaySoundMessage>();
        Messages.Return<PlaySoundMessage3D>();
        Messages.Return<PlaySoundMessage2D>();
    }

    public void PlaySound((AudioClip stream, string busName, float volume, float pitch) info)
    {
        if (players.Count <= 0)
        {
            Debug.LogError("Tried to play a sound but there's no players");
            return;
        }

        //Get an unused player
        if (unusedPlayers.Count > 0)
        {
            var player = unusedPlayers.Dequeue();
            usedPlayers.Add(player);
            if (player != null)
            {
                player.volume = info.volume;
                var mixerGroups = Mixer.FindMatchingGroups(info.busName);
                if (mixerGroups != null && mixerGroups.Length > 0)
                {
                    player.outputAudioMixerGroup = mixerGroups[0];
                }
                else
                {
                    player.outputAudioMixerGroup = null;
                }

                player.clip = info.stream;
                player.spatializePostEffects = false;
                player.spatialBlend = 0.0f;
                player.pitch = info.pitch;
                player.transform.position = new Vector2(0, 0);
                player.Play();
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
    public void PlayPositionedSound2D((AudioClip stream, Vector2 playPosition, string busName, float volume, float pitch) info)
    {
        if (!use2DPlayers)
        {
            Debug.LogError("Tried to play a 2D sound but not using 2D sound pool, See MessagedSoundPool");
            return;
        }
        if (players.Count <= 0)
        {
            Debug.LogError("Tried to play a 2D sound but there's no players");
            return;
        }
        //Get an unused player
        if (unusedPlayers.Count > 0)
        {
            var player = unusedPlayers.Dequeue();
            usedPlayers.Add(player);
            if (player != null)
            {
                player.volume = info.volume;

                var mixerGroups = Mixer.FindMatchingGroups(info.busName);
                if (mixerGroups != null && mixerGroups.Length > 0)
                {
                    player.outputAudioMixerGroup = mixerGroups[0];
                }
                else
                {
                    player.outputAudioMixerGroup = null;
                }
                player.clip = info.stream;
                player.spatializePostEffects = true;
                player.spatialBlend = 1.0f;
                player.pitch = info.pitch;
                player.transform.position = info.playPosition;
                player.Play();
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
    public void PlayPositionedSound3D((AudioClip stream, Vector3 playPosition, string busName, float volume, float pitch) info)
    {
        if (!use3DPlayers)
        {
            Debug.LogError("Tried to play a 3D sound but not using 3D sound pool, See MessagedSoundPool");
            return;
        }

        if (players.Count <= 0)
        {
            Debug.LogError("Tried to play a 3D sound but there's no players");
            return;
        }
        //Get an unused player
        if (unusedPlayers.Count > 0)
        {
            var player = unusedPlayers.Dequeue();
            usedPlayers.Add(player);
            if (player != null)
            {
                player.volume = info.volume;

                var mixerGroups = Mixer.FindMatchingGroups(info.busName);
                if (mixerGroups != null && mixerGroups.Length > 0)
                {
                    player.outputAudioMixerGroup = mixerGroups[0];
                }
                else
                {
                    player.outputAudioMixerGroup = null;
                }
                player.clip = info.stream;
                player.spatializePostEffects = true;
                player.spatialBlend = 1.0f;
                player.pitch = info.pitch;
                player.transform.position = info.playPosition;
                player.Play();
                return;
            }
        }
    }

}
