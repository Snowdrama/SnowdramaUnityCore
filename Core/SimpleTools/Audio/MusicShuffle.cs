using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snow.SimpleTools
{
    public class MusicShuffle : MonoBehaviour
    {
        public List<AudioSource> allSongs;
        public AudioSource currentSource;

        private void Start()
        {
            if(currentSource == null)
            {
                currentSource = GetComponent<AudioSource>();

                if (currentSource == null)
                {
                    Debug.LogError("MusicShuffle doesn't have audio source set", this.gameObject);
                }
            }
        }
        void Update()
        {
            //go to new song if finished
            if (currentSource == null || !currentSource.isPlaying || currentSource.time >= currentSource.clip.length)
            {
                currentSource = allSongs[Random.Range(0, allSongs.Count)];
                currentSource.Play();
            }
        }
    }
}