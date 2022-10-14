using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public float volumeScale;
    private GameManager gameManager;
    private Animator animator;
    private int OpenMenuHash;
    private int CloseMenuHash;
    private SoundManager soundManager;

    private void Awake() {
        gameManager = GameManager.Instance;
        soundManager = SoundManager.Instance;
        animator = GetComponent<Animator>();

        OpenMenuHash = Animator.StringToHash("OpenMenu");
        CloseMenuHash = Animator.StringToHash("CloseMenu");
    }

    public void PauseGame() {
        animator.SetTrigger(OpenMenuHash);
        pauseMenu.SetActive(true);
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
}
