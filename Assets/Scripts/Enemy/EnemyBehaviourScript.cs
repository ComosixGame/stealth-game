using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviourScript : MonoBehaviour
{
    public PlayerScanner playerScanner;
    private NavMeshAgent agent;
    private Mesh mesh;
    private GameObject FieldOfView;


    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        mesh = new Mesh();
        // GetComponent<MeshFilter>().mesh = mesh;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
