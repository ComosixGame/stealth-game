using UnityEngine;
using Cinemachine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] private EquipmentManager equipmentManager;
    public RectTransform joyStick;
    public  CinemachineFreeLook cinemachineFreeLook;
    public BossBehaviourScript boss;
    public bool debugMode;
    private GameObject player;
    private PlayerAttack playerAttack;
    private Transform playerTransform;
    [HideInInspector] public GameObject debugCharacter;
    [HideInInspector] public Mesh debugMesh;

    // Start is called before the first frame update
    void Start()
    {
        PlayerData playerData = PlayerData.Load();
        int selectedCharacter = playerData.selectedCharacter;
        if(debugMode) {
            player = Instantiate(debugCharacter, transform.position, transform.rotation);
            playerTransform = player.transform;
            playerTransform.SetParent(transform);
        } else {
            GameObject character = equipmentManager.Characters[selectedCharacter].character;
            player = Instantiate(character, transform.position, transform.rotation);
            playerTransform = player.transform;
            playerAttack = player.GetComponent<PlayerAttack>();
            int selectedWeapon = playerData.selectedWeapon;
            GameObject weapon = equipmentManager.Weapons[selectedWeapon].weapon;
            playerAttack.weapon = weapon;
            playerTransform.SetParent(transform);
        }
        if(boss != null) {
            boss.Player = playerTransform;
        }
        cinemachineFreeLook.LookAt = playerTransform;
        cinemachineFreeLook.Follow = playerTransform;

        player.GetComponent<PlayerController>().joystickRectTrans = joyStick;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

#if UNITY_EDITOR
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
