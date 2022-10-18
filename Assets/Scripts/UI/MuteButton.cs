using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MuteButton : MonoBehaviour
{
    public bool isMute;
    
    private SettingData settingData;
    private Button button;
    private SoundManager soundManager;

    private void Awake() {
        soundManager = SoundManager.Instance;
        button = GetComponent<Button>();
    }

    private void OnEnable() {
        soundManager.OnMute.AddListener(OnMute);

        button.onClick.AddListener(OnClick);
    }
    
    void Start()
    {
        settingData = SettingData.Load();
        OnMute(settingData.mute);
    }

    private void OnMute(bool mute) {
        if(mute) {
            gameObject.SetActive(!isMute);
        } else {
            gameObject.SetActive(isMute);
        }
    }

    private void OnClick() {
        soundManager.MuteGame(isMute);
    }

    private void OnDestroy() {
        soundManager.OnMute.RemoveListener(OnMute);
    }
}
