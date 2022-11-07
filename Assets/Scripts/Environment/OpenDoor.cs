using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Collider))]
public class OpenDoor : Command
{
    public enum TypeOpen {
        Rotate,
        Translate
    }
    public enum Axis {
        X,
        Y,
        Z
    }
    public Transform door;
    public TypeOpen typeOpen;
    public GameObject locks;
    public LayerMask layer, layerOnAlert;
    public AudioClip audioClip;
    [Range(0,1)] public float volumeScale;
    [HideInInspector] public Axis axis;
    [HideInInspector] public float angel;
    [HideInInspector] public Vector3 PosMove;
    private bool haveKey, unLocked;
    private Vector3 orginPos;
    private Vector3 axisVector;
    private SoundManager soundManager;

    private void Awake() {
        soundManager = SoundManager.Instance;
    }

    private void Start() {
        orginPos = door.position;
    }
    private void Update() {
        if(unLocked) {
            Open();
        }

    }

    private void OnTriggerEnter(Collider other) {
        if((layer & (1<<other.gameObject.layer)) != 0) {
            if(haveKey && !unLocked) {
                soundManager.PlayOneShot(audioClip, volumeScale);
                unLocked = true;
                door.GetComponent<Collider>().isTrigger = true;
            }
        }
    }

    public override void Execute()
    {
        haveKey = true;
        locks.SetActive(false);
    }

    public override void Undo()
    {
        throw new System.NotImplementedException();
    }

    private void Open() {
        GetAxisVector();
        if(typeOpen == TypeOpen.Rotate) {
            Quaternion rot = door.rotation;
            rot = Quaternion.AngleAxis(angel, axisVector);
            door.rotation = Quaternion.Lerp(door.rotation, rot, 5f * Time.deltaTime);
        } else {
            door.position = Vector3.MoveTowards(door.position, PosMove, 5f * Time.deltaTime);
        }
    }

    private void GetAxisVector() {
        switch(axis) {
            case Axis.X:
                axisVector = Vector3.right;
                break;
            case Axis.Y:
                axisVector = Vector3.up;
                break;
            case Axis.Z:
                axisVector = Vector3.forward;
                break;
            default:
                break;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        GetAxisVector();
        Mesh Doormesh = door.GetComponent<MeshFilter>().sharedMesh;
        Gizmos.color = Color.blue;
        if(typeOpen == TypeOpen.Rotate) {
            Quaternion rot = Quaternion.AngleAxis(angel, axisVector);
            Gizmos.DrawMesh(Doormesh, door.position, rot);
        }

        Handles.Label(transform.position, "Interact Object","TextField");
    }
    
    [CustomEditor(typeof(OpenDoor))]
    public class editorOpenDoor : Editor {

        private void OnSceneGUI() {
            OpenDoor t = target as OpenDoor;
            if(t.typeOpen == TypeOpen.Translate) {
                Handles.color = Color.blue;
                Handles.Label(t.door.position,"Start", "Button");
                Handles.Label(t.PosMove,"End", "Button");
                Handles.DrawDottedLine(t.door.position, t.PosMove, 3f);
                EditorGUI.BeginChangeCheck();
                Vector3 newPos = Handles.PositionHandle(t.PosMove, t.transform.rotation);
                if(EditorGUI.EndChangeCheck()) {
                    UnityEditor.Undo.RecordObject(t, "update Position Move");
                    t.PosMove = newPos;
                    EditorUtility.SetDirty(t);
                }
            }
        }
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            OpenDoor t = target as OpenDoor;
            if(t.typeOpen == TypeOpen.Rotate) {
                string[] options = Enum.GetNames(typeof(Axis));

                EditorGUI.BeginChangeCheck();
                int axisIndex = EditorGUILayout.Popup("Axis", (int)t.axis, options);
                if(EditorGUI.EndChangeCheck()) {
                    UnityEditor.Undo.RecordObject(t, "update axis");
                    t.axis = (Axis)axisIndex;
                    EditorUtility.SetDirty(t);
                }

                EditorGUI.BeginChangeCheck();
                float angel = EditorGUILayout.FloatField("Angle", t.angel);
                if(EditorGUI.EndChangeCheck()) {
                    UnityEditor.Undo.RecordObject(t, "update angel");
                    t.angel = angel;
                    EditorUtility.SetDirty(t);
                }
            } else {
                EditorGUI.BeginChangeCheck();
                Vector3 pos = EditorGUILayout.Vector3Field("Move To Position", t.PosMove);
                if(EditorGUI.EndChangeCheck()) {
                    UnityEditor.Undo.RecordObject(t, "update Position Move");
                    t.PosMove = pos;
                    EditorUtility.SetDirty(t);
                }
                EditorGUI.BeginChangeCheck();
                if(GUILayout.Button("Reset Position Move")) {
                    if(EditorGUI.EndChangeCheck()) {
                        UnityEditor.Undo.RecordObject(t, "update Position Move");
                        t.PosMove = t.door.position;
                        EditorUtility.SetDirty(t);
                    }
                }
            }

        }

    }
#endif
}
