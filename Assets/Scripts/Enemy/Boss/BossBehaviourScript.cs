using UnityEngine;
using UnityEngine.AI;
using Cinemachine;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(NavMeshAgent))]
public class BossBehaviourScript : MonoBehaviour
{
    public Boss boss;
    [HideInInspector] public Transform Player;
    public Transform shootPosition;
    public ParticleSystem shotEffect;
    public LayerMask target;
    public CinemachineFreeLook cinemachineFree;
    [Range(0,1)] public float volumeScale;
    public TextMeshProUGUI TextBoss;
    private NavMeshAgent agent;
    private Animator animator;
    private int velocityHash;
    private int fireHash;
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
        fireHash = Animator.StringToHash("Fire");

        cinemachineFree.Priority = 0;
        TextBoss.gameObject.SetActive(true);
        TextBoss.text = boss.nameBoss;
    }

    private void OnEnable() {
        gameManager.OnStart.AddListener(StartCutScene);
    }

    private void Start() {
        angelIncrease = boss.angleAttack/boss.numberOfBullets;
        chargeTimer = Time.time + boss.timeReadyAttack;
        attackTimer = boss.timeAttack;
    }

    private void Update() {
        if(!isStart) return;

        if(Time.time >= chargeTimer) {
            readyAttack = true;
            chargeTimer = Time.time + boss.timeReadyAttack;
        }
        if(!readyAttack) {
            Move();
        } else {
            Attack();
        }
        HandleAnimation();
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Obstacle")) {
            ContactPoint contact = other.GetContact(0);
            IDamageable obstacleDamageable = other.transform.GetComponentInParent<IDamageable>();
            Vector3 dir = other.transform.position -  transform.position;
            dir.y = 0;
            obstacleDamageable.TakeDamge(contact.point, dir.normalized * 10 );
        }
    }

    private void OnDisable() {
        gameManager.OnStart.RemoveListener(StartCutScene);
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
            attackTimer = boss.timeAttack;
            return;
        }
        if(agent.hasPath) {
            agent.ResetPath();
            agent.SetDestination(transform.position);
        }

        Quaternion rotDir = Quaternion.LookRotation(Player.position - transform.position);
        Quaternion rotLook = LerpRotation(Player.position, transform.position, 10);
        transform.rotation = rotLook;
        if(Time.time >= timeNextAttack) {
            //delay trc khi bắn
            Invoke("Fire", boss.delayAttack);
            timeNextAttack = Time.time + boss.delayAttack;
        }
    }

    private void Fire() {
        shotEffect.Play();
        animator.SetTrigger(fireHash);
        //bắn đạn theo nhiều hướng
        for( int i = 0; i < boss.numberOfBullets; i ++) {
            angel = i % 2 == 0? angel + i * angelIncrease : angel - i * angelIncrease ;
            Vector3 dir = Quaternion.Euler(0, angel, 0) * shootPosition.forward;
            Quaternion rot = Quaternion.LookRotation(dir.normalized);
            GameObject c_bullet = objectPooler.SpawnObject("Bullet_L", shootPosition.position, rot);
            c_bullet.layer = LayerMask.NameToLayer("FromEnemy");
            soundManager.PlayOneShot(boss.audioClip, volumeScale);
            c_bullet.GetComponent<Bullet>().TriggerFireBullet(dir.normalized, boss.speedBullet, boss.damage, 100, target);
        }
        angel = 0;
    }

    private void StartActive() {
        isStart = true;
    }

    private void StartCutScene() {
        cinemachineFree.Priority = 100;
        Invoke("EndCutScene", 2f);
    }

    private void EndCutScene() {
        TextBoss.gameObject.SetActive(false);
        cinemachineFree.Priority = -1;
        Invoke("StartActive", 1f);
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
        Vector3 dir = Quaternion.Euler(0, -boss.angleAttack/2, 0) * shootPosition.forward;
        Handles.DrawSolidArc(shootPosition.position, shootPosition.up, dir, boss.angleAttack, 10f);
    }
#endif
}
