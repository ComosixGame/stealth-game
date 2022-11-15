using UnityEngine;

public class AlertSound : MonoBehaviour
{
    private SettingData settingData;
    private GameManager gameManager;
    private SoundManager soundManager;
    private AudioSource audioSource;

    private void Awake() {
        gameManager = GameManager.Instance;
        soundManager = SoundManager.Instance;
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        audioSource.mute =  gameManager.settingData.mute;
    }

    private void OnEnable() {
        gameManager.OnEnemyAlert.AddListener(OnAlert);
        gameManager.OnEnemyAlertOff.AddListener(OnAlertOff);
        gameManager.OnEndGame.AddListener(OnEndGame);
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

    private void OnEndGame(bool win) {
        audioSource.Stop();
    }

    private void OnDisable() {
        gameManager.OnEnemyAlert.RemoveListener(OnAlert);
        gameManager.OnEnemyAlertOff.RemoveListener(OnAlertOff);
        gameManager.OnEndGame.RemoveListener(OnEndGame);
        soundManager.OnMute.RemoveListener(OnMuteGame);
    }
}
