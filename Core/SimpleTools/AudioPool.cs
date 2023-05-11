using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snow.SimpleTools
{
    public class AudioPool : MonoBehaviour
    {
        public List<AudioClip> sounds;
        public List<AudioSource> spawnedSources;
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void PlaySound()
        {
            if (sounds.Count <= 0)
                return;

            bool foundSource = false;
            foreach (var source in spawnedSources)
            {
                if (!source.isPlaying)
                {
                    source.clip = sounds[Random.Range(0, sounds.Count)];
                    source.Play();
                    foundSource = true;
                }
            }
            if (!foundSource)
            {
                var go = Instantiate(new GameObject("AudioPoolClip"));
                var source = go.AddComponent<AudioSource>();
                source.clip = sounds[Random.Range(0, sounds.Count)];
                source.loop = false;
                source.playOnAwake = false;
                source.Play();
            }
        }
    }
}