using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : Singleton<SoundManager>
{
    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayOneShot(AudioClip audioClip, float volumeScale = 1) {
        audioSource.PlayOneShot(audioClip, volumeScale);
    }

    public AudioSource AddAudioSource(GameObject parent) {
        return parent.AddComponent<AudioSource>();
    }
}
