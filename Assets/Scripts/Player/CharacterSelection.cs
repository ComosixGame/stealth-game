using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] private CharacterManager characterManager;
    public RectTransform joyStick;
    public  CinemachineFreeLook cinemachineFreeLook;
    private GameObject player;
    public bool debugMode;

    // Start is called before the first frame update
    void Start()
    {
        if(debugMode) {
            player = Instantiate(debugCharacter, transform.position, transform.rotation);
            player.transform.SetParent(transform);
        } else {
            int indexChar = PlayerData.Load().selectedCharacter;
            GameObject character = characterManager.Characters[indexChar].character;
            player = Instantiate(character, transform.position, transform.rotation);
            player.transform.SetParent(transform);
        }

        cinemachineFreeLook.LookAt = player.transform;
        cinemachineFreeLook.Follow = player.transform;

        player.GetComponent<PlayerController>().joystickRectTrans = joyStick;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

#if UNITY_EDITOR
    [HideInInspector] public GameObject debugCharacter;
    [HideInInspector] public Mesh debugMesh;
    private void OnDrawGizmos() {
        if(!EditorApplication.isPlaying) {
            Gizmos.DrawMesh(debugMesh, transform.position, transform.rotation);
        }
    }
    
    
    [CustomEditor(typeof(CharacterSelection))]
    public class EditorCharacterSelection : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            CharacterSelection selection = target as CharacterSelection;
            if(selection.debugMode) {
                EditorGUI.BeginChangeCheck();
                GameObject character = EditorGUILayout.ObjectField("Debug Character", selection.debugCharacter, typeof(GameObject), true) as GameObject;
                if(EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(selection, "Update debugCharacter");
                    selection.debugCharacter = character;
                }

                EditorGUI.BeginChangeCheck();
                Mesh mesh = EditorGUILayout.ObjectField("Debug Mesh", selection.debugMesh, typeof(Mesh), true) as Mesh;
                if(EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(selection, "Update debugMesh");
                    selection.debugMesh = mesh;
                }
            }
        }
    }
#endif
}
