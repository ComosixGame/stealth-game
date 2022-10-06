using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public abstract class  Command : MonoBehaviour
{
    public abstract void Execute();
    public abstract void Undo();
}
