using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : Singleton<SoundManager>
{
    private AudioSource audioSource;
    private bool isMute;
    public UnityEvent<bool> OnMute;

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

    public void MuteGame(bool mute) {
        isMute = mute;
        audioSource.mute = mute;

        OnMute?.Invoke(isMute);
    }
}
