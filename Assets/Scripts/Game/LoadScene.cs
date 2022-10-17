using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LoadScene : MonoBehaviour
{
    public GameObject LoadingScreen;
    public Slider loadingBar;
    [HideInInspector] public int NextLevel;
    private AsyncOperation operation;
    private GameManager gameManager;

    private void Awake() {
        gameManager = GameManager.Instance;
    }


    public void ResetLevel() {
        StartCoroutine(LoadAsync(SceneManager.GetActiveScene().buildIndex));
        operation.completed += ResetGame;
    }

    public void LoadNextLevel() {
        StartCoroutine(LoadAsync(NextLevel));
        operation.completed += ResetGame;
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

    private void ResetGame(AsyncOperation asyncOperation) {
        gameManager.ResetGame();
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
                scenes[i] = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            }
        }
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            LoadScene loadScene = target as LoadScene;
            EditorGUI.BeginChangeCheck();
            int indexScene = EditorGUILayout.Popup("Next Level",loadScene.NextLevel,scenes);
            if(EditorGUI.EndChangeCheck()) {
                loadScene.NextLevel = indexScene;
            }
        }
    }
#endif

}
