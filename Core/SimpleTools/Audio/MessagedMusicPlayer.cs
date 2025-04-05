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
    AudioSource currentSource;
    AudioSource nextSource;

    void Start()
    {
        
    }
    float timePercentRemaining;
    bool transitioning;
    float transitionTime;
    float transitionSpeed = 1.0f;
    void Update()
    {
        if (nextSong != currentSong)
        {
            //set the next clip
            nextSource.clip = songs[nextSong];
            nextSource.Play();
            transitioning = true;
            transitionTime = 0;
            nextSong = currentSong;
        }

        if (transitioning)
        {
            transitionTime += Time.deltaTime * transitionSpeed;
            float lerp = Mathf.Lerp(0, 1, transitionTime);
            float invLerp = Mathf.Lerp(1, 0, transitionTime);

            //fade between the sources until we get to the end
            var currentSourceVolume = Mathf.Lerp(0.0f, 1.0f, Mathf.InverseLerp(0.0f, 0.1f, timePercentRemaining));
            var nextSourceVolume = Mathf.Lerp(0.0f, 1.0f, Mathf.InverseLerp(0.1f, 0.0f, timePercentRemaining));
            currentSource.volume = currentSourceVolume;
            nextSource.volume = nextSourceVolume;

            if(lerp >= 1.0f)
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

    string currentSong = "";
    string nextSong = "";

    private void SetNextSong(string setNextSong, float setTransitionSpeed)
    {
        transitionSpeed = setTransitionSpeed;
        nextSong = setNextSong;
    }
}
