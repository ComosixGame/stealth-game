using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertSound : MonoBehaviour
{
    private GameManager gameManager;
    private SoundManager soundManager;
    private AudioSource audioSource;

    private void Awake() {
        gameManager = GameManager.Instance;
        soundManager = SoundManager.Instance;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable() {
        gameManager.OnEnemyAlert.AddListener(OnAlert);
        gameManager.OnEnemyAlertOff.AddListener(OnAlertOff);
        soundManager.OnMute.AddListener(OnMuteGame);
    }

    private void OnAlert(Vector3 pos) {
        audioSource.Play();
    }

    private void OnAlertOff() {
        audioSource.Stop();
    }

    private void OnMuteGame(bool mute) {
        audioSource.mute = mute;
    }

    private void OnDisable() {
        gameManager.OnEnemyAlert.RemoveListener(OnAlert);
        gameManager.OnEnemyAlertOff.RemoveListener(OnAlertOff);
        soundManager.OnMute.RemoveListener(OnMuteGame);
    }
}
