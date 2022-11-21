using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineConnectRender : MonoBehaviour
{
    public Transform target;
    public Color color;
    private LineRenderer line;
    private MaterialPropertyBlock  materialPropertyBlock;

    private void Awake() {
        line = GetComponent<LineRenderer>();
        materialPropertyBlock = new MaterialPropertyBlock();
        materialPropertyBlock.SetColor("_BaseColor", color);
        line.SetPropertyBlock(materialPropertyBlock);
    }
    
    private void Start() {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, target.position);
    }

    private void Update() {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, target.position);
    }
}
