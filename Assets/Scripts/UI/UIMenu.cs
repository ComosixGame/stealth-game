using System.Collections;
using UnityEngine;
using Unity.Services.Mediation;
public class UIMenu : MonoBehaviour
{
    public GameObject header, pauseMenu, winMenu, loseMenu, rewardAds;
    public InterstitialAds interstitialAds;
    public float volumeScale;
    private GameManager gameManager;
    private Animator animator;
    private int OpenMenuHash, CloseMenuHash;
    private bool rewardAdsFailed, isWin;
    private SoundManager soundManager;

    private void Awake() {
        gameManager = GameManager.Instance;
        soundManager = SoundManager.Instance;
        animator = GetComponent<Animator>();

        OpenMenuHash = Animator.StringToHash("OpenMenu");
        CloseMenuHash = Animator.StringToHash("CloseMenu");
    }

    private void OnEnable() {
        gameManager.OnEndGame.AddListener(OnEndGame);
    }

    public void StartGame() {
        gameManager.StartGame();
    }

    public void PauseGame() {
        pauseMenu.SetActive(true);
        animator.SetTrigger(OpenMenuHash);
        gameManager.PauseGame();
    }


    public void ResumeGame() {
        animator.SetTrigger(CloseMenuHash);
    }

    private void ResumeGameAnimatiorEvent() {
        pauseMenu.SetActive(false);
        gameManager.ResumeGame();
    }

    public void PlaySound(AudioClip audioClip) {
        soundManager.PlayOneShot(audioClip, volumeScale);
    }
    public void MuteGame(bool mute) {
        soundManager.MuteGame(mute);
    }

    public void QuitGame() {
        Application.Quit();
    }

    private void OnEndGame(bool win) {
        header.SetActive(false);
        rewardAds.SetActive(!rewardAdsFailed);
        isWin = win;
        if(win) {
            winMenu.SetActive(true);
        } else {
            loseMenu.SetActive(true);
        }
        Time.timeScale = 0.3f;
        StartCoroutine(ShowEndMenu());
    }

    private void OnRewardAdsFailed(string message) {
        rewardAdsFailed = true;
    }
    
    IEnumerator ShowEndMenu() {
        yield return new WaitForSeconds(0.3f);
        animator.SetTrigger(OpenMenuHash);
        Time.timeScale = 1;
        if(!isWin) {
            interstitialAds.ShowInterstitial();
        }
    }
    
}
