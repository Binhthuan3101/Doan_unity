using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class JungleMonsterAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 startPosition; // Vị trí gốc của quái rừng

    [Header("Detection & Combat")]
    public float aggroRange = 6.0f;     // Tầm phát hiện người chơi
    public float attackRange = 2.0f;    // Tầm đánh cận chiến
    public float attackDamage = 18f;
    public float attackCooldown = 1.5f;
    private float nextAttackTime = 0f;

    private Transform targetPlayer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        startPosition = transform.position; // Lưu lại vị trí ban đầu

        // Tự động tìm Player dựa vào Tag "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            targetPlayer = playerObj.transform;
        }
    }

    void Update()
    {
        if (targetPlayer == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.position);
        float distanceFromHome = Vector3.Distance(transform.position, startPosition);

        // Nếu người chơi bước vào tầm vây đánh và cách nhà không quá xa (15m)
        if (distanceToPlayer <= aggroRange && distanceFromHome < 15f)
        {
            agent.SetDestination(targetPlayer.position);

            if (distanceToPlayer <= attackRange)
            {
                if (Time.time >= nextAttackTime)
                {
                    AttackPlayer();
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
        }
        else
        {
            // Nếu người chơi chạy thoát, quái tự động quay về vị trí ban đầu đứng canh
            agent.SetDestination(startPosition);
        }
    }

    void AttackPlayer()
    {
        Debug.Log(gameObject.name + " tấn công trả đũa người chơi!");
        Health playerHealth = targetPlayer.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Hiển thị vòng tròn tầm nhìn trong Editor (Màu vàng)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}