using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    private GameManager gameManager;
    private Animator animator;
    private int OpenMenuHash;
    private int CloseMenuHash;

    private void Awake() {
        gameManager = GameManager.Instance;
        animator = GetComponent<Animator>();

        OpenMenuHash = Animator.StringToHash("OpenMenu");
        CloseMenuHash = Animator.StringToHash("CloseMenu");
    }

    private void PauseGame() {
        animator.SetTrigger(OpenMenuHash);
        pauseMenu.SetActive(true);
        gameManager.PauseGame();
    }


    private void ResumeGame() {
        animator.SetTrigger(CloseMenuHash);
    }

    private void ResumeGameAnimatiorEvent() {
        pauseMenu.SetActive(false);
        gameManager.ResumeGame();
    }
}
