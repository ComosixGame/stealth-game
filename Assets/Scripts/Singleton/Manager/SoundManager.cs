using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : Singleton<SoundManager>
{
    private AudioSource audioSource;
    private SettingData settingData;
    public UnityEvent<bool> OnMute = new UnityEvent<bool>();

    protected override void Awake()
    {
        base.Awake();
        settingData = SettingData.Load();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        audioSource.mute = settingData.mute;
    }


    public void PlayOneShot(AudioClip audioClip, float volumeScale = 1) {
        audioSource.PlayOneShot(audioClip, volumeScale);
    }

    public AudioSource AddAudioSource(GameObject parent) {
        AudioSource audioSource = parent.AddComponent<AudioSource>();
        audioSource.mute = settingData.mute;
        return audioSource;
    }

    public void MuteGame(bool mute) {
        settingData.mute = mute;
        settingData.Save();
        audioSource.mute = mute;
        OnMute?.Invoke(mute);
    }
}
