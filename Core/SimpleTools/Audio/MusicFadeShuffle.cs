using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicFadeShuffle : MonoBehaviour
{
    public List<AudioClip> songs;

    public AudioSource currentSource;
    public AudioSource nextSource;

    public float percentRemainingToFade = 0.1f;

    [Header("Debug")]
    public float timePercentRemaining;

    void Start()
    {
        if (currentSource == null)
        {
            currentSource = GetComponent<AudioSource>();

            if (currentSource == null)
            {
                Debug.LogError("MusicShuffle doesn't have audio source set", this.gameObject);
            }
        }
        currentSource.clip = songs[Random.Range(0, songs.Count)];
        currentSource.Play();
    }

    void Update()
    {
        timePercentRemaining = 1f - currentSource.time / currentSource.clip.length;
        if (timePercentRemaining <= percentRemainingToFade)
        {
            //start transitioning
            if (!nextSource.isPlaying)
            {
                //set the next clip
                nextSource.clip = songs[Random.Range(0, songs.Count)];
                nextSource.Play();
            }

            //fade between the sources until we get to the end
            var currentSourceVolume = Mathf.Lerp(0.0f, 1.0f, Mathf.InverseLerp(0.0f, 0.1f, timePercentRemaining));
            var nextSourceVolume = Mathf.Lerp(0.0f, 1.0f, Mathf.InverseLerp(0.1f, 0.0f, timePercentRemaining));
            currentSource.volume = currentSourceVolume;
            nextSource.volume = nextSourceVolume;
        }

        if (!currentSource.isPlaying)
        {
            //swap sources
            var temp = currentSource;
            currentSource = nextSource;
            nextSource = temp;
        }
    }
}
