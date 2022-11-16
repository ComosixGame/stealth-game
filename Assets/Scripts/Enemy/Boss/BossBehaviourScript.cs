using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(NavMeshAgent))]
public class BossBehaviourScript : MonoBehaviour
{
    public Transform Player, shootPosition;
    public ParticleSystem shotEffect;
    public AudioClip audioClip;
    [Range(0,1)] public float volumeScale;
    public int numberOfBullets, numberOfAttacks;
    public float timeReadyAttack, delayAttack, speedBullet, damage;
    [Range(1,360)] public float angleAttack;
    public LayerMask target;
    private NavMeshAgent agent;
    private Animator animator;
    private int velocityHash;
    private int attackHash;
    private float IdleTimer, chargeTimer, attackTimer, timeNextAttack, angel, angelIncrease;
    private bool readyAttack, isStart;
    private GameManager gameManager;
    private SoundManager soundManager;
    private ObjectPooler objectPooler;
    
    private void Awake() {
        gameManager = GameManager.Instance;
        soundManager = SoundManager.Instance;
        objectPooler = ObjectPooler.Instance;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        velocityHash = Animator.StringToHash("Velocity");
        attackHash = Animator.StringToHash("Attack");
    }

    private void OnEnable() {
        gameManager.OnStart.AddListener(OnStartGame);
    }

    private void Start() {
        angelIncrease = angleAttack/numberOfBullets;
        chargeTimer = Time.time + timeReadyAttack;
        attackTimer = numberOfAttacks;
    }

    private void Update() {
        if(!isStart) return;

        if(Time.time >= chargeTimer) {
            readyAttack = true;
            chargeTimer = Time.time + timeReadyAttack;
        }
        if(!readyAttack) {
            Move();
        } else {
            Attack();
        }
        HandleAnimation();
    }

    private void OnDisable() {
        gameManager.OnStart.RemoveListener(OnStartGame);
    }

    private void Move() {
        if(agent.remainingDistance <= agent.stoppingDistance) {
            IdleTimer += Time.deltaTime;
            if(IdleTimer >= 0.3f) {
                Vector3 pos = RandomNavmeshLocation(agent.height * 2);
                agent.SetDestination(pos);
                IdleTimer = 0;
            }
        }
    }

    private void Attack() {
        attackTimer -= Time.deltaTime;
        if(attackTimer <= 0) {
            readyAttack = false;
            attackTimer = numberOfAttacks;
            animator.SetBool(attackHash, false);
            return;
        }
        if(agent.hasPath) {
            agent.ResetPath();
            agent.SetDestination(transform.position);
        }
        animator.SetBool(attackHash, true);
        Quaternion rotDir = Quaternion.LookRotation(Player.position - transform.position);
        Quaternion rotLook = LerpRotation(Player.position, transform.position, 10);
        transform.rotation = rotLook;
        if(Mathf.Abs(Quaternion.Angle(transform.rotation, rotDir)) <= 1) {
            if(Time.time >= timeNextAttack) {
                shotEffect.Play();
                for( int i = 0; i < numberOfBullets; i ++) {
                    angel = i % 2 == 0? angel + i * angelIncrease : angel - i * angelIncrease ;
                    Vector3 dir = Quaternion.Euler(0, angel, 0) * shootPosition.forward;
                    Quaternion rot = Quaternion.LookRotation(dir.normalized);
                    GameObject c_bullet = objectPooler.SpawnObject("Bullet_L", shootPosition.position, rot);
                    c_bullet.layer = LayerMask.NameToLayer("FromEnemy");
                    soundManager.PlayOneShot(audioClip, volumeScale);
                    c_bullet.GetComponent<Bullet>().TriggerFireBullet(dir.normalized, speedBullet, damage, 150, target);
                    timeNextAttack = Time.time + delayAttack;
                }
                angel = 0;
            }
        }
    }

    private void OnStartGame() {
        isStart = true;
    }

    private Vector3 RandomNavmeshLocation(float radius) {
        //tính random điểm có thể đi trên nav mesh
        Vector3 finalPosition = Vector3.zero;
        Vector3 randomDirection = Random.insideUnitSphere * 10f;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) {
            finalPosition = hit.position;     
        }
        return finalPosition;
    }

    private void HandleAnimation() {
        Vector3 horizontalVelocity = new Vector3(agent.velocity.x, 0, agent.velocity.z);
        float Velocity = horizontalVelocity.magnitude/agent.speed;
        if(Velocity > 0) {
            animator.SetFloat(velocityHash, Velocity);
        } else {
            float v = animator.GetFloat(velocityHash);
            v = Mathf.Lerp(v, -0.1f, 20f * Time.deltaTime);
            animator.SetFloat(velocityHash, v);
        }
    }

    private Quaternion LerpRotation(Vector3 pos1, Vector3 pos2, float speed) {
        //tính nội suy phép quay
        Vector3 dirLook = pos1 - pos2;
        Quaternion rotLook = Quaternion.LookRotation(dirLook.normalized);
        rotLook.x = 0;
        rotLook.z = 0;
        return Quaternion.Lerp(transform.rotation, rotLook, speed * Time.deltaTime);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Handles.color = new Color32(76, 122, 90, 80);
        Vector3 dir = Quaternion.Euler(0, -angleAttack/2, 0) * shootPosition.forward;
        Handles.DrawSolidArc(shootPosition.position, shootPosition.up, dir, angleAttack, 10f);
    }
#endif
}
