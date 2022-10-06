using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public abstract class  Command : MonoBehaviour
{
#if UNITY_EDITOR
    protected List<Transform> commanders;
    private bool showList = true;
    public void SetCommander(Transform commander) {
        commanders.Add(commander);
    }
    public void RemoveCommander(Transform commander) {
        commanders.Remove(commander);
    }
    protected void ShowCommanders() {
        showList = EditorGUILayout.Foldout(showList, "Commanders");
        GUI.enabled = false;
        if(showList) {
            foreach(Transform c in commanders) {
                EditorGUILayout.ObjectField(c, typeof(Transform), false);
            }
        }
        GUI.enabled = true;
    }
#endif

    public abstract void Execute();
    public abstract void Undo();
}
