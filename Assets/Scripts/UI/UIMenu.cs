using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIMenu : MonoBehaviour
{
    public GameObject header, pauseMenu, winMenu, loseMenu, rewardAds;
    public Slider resSliderScale;
    public Toggle fps30, fps60, mute;
    public TextMeshProUGUI resSliderText, warningText;
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
        resSliderScale.onValueChanged.AddListener(SetScaleRes);
        fps30.onValueChanged.AddListener(OnValueChangedFps30);
        fps60.onValueChanged.AddListener(OnValueChangedFps60);
        mute.onValueChanged.AddListener(MuteGame);
    }

    private void Start() {
        resSliderScale.value = gameManager.settingData.resolutionScale * 10;
        float fps = gameManager.settingData.fps;
        fps30.isOn = fps == 30;
        fps60.isOn = fps == 60;
        mute.isOn = gameManager.settingData.mute;
        
        warningText.gameObject.SetActive(false);
    }

    private void OnDisable() {
        resSliderScale.onValueChanged.RemoveListener(SetScaleRes);
        mute.onValueChanged.RemoveListener(MuteGame);
        fps30.onValueChanged.RemoveListener(OnValueChangedFps30);
        fps60.onValueChanged.RemoveListener(OnValueChangedFps60);
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

    private void SetScaleRes(float value) {
        float scale = value/10;
        if(gameManager.settingData.resolutionScale != scale) {
            warningText.gameObject.SetActive(true);
            gameManager.settingData.resolutionScale = scale;
            gameManager.settingData.Save();
        }
        resSliderText.text = "Resolution scale:" + scale.ToString();
    }

    private void OnValueChangedFps30(bool check) {
        if(check) {
            if(gameManager.settingData.fps != 30) {
                warningText.gameObject.SetActive(true);
                gameManager.settingData.fps = 30;
                fps60.isOn = false;
                gameManager.settingData.Save();
            }
        }
    }

    private void OnValueChangedFps60(bool check) {
        if(check) {
            if(gameManager.settingData.fps != 60) {
                warningText.gameObject.SetActive(true);
                gameManager.settingData.fps = 60;
                fps30.isOn = false;
                gameManager.settingData.Save();
            }
        }
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
        Invoke("ShowEndMenu", 0.3f);
    }

    public void OnRewardAdsFailed() {
        rewardAdsFailed = true;
    }
    
    
    private void ShowEndMenu() {
        animator.SetTrigger(OpenMenuHash);
        Time.timeScale = 1;
    }
    
}
