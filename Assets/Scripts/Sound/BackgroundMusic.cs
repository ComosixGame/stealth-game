using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private SoundManager soundManager;
    private AudioSource audioSource;

    private void Awake() {
        soundManager = SoundManager.Instance;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable() {
        soundManager.OnMute.AddListener(OnMuteGame);
    }

    private void OnMuteGame(bool mute) {
        audioSource.mute = mute;
    }
}
