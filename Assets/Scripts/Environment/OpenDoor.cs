using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public Axis axis;
    public float angel;
    public float distance;
    private bool beginExecute;
    private Vector3 orginPos;
    private Vector3 axisVector;

    private void Start() {
        orginPos = door.position;
        Execute();
    }
    private void Update() {
        if(beginExecute) {
            Open();
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
            Vector3 pos = door.position;
            pos = orginPos + Vector3.right * distance;
            door.position = Vector3.Lerp(door.position, pos, 2f * Time.deltaTime);
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
        Handles.color = Color.blue;
        Handles.DrawSolidArc(door.position, axisVector, -door.right, angel, 2f);
    }
#endif
}
