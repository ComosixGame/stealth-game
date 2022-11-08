using System.Collections;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public AudioClip audioWin, audioLose;
    private SoundManager soundManager;
    private GameManager gameManager;
    private AudioSource audioSource;

    private void Awake() {
        soundManager = SoundManager.Instance;
        gameManager = GameManager.Instance;
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        audioSource.mute =  gameManager.settingData.mute;
    }

    private void OnEnable() {
        soundManager.OnMute.AddListener(OnMuteGame);
        gameManager.OnEndGame.AddListener(OnEndGame);
    }

    private void OnMuteGame(bool mute) {
        audioSource.mute = mute;
    }

    private void OnEndGame(bool isWin) {
        if(isWin) {
            audioSource.clip = audioWin;
        } else {
            audioSource.clip = audioLose;
        }
        StartCoroutine(PlayAudioEndGame());
    }

    private void OnDisable() {
        soundManager.OnMute.RemoveListener(OnMuteGame);
        gameManager.OnEndGame.RemoveListener(OnEndGame);
    }

    IEnumerator PlayAudioEndGame() {
        yield return new WaitForSeconds(0.3f);
        audioSource.Play();
    }
}
