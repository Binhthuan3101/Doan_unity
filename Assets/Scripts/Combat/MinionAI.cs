using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MinionAI : MonoBehaviour
{
    [Header("Lane Movement (Đường đi)")]
    public Transform[] waypoints;          // Danh sách các mốc điểm đường đi
    private int currentWaypointIndex = 0;

    [Header("Combat & Detection")]
    public float detectionRange = 10.0f;   // Tầm phát hiện địch
    public float attackRange = 2.5f;       // Tầm đánh thực tế
    public float attackDamage = 10f;       // Sát thương mỗi nhát chém
    public float attackCooldown = 1.2f;    // Thời gian hồi chiêu giữa các nhát chém
    public LayerMask enemyLayer;

    private float nextAttackTime = 0f;
    private Transform currentTarget;
    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>(); // Tự động tìm Animator ở đối tượng con
        MoveToNextWaypoint();
    }

    void Update()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh) return;

        // Kiểm tra mục tiêu còn sống không
        if (currentTarget != null)
        {
            Health targetHealth = currentTarget.GetComponent<Health>();
            if (targetHealth == null || targetHealth.currentHealth <= 0)
            {
                currentTarget = null;
            }
        }

        // 1. Nếu chưa có mục tiêu, tìm kiếm địch hoặc tuần tra
        if (currentTarget == null)
        {
            agent.isStopped = false;
            FindTarget();

            if (currentTarget == null)
            {
                PatrolLane();
            }
        }
        else
        {
            // 2. Đã có mục tiêu -> Kiểm tra khoảng cách
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

            if (distanceToTarget <= attackRange)
            {
                // Tới tầm đánh -> Dừng di chuyển và quay mặt về phía địch
                agent.isStopped = true;

                Vector3 lookDir = (currentTarget.position - transform.position).normalized;
                lookDir.y = 0;
                if (lookDir != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(lookDir);
                }

                // Thực hiện tấn công theo nhịp Cooldown
                if (Time.time >= nextAttackTime)
                {
                    Attack();
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
            else if (distanceToTarget <= detectionRange)
            {
                // Trong tầm phát hiện nhưng chưa tới tầm đánh -> Đuổi theo
                agent.isStopped = false;
                agent.SetDestination(currentTarget.position);
            }
            else
            {
                // Mục tiêu chạy quá xa -> Bỏ mục tiêu, quay lại đi đường
                currentTarget = null;
                agent.isStopped = false;
                MoveToNextWaypoint();
            }
        }
    }

    void FindTarget()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);

        float shortestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;

        foreach (Collider enemyCollider in hitEnemies)
        {
            if (enemyCollider == null) continue;
            Health enemyHealth = enemyCollider.GetComponent<Health>();
            if (enemyHealth != null && enemyHealth.currentHealth > 0)
            {
                float distance = Vector3.Distance(transform.position, enemyCollider.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestEnemy = enemyCollider.transform;
                }
            }
        }

        currentTarget = nearestEnemy;
    }

    void Attack()
    {
        if (currentTarget != null)
        {
            // Kích hoạt Trigger Attack để phát animation vung kiếm mỗi khi đến lượt
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            // Gây sát thương cho đối thủ
            Health enemyHealth = currentTarget.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);

                // Nếu đối thủ hết máu (chết)
                if (enemyHealth.currentHealth <= 0)
                {
                    currentTarget = null;
                    agent.isStopped = false;
                    MoveToNextWaypoint();
                }
            }
            else
            {
                currentTarget = null;
                agent.isStopped = false;
                MoveToNextWaypoint();
            }
        }
    }

    void PatrolLane()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        agent.isStopped = false;

        if (currentWaypointIndex < waypoints.Length && waypoints[currentWaypointIndex] != null)
        {
            if (agent.destination != waypoints[currentWaypointIndex].position && !agent.pathPending)
            {
                agent.SetDestination(waypoints[currentWaypointIndex].position);
            }

            float distanceToWaypoint = Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position);
            if (distanceToWaypoint <= 1.5f)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex < waypoints.Length)
                {
                    agent.SetDestination(waypoints[currentWaypointIndex].position);
                }
                else
                {
                    currentWaypointIndex = waypoints.Length - 1;
                }
            }
        }
    }

    void MoveToNextWaypoint()
    {
        if (waypoints != null && waypoints.Length > 0 && agent.isOnNavMesh)
        {
            currentWaypointIndex = Mathf.Clamp(currentWaypointIndex, 0, waypoints.Length - 1);
            agent.isStopped = false;
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    public void ResetAI()
    {
        currentTarget = null;
        currentWaypointIndex = 0;
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            MoveToNextWaypoint();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}