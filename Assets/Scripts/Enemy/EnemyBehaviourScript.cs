using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviourScript : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    private NavMeshAgent agent;
    private PlayerScanner playerScanner = new PlayerScanner();

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();

        //creata field of view
        playerScanner.CreataFieldOfView(transform, transform.position);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerScanner.renderFieldOfView(enemy.detectionAngle, enemy.viewDistancee);
    }
}
