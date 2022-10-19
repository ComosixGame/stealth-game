using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

public class LoadScene : MonoBehaviour
{
    public GameObject LoadingScreen;
    public Slider loadingBar;
    public bool StartScene;
    [HideInInspector] public int firstLevel, nextLevel;
    private AsyncOperation operation;
    private int levelIndex;
    private GameManager gameManager;

    private void Awake() {
        levelIndex = SceneManager.GetActiveScene().buildIndex;
        gameManager = GameManager.Instance;
    }

    private void OnEnable() {
        gameManager.OnEndGame.AddListener(OnEndGame);
    }

    private void Start() {
        if(StartScene) {
            LoadNewScene(firstLevel);
        }
    }

    public void ResetLevel() {
        LoadNewScene(levelIndex);
    }

    public void LoadNextLevel() {
        LoadNewScene(nextLevel);
    }
    
    public void LoadNewScene(int index) {
        StartCoroutine(LoadAsync(index));
        operation.completed += InitGame;
    }

    IEnumerator LoadAsync(int index) {
        operation = SceneManager.LoadSceneAsync(index);
        LoadingScreen.SetActive(true);
        while(!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress/ 0.9f);
            loadingBar.value = progress;
            yield return null;
        }
    }

    private void InitGame(AsyncOperation asyncOperation) {
        gameManager.InitGame();
    }

    private void OnEndGame(bool isWin, int moneyInLevel) {
        if(isWin) {
            gameManager.UnlockNewLevel(nextLevel);
        }
    }

    private void OnDisable() {
        gameManager.OnEndGame.RemoveListener(OnEndGame);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LoadScene))]
    public class EditorLoadScene : Editor {
        string[] scenes;
        private void OnEnable() {
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            scenes = new string[sceneCount];

            for( int i = 0; i < sceneCount; i++ )
            {
                scenes[i] = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            }
        }
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            LoadScene loadScene = target as LoadScene;

            string Label = loadScene.StartScene?"first Level":"Next Level";
            int levelIndex = loadScene.StartScene?loadScene.firstLevel:loadScene.nextLevel;
 
            EditorGUI.BeginChangeCheck();
            int indexScene = EditorGUILayout.Popup(Label ,levelIndex, scenes);
            if(EditorGUI.EndChangeCheck()) {
                if(loadScene.StartScene) {
                    Undo.RecordObject(loadScene, "Update first Level");
                    loadScene.firstLevel = indexScene;
                } else {
                    Undo.RecordObject(loadScene, "Update Next Level");
                    loadScene.nextLevel = indexScene;
                }
            }
        }
    }
#endif

}
