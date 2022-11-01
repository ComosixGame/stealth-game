using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public abstract class Command : MonoBehaviour
{
    public abstract void Execute();
    public abstract void Undo();
}
