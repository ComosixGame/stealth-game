using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;


public class NewSceneCreator : EditorWindow {

    private string nameLevel;

    [MenuItem("Comosix Tools/Create New level")]
    private static void ShowWindow() {
        var window = GetWindow<NewSceneCreator>();
        window.titleContent = new GUIContent("Create New level");
        window.Show();
    }

    private void OnGUI() {
        nameLevel = EditorGUILayout.TextField("Name Level", nameLevel);
        GUI.enabled = !string.IsNullOrWhiteSpace(nameLevel);
        if(GUILayout.Button("Create")) {
            CheckAndCreateScene();
        }
    }

    private void CheckAndCreateScene() {
        if (EditorApplication.isPlaying) {
            Debug.LogWarning ("Cannot create scenes while in play mode.  Exit play mode first.");
            return;
        }

        Scene currentActiveScene = SceneManager.GetActiveScene ();

        if (currentActiveScene.isDirty) {
            string title = currentActiveScene.name + " Has Been Modified";
            string message = "Do you want to save the changes you made to " + currentActiveScene.path + "?\nChanges will be lost if you don't save them.";
            int option = EditorUtility.DisplayDialogComplex (title, message, "Save", "Don't Save", "Cancel");

            if (option == 0) {
                EditorSceneManager.SaveScene (currentActiveScene);
            }
            else if (option == 2) {
                return;
            }
        }

        bool assetExists = AssetDatabase.GetMainAssetTypeAtPath($"Assets/Scenes/Levels/{nameLevel}.unity") != null;
        if(assetExists) {
            EditorUtility.DisplayDialog("Error",
                    "Name of level already exist!",
                    "OK");
            return;
        }

        CreateScene();
    }

    private void CreateScene() {
        string[] result = AssetDatabase.FindAssets("_TemplateScene");
        if(result.Length > 0) {
            string templateScenePath = AssetDatabase.GUIDToAssetPath(result[0]) + ".unity";
            string newPath = $"Assets/Scenes/Levels/{nameLevel}.unity";
            if (!AssetDatabase.CopyAsset(templateScenePath, newPath)) {
                Debug.LogError("Couldn't copy the scene to the new location'");
                return;
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            Scene newScene = EditorSceneManager.OpenScene(newPath, OpenSceneMode.Single);
            AddSceneToBuildSettings(newScene);
            Close();
        } else {
            EditorUtility.DisplayDialog("Error",
                    "The scene _TemplateScene was not found in Gamekit3D/Scenes folder. This scene is required by the New Scene Creator.",
                    "OK");
        }
    }

    private void AddSceneToBuildSettings (Scene scene) {
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;
    
        EditorBuildSettingsScene[] newBuildScenes = new EditorBuildSettingsScene[buildScenes.Length + 1];
        for (int i = 0; i < buildScenes.Length; i++)
        {
            newBuildScenes[i] = buildScenes[i];
        }
        newBuildScenes[buildScenes.Length] = new EditorBuildSettingsScene(scene.path, true);
        EditorBuildSettings.scenes = newBuildScenes;
    }
}
