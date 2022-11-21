using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Hostage : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private int velocityHash;

    private void Awake() {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        velocityHash = Animator.StringToHash("velocity");
    }

    private void Update() {
        HandleAnimation();
    }

    public void Leave(Vector3 position) {
        if(NavMesh.SamplePosition(position, out NavMeshHit hit, agent.height * 2, 1)) {
            agent.SetDestination(hit.position);
        }

        
    }

    public void HandleAnimation() {
        Vector3 horizontalVelocity = new Vector3(agent.velocity.x, 0, agent.velocity.z);
        float velocity = horizontalVelocity.magnitude / agent.speed;
        animator.SetFloat(velocityHash, velocity);
    }
}
