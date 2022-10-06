using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
    [HideInInspector] public Axis axis;
    [HideInInspector] public float angel;
    [HideInInspector] public Vector3 PosMove;
    private bool beginExecute, unLocked;
    private Vector3 orginPos;
    private Vector3 axisVector;

    private void Start() {
        orginPos = door.position;
    }
    private void Update() {
        if(beginExecute && unLocked) {
            Open();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            unLocked = true;
        }
    }

    public override void Execute()
    {
        beginExecute = true;
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
            door.rotation = Quaternion.Lerp(door.rotation, rot, 2f * Time.deltaTime);
        } else {
            door.position = Vector3.Lerp(door.position, PosMove, 2f * Time.deltaTime);
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
                }

                EditorGUI.BeginChangeCheck();
                float angel = EditorGUILayout.FloatField("Angle", t.angel);
                if(EditorGUI.EndChangeCheck()) {
                    UnityEditor.Undo.RecordObject(t, "update angel");
                    t.angel = angel;
                }
            } else {
                EditorGUI.BeginChangeCheck();
                Vector3 pos = EditorGUILayout.Vector3Field("Move To Position", t.PosMove);
                if(EditorGUI.EndChangeCheck()) {
                    UnityEditor.Undo.RecordObject(t, "update Position Move");
                    t.PosMove = pos;
                }
            }

        }

    }
#endif
}
