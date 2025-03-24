using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiChannelAudio : MonoBehaviour
{
    public AudioClip[] clips;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void PlayRandomSound()
    {
        // Pick a random clip
        AudioClip clip = clips[Random.Range(0, clips.Length)];

        // Create a new temporary GameObject to hold the AudioSource
        GameObject tempAudioObj = new GameObject("TempAudio");
        tempAudioObj.transform.position = transform.position;

        // Add AudioSource component
        AudioSource tempSource = tempAudioObj.AddComponent<AudioSource>();
        tempSource.clip = clip;
        //tempSource.volume = volume;
        tempSource.Play();

        // Destroy the GameObject after the clip finishes playing
        Destroy(tempAudioObj, clip.length);
    }
}
