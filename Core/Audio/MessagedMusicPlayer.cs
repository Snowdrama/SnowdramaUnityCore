using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Plays a song by it's name and a transition speed 
/// </summary>
public class PlayMusicRequestMessage : AMessage<string, float> { }
public class MessagedMusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string musicMixerGroupName = "Music";
    public UnitySerializedDictionary<string, AudioClip> songs = new UnitySerializedDictionary<string, AudioClip>();
    private PlayMusicRequestMessage requestMessage;

    [SerializeField] private AudioSource currentSource;
    [SerializeField] private AudioSource nextSource;

    private void Start()
    {
        if (currentSource.clip == null)
        {
            currentSource.clip = songs.GetRandom().Value;
        }
        if (nextSource == null)
        {
            GameObject newNextSource = new GameObject();
            newNextSource.transform.SetParent(this.transform, false);
            nextSource = newNextSource.AddComponent<AudioSource>();
        }
        if (currentSource == null)
        {
            GameObject newCurrentSource = new GameObject();
            newCurrentSource.transform.SetParent(this.transform, false);
            currentSource = newCurrentSource.AddComponent<AudioSource>();
        }
        //music isn't directional
        nextSource.spatialBlend = 0.0f;
        currentSource.spatialBlend = 0.0f;
        if (mixer != null)
        {
            var groups = mixer.FindMatchingGroups(musicMixerGroupName);
            foreach (var g in groups)
            {
                Debug.Log($"Group: {g}");
            }
            if (groups != null && groups.Length > 0)
            {
                //assume we only found one group...
                nextSource.outputAudioMixerGroup = groups[0];
                currentSource.outputAudioMixerGroup = groups[0];
            }
        }
    }

    [Header("Target Songs")]
    [SerializeField, EditorReadOnly] private string nextSong = "";
    [SerializeField, EditorReadOnly] private string currentSong = "";

    [Header("Transition Info")]
    [SerializeField, EditorReadOnly] private bool transitioning;
    [SerializeField, EditorReadOnly] private float transitionTime;
    [SerializeField, EditorReadOnly] private float transitionSpeed = 1.0f;

    [Header("Debug")]
    [SerializeField, EditorReadOnly] private bool debugChangeSong;

    private void Update()
    {
        if (debugChangeSong)
        {
            Messages.Get<PlayMusicRequestMessage>().Dispatch(songs.GetRandom().Key, 0.25f);
            debugChangeSong = false;
        }

        if (nextSong != currentSong)
        {
            //set the next clip
            nextSource.clip = songs[nextSong];
            nextSource.Play();
            transitioning = true;
            transitionTime = 0;
            currentSong = nextSong;
        }

        //if it's not playing and the clip isn't null, start playing
        if (!currentSource.isPlaying && currentSource.clip != null)
        {
            currentSource.Play();
        }

        if (transitioning)
        {
            transitionTime += Time.deltaTime * transitionSpeed;

            //fade between the sources until we get to the end
            var nextSourceVolume = Mathf.Lerp(0.0f, 1.0f, transitionTime);
            var currentSourceVolume = Mathf.Lerp(1.0f, 0.0f, transitionTime);

            currentSource.volume = currentSourceVolume;
            nextSource.volume = nextSourceVolume;

            if (transitionTime >= 1.0f)
            {
                transitioning = false;
                transitionTime = 0;
                SwapSources();
            }
        }
    }

    private void SwapSources()
    {
        //swap sources
        var temp = currentSource;
        currentSource = nextSource;
        nextSource = temp;
    }


    private void OnEnable()
    {
        requestMessage = Messages.Get<PlayMusicRequestMessage>();
        requestMessage.AddListener(SetNextSong);
    }

    private void OnDisable()
    {
        requestMessage.RemoveListener(SetNextSong);
        requestMessage = null;
    }


    private void SetNextSong(string setNextSong, float setTransitionSpeed)
    {
        if (currentSong != setNextSong)
        {
            transitionSpeed = setTransitionSpeed;
            nextSong = setNextSong;
        }
    }
}
