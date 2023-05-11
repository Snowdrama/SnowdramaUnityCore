using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snow.SimpleTools
{
    public class MusicShuffle : MonoBehaviour
    {
        public List<AudioClip> songs;
        AudioSource source;
        // Start is called before the first frame update
        void Start()
        {
            source = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!source.isPlaying)
            {
                Debug.Log("Starting Song");
                //the song ended, so we can start the next one
                source.clip = songs[Random.Range(0, songs.Count)];
                source.Play();
            }
        }
    }
}