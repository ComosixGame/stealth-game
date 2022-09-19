using UnityEngine;

[System.Serializable]
public class PlayerScanner
{
    private Mesh mesh;
    private MeshFilter meshFilterFOV;
    GameObject FieldOfView;
    public void CreataFieldOfView(Transform detector, Vector3 pos) {
        //creata field of view
        mesh = new Mesh();
        FieldOfView = new GameObject("FieldOfView");
        FieldOfView.transform.SetParent(detector);
        FieldOfView.transform.position = pos;
        FieldOfView.transform.rotation = detector.rotation;
        MeshRenderer meshRendererFOV = FieldOfView.AddComponent<MeshRenderer>();
        meshFilterFOV =  FieldOfView.AddComponent<MeshFilter>();
        meshRendererFOV.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshFilterFOV.mesh = mesh;
    }

    public void renderFieldOfView(float fov, float distance) {
        int rayCount = 50;
        float angelIncrease = fov/rayCount;

        //init vetex, uv, triangle
        Vector3[] vertices = new Vector3[rayCount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        float angel = -fov/2;
        vertices[0] = Vector3.zero;
        int triangleIndex = 0;

        //set vetex and triangle for mesh
        for(int i = 0; i<=rayCount; i++) {
            Vector3 dir = Quaternion.Euler(0, angel, 0) * Vector3.forward;
            Vector3 vertex = Vector3.zero +  (dir.normalized * distance);
            vertices[i + 1] = vertex;
            if(i>0) {
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = i;
                triangles[triangleIndex + 2] = i + 1;
                triangleIndex += 3;
            }

            angel += angelIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}
