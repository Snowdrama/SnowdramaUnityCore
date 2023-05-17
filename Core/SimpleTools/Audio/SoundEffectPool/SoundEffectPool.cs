using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectPool : MonoBehaviour
{
    [SerializeField] private Vector2 defaultVolume = new Vector2(1, 1);
    [SerializeField] private string poolName;
    [SerializeField] private List<AudioSource> sources;

    [Header("Expandable")]
    [SerializeField] private bool createNewSources;
    [SerializeField] private AudioSource sourcePrefab;
    private void OnEnable()
    {
        SoundEffectPoolManager.AddPool(poolName, this);
    }

    private void OnDisable()
    {
        SoundEffectPoolManager.RemovePool(poolName);
    }

    public void PlaySource(AudioClip clip, Vector2? pitchMinMax = null, float spatialBlend = 0, Vector3? position = null)
    {
        //try to find a source
        bool found = false; 
        for (int i = 0; i < sources.Count; i++)
        {
            var foundSource = sources[i];
            if (foundSource.clip == null)
            {
                //we assume that if it's null then it's never played a sound!
                found = true;
            }
            else if (foundSource.isPlaying == false)
            {
                //if it's not playing then it's available to play a sound
                found = true;
            }
            else if(foundSource.clip != null && foundSource.time >= foundSource.clip.length)
            {
                //This is basically a WebGL bug fix
                //In WebGL isPlaying is true if it's played at all for... reasons?
                //so we check if the clip has played to the end of the clip
                found = true;
            }

            //we found a source! 
            if (found) {

                PlaySource(foundSource, clip, pitchMinMax, spatialBlend, position);
                break;
            }
        }

        //if we didn't find one
        if (!found)
        {
            Debug.LogWarning($"No sources found that could play clip");

            //see if we want to create new sources
            if (createNewSources)
            {
                Debug.LogWarning($"Creating new source");
                var newSource = CreateSource();
                PlaySource(newSource, clip, pitchMinMax, spatialBlend, position);
            }
        }
}
    public void PlaySource(AudioSource source, AudioClip clip, Vector2? pitchMinMax = null, float spatialBlend = 0, Vector3? position = null)
    {
        //set the clip
        source.clip = clip;
        //we can pitch the value from min to max
        //min is -3 and max is 3
        //though I think there's a bug with this in WebGL
        if (pitchMinMax.HasValue)
        {
            //change the pitch
            source.pitch = Random.Range(0, pitchMinMax.Value.y);
        }
        else
        {
            //otherwise pitch is 1
            source.pitch = 1;
        }

        //set the posiiton if we have one
        if (position.HasValue)
        {
            source.transform.position = position.Value;
        }
        else
        {
            source.transform.position = this.transform.position;
        }

        //2D by Default
        source.spatialBlend = spatialBlend;
        source.Play();
    }

    public AudioSource CreateSource()
    {
        //create a new source
        var go = Instantiate(sourcePrefab.gameObject, this.transform);
        go.transform.position = this.transform.position;
        go.transform.rotation = Quaternion.identity;

        //get the source
        var source = go.GetComponent<AudioSource>();
        sources.Add(source);
        return source;
    }
}
