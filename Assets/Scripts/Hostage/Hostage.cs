using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Hostage : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private int velocityHash;
    private int idleHash;
    private float idleTime;
    private int indexIdle;
    private float randomTime;

    private void Awake() {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        velocityHash = Animator.StringToHash("velocity");
        idleHash = Animator.StringToHash("IdleAnimation");
    }

    private void Start() {
        animator.SetFloat(idleHash, Random.Range(0,5));
        randomTime = Random.Range(5, 15);
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

        if(Time.time >= idleTime) {
            indexIdle = Random.Range(0,5);
            randomTime = Random.Range(5, 15);
            idleTime = Time.time + randomTime;
        }

        float idleAnmation = animator.GetFloat(idleHash);
        if(idleAnmation != indexIdle) {
            float value = Mathf.MoveTowards(idleAnmation, indexIdle, 5f * Time.deltaTime);
            animator.SetFloat(idleHash, value);
        }
    }
}
