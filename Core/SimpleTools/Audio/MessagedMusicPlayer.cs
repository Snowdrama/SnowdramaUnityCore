using Snowdrama.Core.GameData;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Plays a song by it's name and a transition speed 
/// </summary>
public class PlayMusicRequestMessage : AMessage<string, float> { }
public class MessagedMusicPlayer : MonoBehaviour
{
    public UnityDictionary<string, AudioClip> songs;
    PlayMusicRequestMessage requestMessage;

    [SerializeField] AudioSource currentSource;
    [SerializeField] AudioSource nextSource;

    void Start()
    {
        if (currentSource.clip == null)
        {
            currentSource.clip = songs.GetRandom().Value;
        }
    }
   

    [Header("Target Songs")]
    [SerializeField, EditorReadOnly] string nextSong = "";
    [SerializeField, EditorReadOnly] string currentSong = "";

    [Header("Transition Info")]
    [SerializeField, EditorReadOnly] bool transitioning;
    [SerializeField, EditorReadOnly] float transitionTime;
    [SerializeField, EditorReadOnly] float transitionSpeed = 1.0f;

    [Header("Debug")]
    [SerializeField, EditorReadOnly] bool debugChangeSong;

    void Update()
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

            if(transitionTime >= 1.0f)
            {
                transitioning = false;
                transitionTime = 0;
                SwapSources();
            }
        }
    }

    void SwapSources()
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
