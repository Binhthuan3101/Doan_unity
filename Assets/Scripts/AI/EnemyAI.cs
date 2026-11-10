using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Lane Movement (Đường giữa)")]
    public Transform[] waypoints; // Kéo các mốc Waypoint của đường giữa vào đây
    private int currentWaypointIndex = 0;

    [Header("Combat & Detection")]
    public float detectionRange = 10f;  // Tầm phát hiện mục tiêu (cả lính lẫn player)
    public float attackRange = 2.5f;     // Tầm đánh
    public float attackDamage = 15f;
    public float attackCooldown = 1.2f;
    public LayerMask enemyLayer;       // Layer của đối phương (Lính ta và Player)

    private float nextAttackTime = 0f;
    private Transform currentTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        MoveToNextWaypoint();
    }

    void Update()
    {
        // BẢO VỆ: Nếu agent đang tắt, chưa khởi tạo xong, hoặc chưa đứng trên NavMesh thì bỏ qua frame này
        if (agent == null || !agent.enabled || !agent.isOnNavMesh) return;

        // 1. Quét tìm mục tiêu gần nhất trong tầm phát hiện
        FindNearestTarget();

        if (currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

            // Đuổi theo mục tiêu
            agent.isStopped = false;
            agent.SetDestination(currentTarget.position);

            // Nếu đã vào tầm đánh
            if (distanceToTarget <= attackRange)
            {
                agent.isStopped = true; // Dừng lại để đánh

                // Quay mặt về phía mục tiêu
                Vector3 lookDir = (currentTarget.position - transform.position).normalized;
                lookDir.y = 0;
                if (lookDir != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(lookDir);
                }

                // Tấn công theo thời gian hồi chiêu
                if (Time.time >= nextAttackTime)
                {
                    AttackTarget();
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
        }
        else
        {
            // 2. Không thấy mục tiêu nào -> Tiếp tục di chuyển theo đường giữa (Waypoints)
            PatrolLane();
        }
    }

    // === HÀM RESET AI KHI HỒI SINH (Rất quan trọng để địch di chuyển lại sau khi chết) ===
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

    void FindNearestTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);

        float shortestDistance = Mathf.Infinity;
        Transform nearest = null;

        foreach (Collider col in hitColliders)
        {
            Health health = col.GetComponent<Health>();
            // Kiểm tra xem mục tiêu còn sống không
            if (health != null && health.currentHealth > 0)
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearest = col.transform;
                }
            }
        }

        currentTarget = nearest;
    }

    void AttackTarget()
    {
        if (currentTarget != null)
        {
            Health targetHealth = currentTarget.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(attackDamage);
                Debug.Log(gameObject.name + " tấn công " + currentTarget.name + " gây " + attackDamage + " sát thương!");

                if (targetHealth.currentHealth <= 0)
                {
                    currentTarget = null;
                }
            }
        }
    }

    void PatrolLane()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        agent.isStopped = false;

        // Khi đã đến gần waypoint hiện tại -> Chuyển sang waypoint tiếp theo
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex < waypoints.Length)
            {
                agent.SetDestination(waypoints[currentWaypointIndex].position);
            }
            else
            {
                currentWaypointIndex = waypoints.Length - 1; // Giữ ở điểm cuối
            }
        }
    }

    void MoveToNextWaypoint()
    {
        if (waypoints != null && waypoints.Length > 0 && agent.isOnNavMesh)
        {
            currentWaypointIndex = Mathf.Clamp(currentWaypointIndex, 0, waypoints.Length - 1);
            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}